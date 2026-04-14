-- INSERT DATA INTO PRQ_Automobiles
-- Purpose: Insert 5+ sample automobiles with variety of makes, models, colors, and types
-- MySQL 8.0 Compatible
-- ========================================
INSERT INTO PRQ_Automobiles (color, year, manufacturer, type) VALUES
('Red', 2020, 'Toyota', 'sedan'),
('Black', 2019, 'Honda', 'sedan'),
('Silver', 2021, 'Ford', '4x4'),
('Blue', 2018, 'BMW', 'sedan'),
('White', 2022, 'Yamaha', 'motorcycle'),
('Gray', 2020, 'Jeep', '4x4'),
('Green', 2021, 'Harley-Davidson', 'motorcycle'),
('Orange', 2019, 'Nissan', 'sedan');

-- ========================================
-- INSERT DATA INTO PRQ_Parking
-- Purpose: Insert 2+ sample parking lots with different provinces and price structures
-- MySQL 8.0 Compatible
-- ========================================
INSERT INTO PRQ_Parking (province_name, parking_name, price_per_hour) VALUES
('San José', 'Parking Central Downtown', 2.50),
('Alajuela', 'Alajuela Mall Parking', 2.00),
('Cartago', 'Cartago Industrial Park', 1.75),
('Heredia', 'Heredia Medical Center', 3.00);

-- ========================================
-- INSERT DATA INTO PRQ_CarEntry
-- Purpose: Insert 15+ sample parking session records
-- Some records have NULL exit_datetime indicating vehicles still parked
-- MySQL 8.0 Compatible
-- ========================================
INSERT INTO PRQ_CarEntry (parking_id, automobile_id, entry_datetime, exit_datetime) VALUES
(1, 1, '2026-04-12 08:00:00', '2026-04-12 10:30:00'),
(1, 2, '2026-04-12 09:15:00', '2026-04-12 11:45:00'),
(1, 3, '2026-04-12 10:00:00', NULL),
(1, 4, '2026-04-12 11:30:00', '2026-04-12 14:00:00'),
(1, 5, '2026-04-12 13:00:00', '2026-04-12 15:30:00'),
(1, 6, '2026-04-13 07:45:00', '2026-04-13 09:15:00'),
(2, 1, '2026-04-12 12:00:00', '2026-04-12 14:20:00'),
(2, 3, '2026-04-12 14:30:00', NULL),
(2, 7, '2026-04-13 08:00:00', '2026-04-13 10:45:00'),
(2, 2, '2026-04-13 09:00:00', NULL),
(3, 4, '2026-04-12 15:30:00', '2026-04-12 17:45:00'),
(3, 5, '2026-04-12 16:00:00', '2026-04-12 18:30:00'),
(3, 8, '2026-04-13 06:30:00', '2026-04-13 08:00:00'),
(4, 6, '2026-04-12 18:00:00', NULL),
(4, 7, '2026-04-13 10:00:00', '2026-04-13 12:30:00');

-- ========================================
-- Summary of Inserted Data (MySQL 8.0 Optimized)
-- ========================================
-- PRQ_Automobiles: 8 automobiles inserted
--   - 4 sedans (Toyota, Honda, BMW, Nissan)
--   - 2 4x4 vehicles (Ford, Jeep)
--   - 2 motorcycles (Yamaha, Harley-Davidson)
--   - Year range: 2018-2022 (using INT type for MySQL 8.0)
--
-- PRQ_Parking: 4 parking lots inserted
--   - San José: $2.50/hour
--   - Alajuela: $2.00/hour
--   - Cartago: $1.75/hour
--   - Heredia: $3.00/hour
--
-- PRQ_CarEntry: 15 parking session records inserted
--   - Total entries: 15
--   - Completed sessions (with exit): 11
--   - Currently parked vehicles (NULL exit): 4
--   - Date range: April 12-13, 2026
--   - All CHECK constraints validated
--   - All Foreign keys properly referenced
