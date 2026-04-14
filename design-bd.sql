-- ========================================
-- Car Parking System Database Schema
-- MySQL Version: 8.0+
-- Tables with prefix: PRQ
-- Created: April 13, 2026
-- ========================================

-- ========================================
-- Table: PRQ_Automobiles
-- Purpose: Store information about vehicles
-- MySQL 8.0 Optimized
-- ========================================

CREATE DATABASE IF NOT EXISTS CarParkingSystem;
USE CarParkingSystem;   

CREATE TABLE PRQ_Automobiles (
    -- Primary Key: Unique identifier for each automobile
    id BIGINT AUTO_INCREMENT PRIMARY KEY COMMENT 'Unique identifier for each automobile',
    color VARCHAR(50) NOT NULL COMMENT 'Vehicle color',
    year INT NOT NULL COMMENT 'Manufacturing year (e.g., 2020)',
    manufacturer VARCHAR(100) NOT NULL COMMENT 'Vehicle manufacturer/brand name',
    type ENUM('sedan', '4x4', 'motorcycle') NOT NULL COMMENT 'Vehicle type classification',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP COMMENT 'Record creation timestamp',
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'Last update timestamp',
    CONSTRAINT ck_year_valid CHECK (year >= 1900 AND year <= 2100),
    CONSTRAINT ck_color_not_empty CHECK (color <> ''),
    CONSTRAINT ck_manufacturer_not_empty CHECK (manufacturer <> ''),
    INDEX idx_manufacturer (manufacturer),
    INDEX idx_type (type),
    INDEX idx_year (year)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='Automobiles table - MySQL 8.0 optimized';

-- ========================================
-- Table: PRQ_Parking
-- Purpose: Store parking lot information
-- MySQL 8.0 Optimized
-- ========================================
CREATE TABLE PRQ_Parking (
    id BIGINT AUTO_INCREMENT PRIMARY KEY COMMENT 'Unique identifier for each parking lot',
    province_name VARCHAR(100) NOT NULL COMMENT 'Province/State name where parking is located',
    parking_name VARCHAR(150) NOT NULL COMMENT 'Name/identifier of the parking lot',
    price_per_hour DECIMAL(10, 2) NOT NULL COMMENT 'Hourly parking rate in currency units',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP COMMENT 'Record creation timestamp',
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'Last update timestamp',
    CONSTRAINT ck_province_not_empty CHECK (province_name <> ''),
    CONSTRAINT ck_parking_name_not_empty CHECK (parking_name <> ''),
    CONSTRAINT ck_price_positive CHECK (price_per_hour > 0),
    CONSTRAINT uk_parking_location UNIQUE (province_name, parking_name),
    INDEX idx_province_name (province_name),
    INDEX idx_parking_name (parking_name)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='Parking lots table - MySQL 8.0 optimized';

-- ========================================
-- Table: PRQ_CarEntry
-- Purpose: Track vehicle entry and exit records
-- MySQL 8.0 Optimized
-- ========================================
CREATE TABLE PRQ_CarEntry (
    consecutive BIGINT AUTO_INCREMENT PRIMARY KEY COMMENT 'Unique consecutive ID for each parking session',
    parking_id BIGINT NOT NULL COMMENT 'Reference to parking lot (FK)',
    automobile_id BIGINT NOT NULL COMMENT 'Reference to automobile (FK)',
    entry_datetime DATETIME NOT NULL COMMENT 'Date and time when vehicle entered the parking lot',
    exit_datetime DATETIME NULL COMMENT 'Date and time when vehicle exited (NULL if still parked)',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP COMMENT 'Record creation timestamp',
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT 'Last update timestamp',
    CONSTRAINT ck_exit_after_entry CHECK (
        exit_datetime IS NULL OR exit_datetime > entry_datetime
    ),
    CONSTRAINT fk_parking_id FOREIGN KEY (parking_id)
        REFERENCES PRQ_Parking(id)
        ON DELETE RESTRICT
        ON UPDATE CASCADE,   
    CONSTRAINT fk_automobile_id FOREIGN KEY (automobile_id)
        REFERENCES PRQ_Automobiles(id)
        ON DELETE RESTRICT
        ON UPDATE CASCADE,
    INDEX idx_parking_id (parking_id),
    INDEX idx_automobile_id (automobile_id),
    INDEX idx_entry_datetime (entry_datetime),
    INDEX idx_exit_datetime (exit_datetime),
    INDEX idx_vehicle_parking (automobile_id, parking_id),
    INDEX idx_entry_exit_composite (entry_datetime, exit_datetime)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='Car entry/exit records table - MySQL 8.0 optimized';

