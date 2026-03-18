-- Data insert file for library schema
-- Deletes data from all tables and inserts sample rows for testing

SET FOREIGN_KEY_CHECKS = 0;
DELETE FROM Borrows;
DELETE FROM BookAuthors;
DELETE FROM BookThemes;
DELETE FROM BookCopies;
DELETE FROM Books;
DELETE FROM Authors;
DELETE FROM Themes;
DELETE FROM Users;
SET FOREIGN_KEY_CHECKS = 1;

-- Users
INSERT INTO Users (id, name, email, max_concurrent_borrows) VALUES
(1, 'Alice Smith', 'alice@example.com', 1),
(2, 'Bob Johnson', 'bob@example.com', 2),
(3, 'Carol Lee', 'carol@example.com', 3);

-- Authors
INSERT INTO Authors (id, first_name, last_name, bio) VALUES
(1, 'George', 'Orwell', 'English novelist and essayist'),
(2, 'J.K.', 'Rowling', 'British author, best known for the Harry Potter series'),
(3, 'J.R.R.', 'Tolkien', 'English writer, poet, philologist');

-- Themes
INSERT INTO Themes (id, name) VALUES
(1, 'Dystopia'),
(2, 'Fantasy'),
(3, 'Adventure');

-- Books
INSERT INTO Books (id, isbn, title, description, publisher, published_date, pages) VALUES
(1, '9780451524935', '1984', 'Dystopian novel', 'Secker & Warburg', '1949-06-08', 328),
(2, '9780544003415', 'The Hobbit', 'Fantasy novel', 'George Allen & Unwin', '1937-09-21', 310),
(3, '9780747532743', 'Harry Potter and the Philosopher''s Stone', 'First book of Harry Potter', 'Bloomsbury', '1997-06-26', 223);

-- Book copies (two copies for book 1, one for others). copy 4 has heavy penalty
INSERT INTO BookCopies (id, book_id, barcode, status, penalty_weight) VALUES
(1, 1, 'BC-1984-001', 2, 1), -- AVAILABLE
(2, 1, 'BC-1984-002', 2, 1), -- AVAILABLE
(3, 2, 'BC-HOBBIT-001', 2, 1),
(4, 3, 'BC-HP1-001', 2, 4); -- heavy penalty (4)

-- BookAuthors
INSERT INTO BookAuthors (book_id, author_id, role) VALUES
(1, 1, 'author'),
(2, 3, 'author'),
(3, 2, 'author');

-- BookThemes
INSERT INTO BookThemes (book_id, theme_id) VALUES
(1, 1),
(2, 2),
(2, 3),
(3, 2);

-- Borrows sequence (10 borrows): insert as ACTIVE (1) and then update to desired state so triggers are exercised

-- Borrow 1: user2 borrows copy1 and returns it
INSERT INTO Borrows (id, user_id, book_copy_id, borrow_date, due_date, status, notes) VALUES
(1, 2, 1, NOW() - INTERVAL 30 DAY, DATE_ADD(NOW() - INTERVAL 30 DAY, INTERVAL 14 DAY), 1, 'Borrow 1 - returned');
UPDATE Borrows SET status = 2 WHERE id = 1;

-- Borrow 2: user2 borrows copy1 and loses it (penalty 1)
INSERT INTO Borrows (id, user_id, book_copy_id, borrow_date, due_date, status, notes) VALUES
(2, 2, 1, NOW() - INTERVAL 15 DAY, DATE_ADD(NOW() - INTERVAL 15 DAY, INTERVAL 14 DAY), 1, 'Borrow 2 - lost');
UPDATE Borrows SET status = 0 WHERE id = 2;

-- Borrow 3: user1 borrows copy2 and returns it
INSERT INTO Borrows (id, user_id, book_copy_id, borrow_date, due_date, status, notes) VALUES
(3, 1, 2, NOW() - INTERVAL 20 DAY, DATE_ADD(NOW() - INTERVAL 20 DAY, INTERVAL 14 DAY), 1, 'Borrow 3 - returned');
UPDATE Borrows SET status = 2 WHERE id = 3;

-- Borrow 4: user3 borrows copy3 and is still active
INSERT INTO Borrows (id, user_id, book_copy_id, borrow_date, due_date, status, notes) VALUES
(4, 3, 3, NOW() - INTERVAL 2 DAY, DATE_ADD(NOW() - INTERVAL 2 DAY, INTERVAL 14 DAY), 1, 'Borrow 4 - active');

-- Borrow 5: user2 borrows copy4 and loses it (heavy penalty 4) -> user2 becomes banned (-1)
INSERT INTO Borrows (id, user_id, book_copy_id, borrow_date, due_date, status, notes) VALUES
(5, 2, 4, NOW() - INTERVAL 5 DAY, DATE_ADD(NOW() - INTERVAL 5 DAY, INTERVAL 14 DAY), 1, 'Borrow 5 - lost heavy');
UPDATE Borrows SET status = 0 WHERE id = 5;

-- Borrow 6: user3 borrows copy2 and returns it
INSERT INTO Borrows (id, user_id, book_copy_id, borrow_date, due_date, status, notes) VALUES
(6, 3, 2, NOW() - INTERVAL 10 DAY, DATE_ADD(NOW() - INTERVAL 10 DAY, INTERVAL 14 DAY), 1, 'Borrow 6 - returned');
UPDATE Borrows SET status = 2 WHERE id = 6;

-- Borrow 7: user3 borrows copy2 again and returns it
INSERT INTO Borrows (id, user_id, book_copy_id, borrow_date, due_date, status, notes) VALUES
(7, 3, 2, NOW() - INTERVAL 1 DAY, DATE_ADD(NOW() - INTERVAL 1 DAY, INTERVAL 14 DAY), 1, 'Borrow 7 - returned');
UPDATE Borrows SET status = 2 WHERE id = 7;

-- Borrow 8: user1 borrows copy2 and returns it
INSERT INTO Borrows (id, user_id, book_copy_id, borrow_date, due_date, status, notes) VALUES
(8, 1, 2, NOW(), DATE_ADD(CURDATE(), INTERVAL 14 DAY), 1, 'Borrow 8 - returned');
UPDATE Borrows SET status = 2 WHERE id = 8;

-- Borrow 9: finish borrow 4 (user3) as returned, then user1 borrows copy3 and returns
UPDATE Borrows SET status = 2 WHERE id = 4;
INSERT INTO Borrows (id, user_id, book_copy_id, borrow_date, due_date, status, notes) VALUES
(9, 1, 3, NOW() - INTERVAL 3 DAY, DATE_ADD(NOW() - INTERVAL 3 DAY, INTERVAL 14 DAY), 1, 'Borrow 9 - returned');
UPDATE Borrows SET status = 2 WHERE id = 9;

-- Borrow 10: user3 borrows copy3 and remains active
INSERT INTO Borrows (id, user_id, book_copy_id, borrow_date, due_date, status, notes) VALUES
(10, 3, 3, NOW(), DATE_ADD(CURDATE(), INTERVAL 14 DAY), 1, 'Borrow 10 - active');

-- End of data file
