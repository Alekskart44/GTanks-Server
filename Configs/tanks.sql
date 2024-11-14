-- phpMyAdmin SQL Dump
-- version 3.5.1
-- http://www.phpmyadmin.net
--
-- Хост: 127.0.0.1
-- Время создания: Ноя 14 2024 г., 19:25
-- Версия сервера: 5.5.25
-- Версия PHP: 5.3.13

SET SQL_MODE="NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- База данных: `tanks`
--

-- --------------------------------------------------------

--
-- Структура таблицы `black_ips`
--

CREATE TABLE IF NOT EXISTS `black_ips` (
  `idblack_ips` bigint(20) NOT NULL AUTO_INCREMENT,
  `ip` varchar(255) NOT NULL,
  PRIMARY KEY (`idblack_ips`),
  UNIQUE KEY `idblack_ips` (`idblack_ips`)
) ENGINE=MyISAM  DEFAULT CHARSET=latin1 AUTO_INCREMENT=11 ;

-- --------------------------------------------------------

--
-- Структура таблицы `daily_bonus_info`
--

CREATE TABLE IF NOT EXISTS `daily_bonus_info` (
  `uid` bigint(20) NOT NULL AUTO_INCREMENT,
  `last_issue_bonuses` datetime NOT NULL,
  PRIMARY KEY (`uid`),
  UNIQUE KEY `uid` (`uid`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1 AUTO_INCREMENT=1 ;

-- --------------------------------------------------------

--
-- Структура таблицы `garages`
--

CREATE TABLE IF NOT EXISTS `garages` (
  `uid` bigint(20) NOT NULL AUTO_INCREMENT,
  `colormaps` longtext NOT NULL,
  `hulls` longtext NOT NULL,
  `inventory` longtext NOT NULL,
  `turrets` longtext NOT NULL,
  `userid` varchar(255) NOT NULL,
  `effects` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`uid`),
  UNIQUE KEY `uid` (`uid`),
  UNIQUE KEY `userid` (`userid`)
) ENGINE=MyISAM  DEFAULT CHARSET=latin1 AUTO_INCREMENT=55 ;

--
-- Дамп данных таблицы `garages`
--

INSERT INTO `garages` (`uid`, `colormaps`, `hulls`, `inventory`, `turrets`, `userid`, `effects`) VALUES
(48, '{"colormaps":[{"id":"green","modification":0,"mounted":false},{"id":"holiday","modification":0,"mounted":false},{"id":"tundra","modification":0,"mounted":true}]}', '{"hulls":[{"id":"wasp","modification":3,"mounted":false},{"id":"viking","modification":2,"mounted":true},{"id":"dictator","modification":0,"mounted":false},{"id":"hunter","modification":0,"mounted":false},{"id":"hornet","modification":0,"mounted":false},{"id":"titan","modification":0,"mounted":false},{"id":"mamont","modification":1,"mounted":false}]}', '{"inventory":[{"id":"health","count":1280},{"id":"mine","count":1258},{"id":"armor","count":4936},{"id":"n2o","count":5458},{"id":"double_damage","count":4935}]}', '{"turrets":[{"id":"smoky","modification":3,"mounted":true},{"id":"frezee","modification":1,"mounted":false},{"id":"thunder","modification":2,"mounted":false},{"id":"railgun","modification":1,"mounted":false},{"id":"flamethrower","modification":1,"mounted":false},{"id":"ricochet","modification":0,"mounted":false}]}', 'test', NULL),
(49, '{"colormaps":[{"id":"green","modification":0,"mounted":true},{"id":"holiday","modification":0,"mounted":false}]}', '{"hulls":[{"id":"wasp","modification":0,"mounted":true}]}', '{"inventory":[]}', '{"turrets":[{"id":"smoky","modification":0,"mounted":true}]}', 'test1', NULL),
(51, '{"colormaps":[{"id":"green","modification":0,"mounted":true},{"id":"holiday","modification":0,"mounted":false}]}', '{"hulls":[{"id":"wasp","modification":0,"mounted":true}]}', '{"inventory":[]}', '{"turrets":[{"id":"smoky","modification":0,"mounted":true}]}', 'test12', NULL),
(52, '{"colormaps":[{"id":"green","modification":0,"mounted":true},{"id":"holiday","modification":0,"mounted":false}]}', '{"hulls":[{"id":"wasp","modification":0,"mounted":true}]}', '{"inventory":[]}', '{"turrets":[{"id":"smoky","modification":0,"mounted":true}]}', 'dds12', NULL),
(53, '{"colormaps":[{"id":"green","modification":0,"mounted":true},{"id":"holiday","modification":0,"mounted":false}]}', '{"hulls":[{"id":"wasp","modification":0,"mounted":true}]}', '{"inventory":[]}', '{"turrets":[{"id":"smoky","modification":0,"mounted":true}]}', 'dswe', NULL),
(54, '{"colormaps":[{"id":"green","modification":0,"mounted":true}]}', '{"hulls":[{"id":"viking","modification":0,"mounted":true}]}', '{"inventory":[]}', '{"turrets":[{"id":"smoky","modification":3,"mounted":true}]}', 'fdeer', '{"effects":[]}');

-- --------------------------------------------------------

--
-- Структура таблицы `karma`
--

CREATE TABLE IF NOT EXISTS `karma` (
  `idkarma` bigint(20) NOT NULL AUTO_INCREMENT,
  `chat_banned` bit(1) DEFAULT NULL,
  `chat_banned_before` datetime DEFAULT NULL,
  `game_banned` bit(1) DEFAULT NULL,
  `game_banned_before` datetime DEFAULT NULL,
  `reason_for_chat_ban` varchar(255) DEFAULT NULL,
  `reason_for_game_ban` varchar(255) DEFAULT NULL,
  `userid` varchar(255) NOT NULL,
  `banner_chat_user_id` varchar(255) DEFAULT NULL,
  `banner_game_user_id` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`idkarma`)
) ENGINE=MyISAM  DEFAULT CHARSET=latin1 AUTO_INCREMENT=53 ;

--
-- Дамп данных таблицы `karma`
--

INSERT INTO `karma` (`idkarma`, `chat_banned`, `chat_banned_before`, `game_banned`, `game_banned_before`, `reason_for_chat_ban`, `reason_for_game_ban`, `userid`, `banner_chat_user_id`, `banner_game_user_id`) VALUES
(46, b'0', NULL, b'0', NULL, NULL, NULL, 'test', NULL, NULL),
(47, b'0', NULL, b'0', NULL, NULL, NULL, 'test1', NULL, NULL),
(49, b'0', NULL, b'0', NULL, NULL, NULL, 'test12', NULL, NULL),
(50, b'0', NULL, b'0', NULL, NULL, NULL, 'dds12', NULL, NULL),
(51, b'0', NULL, b'0', NULL, NULL, NULL, 'dswe', NULL, NULL),
(52, b'0', NULL, b'0', NULL, NULL, NULL, 'fdeer', NULL, NULL);

-- --------------------------------------------------------

--
-- Структура таблицы `logs`
--

CREATE TABLE IF NOT EXISTS `logs` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `date` datetime NOT NULL,
  `message` longtext NOT NULL,
  `type` varchar(255) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM  DEFAULT CHARSET=latin1 AUTO_INCREMENT=298 ;

-- --------------------------------------------------------

--
-- Структура таблицы `payment`
--

CREATE TABLE IF NOT EXISTS `payment` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `date` datetime NOT NULL,
  `id_payment` bigint(20) NOT NULL,
  `nickname` varchar(255) NOT NULL,
  `status` tinyint(4) NOT NULL,
  `summ` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id` (`id`),
  UNIQUE KEY `date` (`date`),
  UNIQUE KEY `id_payment` (`id_payment`),
  UNIQUE KEY `nickname` (`nickname`),
  UNIQUE KEY `status` (`status`),
  UNIQUE KEY `summ` (`summ`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1 AUTO_INCREMENT=1 ;

-- --------------------------------------------------------

--
-- Структура таблицы `users`
--

CREATE TABLE IF NOT EXISTS `users` (
  `uid` bigint(20) NOT NULL AUTO_INCREMENT,
  `crystalls` bigint(11) NOT NULL,
  `email` varchar(255) DEFAULT NULL,
  `last_ip` varchar(255) NOT NULL,
  `last_issue_bonus` datetime DEFAULT NULL,
  `next_score` bigint(11) NOT NULL,
  `nickname` varchar(255) NOT NULL,
  `password` varchar(255) NOT NULL,
  `place` bigint(11) NOT NULL,
  `rank` bigint(11) NOT NULL,
  `rating` bigint(11) NOT NULL,
  `score` bigint(11) NOT NULL,
  `user_type` bigint(11) NOT NULL,
  PRIMARY KEY (`uid`),
  UNIQUE KEY `uid` (`uid`),
  UNIQUE KEY `nickname` (`nickname`),
  UNIQUE KEY `email` (`email`),
  KEY `uid_2` (`uid`),
  KEY `crystalls_2` (`crystalls`),
  KEY `password` (`password`) USING BTREE,
  KEY `next_score` (`next_score`) USING BTREE,
  KEY `score` (`score`) USING BTREE,
  KEY `crystalls` (`crystalls`) USING BTREE,
  KEY `user_type` (`user_type`) USING BTREE,
  KEY `place` (`place`) USING BTREE,
  KEY `rating` (`rating`) USING BTREE,
  KEY `rank` (`rank`) USING BTREE
) ENGINE=MyISAM  DEFAULT CHARSET=latin1 AUTO_INCREMENT=54 ;

--
-- Дамп данных таблицы `users`
--

INSERT INTO `users` (`uid`, `crystalls`, `email`, `last_ip`, `last_issue_bonus`, `next_score`, `nickname`, `password`, `place`, `rank`, `rating`, `score`, `user_type`) VALUES
(47, 66685, 'alekskart@alekskart.com', '127.0.0.1:62317', '2024-09-13 10:44:45', 125000, 'test', 'test', 0, 12, 1, 111330, 2),
(48, 5, NULL, '/127.0.0.1:53352', NULL, 100, 'test1', '123123', 0, 0, 1, 20, 0),
(50, 5, NULL, '/127.0.0.1:55712', NULL, 100, 'test12', '123123', 0, 0, 1, 0, 0),
(51, 5, NULL, '/127.0.0.1:53430', NULL, 100, 'dds12', '123123', 0, 0, 1, 0, 0),
(52, 5, NULL, '/127.0.0.1:53790', NULL, 100, 'dswe', '123123', 0, 0, 1, 0, 0),
(53, 2, 'default@gtanks.com', '127.0.0.1:62311', '2024-09-28 14:15:41', 125000, 'fdeer', 'fdeer1', 0, 12, 0, 121110, 2);

-- --------------------------------------------------------

--
-- Структура таблицы `__efmigrationshistory`
--

CREATE TABLE IF NOT EXISTS `__efmigrationshistory` (
  `MigrationId` varchar(150) NOT NULL,
  `ProductVersion` varchar(32) NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
