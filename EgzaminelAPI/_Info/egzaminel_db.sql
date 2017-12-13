-- phpMyAdmin SQL Dump
-- version 4.7.4
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1:3306
-- Generation Time: Dec 13, 2017 at 10:08 AM
-- Server version: 5.7.19
-- PHP Version: 5.6.31

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `egzaminel_db`
--

-- --------------------------------------------------------

--
-- Table structure for table `events_groups`
--

DROP TABLE IF EXISTS `events_groups`;
CREATE TABLE IF NOT EXISTS `events_groups` (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT 'id',
  `name` tinytext COMMENT 'nazwa wydarzenia (np. kolokwium)',
  `description` text COMMENT 'opis wydarzenia',
  `date` timestamp NULL DEFAULT NULL COMMENT 'data wydarzenia',
  `place` tinytext COMMENT 'miejsce wydarzenia',
  `parent_id` int(11) NOT NULL COMMENT 'grupa studencka',
  `last_update` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `group_id` (`parent_id`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=latin2;

--
-- Triggers `events_groups`
--
DROP TRIGGER IF EXISTS `event_groups_last_upadate_start`;
DELIMITER $$
CREATE TRIGGER `event_groups_last_upadate_start` BEFORE INSERT ON `events_groups` FOR EACH ROW SET NEW.last_update = NOW()
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `events_subjects`
--

DROP TABLE IF EXISTS `events_subjects`;
CREATE TABLE IF NOT EXISTS `events_subjects` (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT 'id',
  `name` tinytext COMMENT 'nazwa wydarzenia (np. kolokwium)',
  `description` text COMMENT 'opis wydarzenia',
  `date` timestamp NULL DEFAULT NULL COMMENT 'data wydarzenia',
  `place` tinytext COMMENT 'miejsce wydarzenia',
  `parent_id` int(11) NOT NULL COMMENT 'grupa studencka',
  `last_update` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `subject_id` (`parent_id`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=latin2;

--
-- Triggers `events_subjects`
--
DROP TRIGGER IF EXISTS `event_subjects_last_upadate_start`;
DELIMITER $$
CREATE TRIGGER `event_subjects_last_upadate_start` BEFORE INSERT ON `events_subjects` FOR EACH ROW SET NEW.last_update = NOW()
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `events_subjects_groups`
--

DROP TABLE IF EXISTS `events_subjects_groups`;
CREATE TABLE IF NOT EXISTS `events_subjects_groups` (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT 'id',
  `name` tinytext COMMENT 'nazwa wydarzenia (np. kolokwium)',
  `description` text COMMENT 'opis wydarzenia',
  `date` timestamp NULL DEFAULT NULL COMMENT 'data wydarzenia',
  `place` tinytext COMMENT 'miejsce wydarzenia',
  `parent_id` int(11) NOT NULL COMMENT 'grupa studencka',
  `last_update` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `subject_group_id` (`parent_id`)
) ENGINE=InnoDB AUTO_INCREMENT=36 DEFAULT CHARSET=latin2;

--
-- Triggers `events_subjects_groups`
--
DROP TRIGGER IF EXISTS `event_subjects_groups_last_upadate_start`;
DELIMITER $$
CREATE TRIGGER `event_subjects_groups_last_upadate_start` BEFORE INSERT ON `events_subjects_groups` FOR EACH ROW SET NEW.last_update = NOW()
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `groups`
--

DROP TABLE IF EXISTS `groups`;
CREATE TABLE IF NOT EXISTS `groups` (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT 'id',
  `name` tinytext NOT NULL COMMENT 'nazwa grupy',
  `description` text,
  `password` varchar(60) NOT NULL COMMENT 'hasło grupy',
  `owner` int(11) NOT NULL COMMENT 'właściciel grupy (id uzytkownika)',
  `last_update` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `owner` (`owner`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=latin2;

--
-- Triggers `groups`
--
DROP TRIGGER IF EXISTS `groups_last_upadate_start`;
DELIMITER $$
CREATE TRIGGER `groups_last_upadate_start` BEFORE INSERT ON `groups` FOR EACH ROW SET NEW.last_update = NOW()
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `groups_permissions`
--

DROP TABLE IF EXISTS `groups_permissions`;
CREATE TABLE IF NOT EXISTS `groups_permissions` (
  `user_id` int(11) NOT NULL COMMENT 'id uzytkownika',
  `object_id` int(11) NOT NULL COMMENT 'id grupy',
  `has_admin_permission` tinyint(1) NOT NULL DEFAULT '0',
  `can_modify` tinyint(1) NOT NULL DEFAULT '0' COMMENT 'czy uzytkownik ma prawa do modyfikacji',
  `last_update` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`user_id`,`object_id`),
  KEY `user_id` (`user_id`),
  KEY `groups_permissions_ibfk_2` (`object_id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin2;

--
-- Triggers `groups_permissions`
--
DROP TRIGGER IF EXISTS `groups_permissions_last_update_start`;
DELIMITER $$
CREATE TRIGGER `groups_permissions_last_update_start` BEFORE INSERT ON `groups_permissions` FOR EACH ROW SET NEW.last_update = NOW()
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `subjects`
--

DROP TABLE IF EXISTS `subjects`;
CREATE TABLE IF NOT EXISTS `subjects` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` tinytext NOT NULL COMMENT 'nazwa przedmiotu (kursu)',
  `description` text COMMENT 'opis',
  `group_id` int(11) NOT NULL COMMENT 'id grupy',
  `last_update` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `group_id` (`group_id`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=latin2;

--
-- Triggers `subjects`
--
DROP TRIGGER IF EXISTS `subject_last_update_start`;
DELIMITER $$
CREATE TRIGGER `subject_last_update_start` BEFORE INSERT ON `subjects` FOR EACH ROW SET NEW.last_update = NOW()
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `subject_groups`
--

DROP TABLE IF EXISTS `subject_groups`;
CREATE TABLE IF NOT EXISTS `subject_groups` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `subject_id` int(11) NOT NULL,
  `place` tinytext,
  `teacher` tinytext,
  `description` text,
  `last_update` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `subject_id` (`subject_id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=latin2;

--
-- Triggers `subject_groups`
--
DROP TRIGGER IF EXISTS `subject_terms_last_upadate_start`;
DELIMITER $$
CREATE TRIGGER `subject_terms_last_upadate_start` BEFORE INSERT ON `subject_groups` FOR EACH ROW SET NEW.last_update = NOW()
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `subject_groups_permissions`
--

DROP TABLE IF EXISTS `subject_groups_permissions`;
CREATE TABLE IF NOT EXISTS `subject_groups_permissions` (
  `user_id` int(11) NOT NULL COMMENT 'id uzytkownika',
  `object_id` int(11) NOT NULL COMMENT 'id przedmiotu',
  `has_admin_permission` tinyint(1) NOT NULL DEFAULT '0',
  `can_modify` tinyint(1) NOT NULL DEFAULT '0' COMMENT 'czy moze edytowac',
  `last_update` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`user_id`,`object_id`),
  KEY `user_id` (`user_id`),
  KEY `subjects_groups_permission_ibfk_1` (`object_id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Triggers `subject_groups_permissions`
--
DROP TRIGGER IF EXISTS `subject_terms_permission_last_upadate_start`;
DELIMITER $$
CREATE TRIGGER `subject_terms_permission_last_upadate_start` BEFORE INSERT ON `subject_groups_permissions` FOR EACH ROW SET NEW.last_update = NOW()
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
CREATE TABLE IF NOT EXISTS `users` (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT 'id uzytkownika',
  `username` varchar(60) NOT NULL COMMENT 'nazwa uzytkownika',
  `password` varchar(32) NOT NULL COMMENT 'haslo (zaszyfrowane)',
  `salt` text NOT NULL COMMENT 'klucz publiczny',
  `email` varchar(60) NOT NULL COMMENT 'email uzytkownika',
  `last_update` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `username` (`username`,`email`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=latin2;

--
-- Triggers `users`
--
DROP TRIGGER IF EXISTS `user_last_upadate_start`;
DELIMITER $$
CREATE TRIGGER `user_last_upadate_start` BEFORE INSERT ON `users` FOR EACH ROW SET NEW.last_update = NOW()
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `users_token`
--

DROP TABLE IF EXISTS `users_token`;
CREATE TABLE IF NOT EXISTS `users_token` (
  `token_id` int(11) NOT NULL AUTO_INCREMENT,
  `user_id` int(11) NOT NULL,
  `auth_token` varchar(250) CHARACTER SET latin2 NOT NULL,
  `issued_on` timestamp NOT NULL,
  `expires_on` timestamp NOT NULL,
  PRIMARY KEY (`token_id`),
  UNIQUE KEY `user_id` (`user_id`)
) ENGINE=InnoDB AUTO_INCREMENT=61 DEFAULT CHARSET=latin1;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `events_groups`
--
ALTER TABLE `events_groups`
  ADD CONSTRAINT `events_groups_ibfk_2` FOREIGN KEY (`parent_id`) REFERENCES `groups` (`id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Constraints for table `events_subjects`
--
ALTER TABLE `events_subjects`
  ADD CONSTRAINT `events_subjects_ibfk_2` FOREIGN KEY (`parent_id`) REFERENCES `subjects` (`id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Constraints for table `events_subjects_groups`
--
ALTER TABLE `events_subjects_groups`
  ADD CONSTRAINT `events_subjects_groups_ibfk_2` FOREIGN KEY (`parent_id`) REFERENCES `subject_groups` (`id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Constraints for table `groups`
--
ALTER TABLE `groups`
  ADD CONSTRAINT `groups_ibfk_3` FOREIGN KEY (`owner`) REFERENCES `users` (`id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Constraints for table `groups_permissions`
--
ALTER TABLE `groups_permissions`
  ADD CONSTRAINT `groups_permissions_ibfk_2` FOREIGN KEY (`object_id`) REFERENCES `groups` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `groups_permissions_ibfk_3` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Constraints for table `subjects`
--
ALTER TABLE `subjects`
  ADD CONSTRAINT `subjects_ibfk_1` FOREIGN KEY (`group_id`) REFERENCES `groups` (`id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Constraints for table `subject_groups`
--
ALTER TABLE `subject_groups`
  ADD CONSTRAINT `subject_groups_ibfk_2` FOREIGN KEY (`subject_id`) REFERENCES `subjects` (`id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Constraints for table `subject_groups_permissions`
--
ALTER TABLE `subject_groups_permissions`
  ADD CONSTRAINT `subject_groups_permissions_ibfk_1` FOREIGN KEY (`object_id`) REFERENCES `subject_groups` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `subject_groups_permissions_ibfk_2` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Constraints for table `users_token`
--
ALTER TABLE `users_token`
  ADD CONSTRAINT `users_token_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
