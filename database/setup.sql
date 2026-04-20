

CREATE TABLE IF NOT EXISTS `public_warehouses` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) NOT NULL,
  `address` varchar(255) NOT NULL,
  `price` int(11) NOT NULL DEFAULT 5000000,
  `capacity` int(11) NOT NULL DEFAULT 5000,
  `total_units` int(11) NOT NULL DEFAULT 20,
  `pos` text NOT NULL,
  `interior_pos` text NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `public_warehouse_units` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `warehouse_id` int(11) NOT NULL,
  `slot_number` int(11) NOT NULL,
  `owner_uuid` int(11) DEFAULT NULL,
  `family_id` int(11) DEFAULT NULL,
  `locked` tinyint(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`id`),
  CONSTRAINT `fk_warehouse` FOREIGN KEY (`warehouse_id`) REFERENCES `public_warehouses` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT IGNORE INTO `public_warehouses` (`id`, `name`, `address`, `price`, `capacity`, `total_units`, `pos`, `interior_pos`) 
VALUES (1, 'Средний склад №5', 'Эксепшионалист-вэй, 5', 6800000, 8000, 20, 
'{"X": 893.59, "Y": -2346.51, "Z": 30.34}', 
'{"X": 1048.22, "Y": -3096.34, "Z": -39.0}');
