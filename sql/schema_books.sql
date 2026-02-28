-- DDL: Library schema for MySQL
-- Drops triggers and tables (safe to run multiple times), creates tables, constraints and triggers.

-- Status enums (numeric):
-- BookCopyStatus: 0=LOST, 1=BORROWED, 2=AVAILABLE
-- BorrowStatus:   0=LOST, 1=ACTIVE,   2=RETURNED

-- Drop triggers and tables (order matters)
DROP TRIGGER IF EXISTS borrows_before_insert;
DROP TRIGGER IF EXISTS borrows_before_update;

SET FOREIGN_KEY_CHECKS = 0;
DROP TABLE IF EXISTS BookAuthors;
DROP TABLE IF EXISTS BookThemes;
DROP TABLE IF EXISTS Borrows;
DROP TABLE IF EXISTS BookCopies;
DROP TABLE IF EXISTS Books;
DROP TABLE IF EXISTS Authors;
DROP TABLE IF EXISTS Themes;
DROP TABLE IF EXISTS Users;
SET FOREIGN_KEY_CHECKS = 1;

-- Users
CREATE TABLE Users (
  id INT AUTO_INCREMENT PRIMARY KEY,
  name VARCHAR(200) NOT NULL,
  email VARCHAR(255) UNIQUE,
  max_concurrent_borrows INT NOT NULL DEFAULT 1,
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4; -- default max concurrent borrows = 1, -1 means banned (cannot borrow)


-- Authors
CREATE TABLE Authors (
  id INT AUTO_INCREMENT PRIMARY KEY,
  first_name VARCHAR(100),
  last_name VARCHAR(100),
  bio TEXT,
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Themes
CREATE TABLE Themes (
  id INT AUTO_INCREMENT PRIMARY KEY,
  name VARCHAR(150) NOT NULL UNIQUE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Books
CREATE TABLE Books (
  id INT AUTO_INCREMENT PRIMARY KEY,
  isbn VARCHAR(20) UNIQUE,
  title VARCHAR(300) NOT NULL,
  description TEXT,
  publisher VARCHAR(200),
  published_date DATE,
  pages INT,
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Book Copies
CREATE TABLE BookCopies (
  id INT AUTO_INCREMENT PRIMARY KEY,
  book_id INT NOT NULL,
  barcode VARCHAR(100) UNIQUE,
  status TINYINT NOT NULL DEFAULT 2,
  penalty_weight TINYINT NOT NULL DEFAULT 1,
  added_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_bookcopies_book FOREIGN KEY (book_id) REFERENCES Books(id) ON DELETE CASCADE,
  CONSTRAINT chk_bookcopy_status CHECK (status IN (0,1,2)),
  CONSTRAINT chk_bookcopy_penalty CHECK (penalty_weight >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4; -- penalty_weight reduces user's max_concurrent_borrows when a borrow is LOST


-- Many-to-many: BookAuthors
CREATE TABLE BookAuthors (
  book_id INT NOT NULL,
  author_id INT NOT NULL,
  role VARCHAR(100) DEFAULT 'author',
  PRIMARY KEY (book_id, author_id),
  CONSTRAINT fk_ba_book FOREIGN KEY (book_id) REFERENCES Books(id) ON DELETE CASCADE,
  CONSTRAINT fk_ba_author FOREIGN KEY (author_id) REFERENCES Authors(id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Many-to-many: BookThemes
CREATE TABLE BookThemes (
  book_id INT NOT NULL,
  theme_id INT NOT NULL,
  PRIMARY KEY (book_id, theme_id),
  CONSTRAINT fk_bt_book FOREIGN KEY (book_id) REFERENCES Books(id) ON DELETE CASCADE,
  CONSTRAINT fk_bt_theme FOREIGN KEY (theme_id) REFERENCES Themes(id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Borrows
CREATE TABLE Borrows (
  id INT AUTO_INCREMENT PRIMARY KEY,
  user_id INT NOT NULL,
  book_copy_id INT NOT NULL,
  borrow_date DATETIME DEFAULT CURRENT_TIMESTAMP,
  due_date DATE,
  return_date DATETIME,
  status TINYINT NOT NULL DEFAULT 1,
  notes TEXT,
  CONSTRAINT fk_borrows_user FOREIGN KEY (user_id) REFERENCES Users(id) ON DELETE CASCADE,
  CONSTRAINT fk_borrows_copy FOREIGN KEY (book_copy_id) REFERENCES BookCopies(id) ON DELETE CASCADE,
  CONSTRAINT chk_borrow_status CHECK (status IN (0,1,2))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Triggers
DELIMITER $$

-- Before insert: enforce status=ACTIVE, ensure book copy is AVAILABLE and respect user's max concurrent borrows
CREATE TRIGGER borrows_before_insert
BEFORE INSERT ON Borrows
FOR EACH ROW
BEGIN
  DECLARE cur_max INT DEFAULT 0;
  DECLARE active_count INT DEFAULT 0;
  DECLARE copy_status INT DEFAULT 0;

  -- Force status to ACTIVE if NULL
  IF NEW.status IS NULL THEN
    SET NEW.status = 1; -- ACTIVE
  END IF;

  -- Only allow inserting with ACTIVE
  IF NEW.status <> 1 THEN
    SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Borrows must be inserted with status ACTIVE';
  END IF;

  -- Book copy must be AVAILABLE
  SELECT status INTO copy_status FROM BookCopies WHERE id = NEW.book_copy_id;
  IF copy_status <> 2 THEN
    SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Book copy is not AVAILABLE';
  END IF;

  -- Check user's max concurrent borrows and banned state
  SELECT max_concurrent_borrows INTO cur_max FROM Users WHERE id = NEW.user_id;
  IF cur_max = -1 THEN
    SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'User is banned from borrowing';
  END IF;

  SELECT COUNT(*) INTO active_count FROM Borrows WHERE user_id = NEW.user_id AND status = 1;
  IF active_count >= cur_max THEN
    SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'User has reached max concurrent borrows';
  END IF;

  -- Mark the book copy as BORROWED
  UPDATE BookCopies SET status = 1 WHERE id = NEW.book_copy_id;
END$$

-- Before update: only allow updates when old status is ACTIVE; change book copy status depending on new borrow status and apply penalties
CREATE TRIGGER borrows_before_update
BEFORE UPDATE ON Borrows
FOR EACH ROW
BEGIN
  DECLARE penalty INT DEFAULT 0;
  -- Only allow update when the existing borrow is ACTIVE
  IF OLD.status <> 1 THEN
    SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Only ACTIVE borrows can be updated';
  END IF;
  -- Do not allow changing the associated book copy
  IF OLD.book_copy_id <> NEW.book_copy_id THEN
    SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Cannot change book_copy_id of a borrow';
  END IF;
  -- Handle status change only when it differs
  IF OLD.status <> NEW.status THEN
    IF NEW.status = 0 THEN
      -- LOST: set book copy LOST and penalize user
      UPDATE BookCopies SET status = 0 WHERE id = OLD.book_copy_id;
      SET NEW.return_date = NOW();
      SELECT penalty_weight INTO penalty FROM BookCopies WHERE id = OLD.book_copy_id;
      UPDATE Users
      SET max_concurrent_borrows = CASE
        WHEN max_concurrent_borrows - penalty < 0 THEN -1
        ELSE max_concurrent_borrows - penalty
      END
      WHERE id = OLD.user_id;
    ELSEIF NEW.status = 2 THEN
      -- RETURNED: set book copy AVAILABLE
      UPDATE BookCopies SET status = 2 WHERE id = OLD.book_copy_id;
      SET NEW.return_date = NOW();
    ELSE
      SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Invalid new status for borrow update';
    END IF;
  END IF;
END$$

DELIMITER ;

-- Helpful comments about statuses
-- BookCopyStatus: 0=LOST, 1=BORROWED, 2=AVAILABLE
-- BorrowStatus:   0=LOST, 1=ACTIVE,   2=RETURNED
