-- SQL Server drzi iskljucivo podatke o korisnicima (nalog i aktivacija mejla).
-- Telemetrija, uredjaji, racuni, rucna ocitavanja, tarifni modeli i limiti potrosnje
-- cuvaju se u Azure Table / Azure Blob (vidi appsettings: AzureTableOptions / AzureBlobOptions).

-- Aplikacijske tabele NE drzati u sistemu master (rizik i konfuzija sa pogresnom semom).
-- Kreiraj bazu jednom u SSMS ako ne postoji:
--   CREATE DATABASE SmartGrid;
-- SmartGrid.WebApi -> appsettings.json -> SQLServer:Database mora da se poklapa sa ovim USE.
USE SmartGrid;

CREATE TABLE Users (
  idUsers UNIQUEIDENTIFIER NOT NULL,
  email VARCHAR(320) NOT NULL,
  password VARCHAR(255) NOT NULL,
  role VARCHAR(45) NOT NULL,
  accountCreated DATETIME NOT NULL,
  isActivated BIT NOT NULL,
  PRIMARY KEY (idUsers));

CREATE TABLE EmailActivation (
  idEmailActivation UNIQUEIDENTIFIER NOT NULL,
  idUsers UNIQUEIDENTIFIER NOT NULL,
  activationToken VARCHAR(255) NOT NULL,
  createdAt DATETIME NOT NULL,
  expireAt DATETIME NOT NULL,
  PRIMARY KEY (idEmailActivation),
  FOREIGN KEY (idUsers) REFERENCES Users(idUsers));

-- Migracija sa starije verije (ako su tabele postojale u SQL-u):
-- DROP TABLE IF EXISTS ConsumptionLimitNotified;
-- DROP TABLE IF EXISTS DeviceConsumptionLimits;
-- DROP TABLE IF EXISTS TariffModels;

-- Azure Table: TariffModels (ime tabele iz appsettings)
--   PartitionKey = TariffModels, RowKey = proizvoljan GUID, IsActive = true, CreatedAtUtc, Name,
--   GreenZoneVtPrice, GreenZoneNtPrice, BlueZoneVtPrice, BlueZoneNtPrice, RedZoneVtPrice, RedZoneNtPrice,
--   NetworkCostPerKw, SupplierCost, ApprovedPowerKw (kao u starom SQL seedu).

-- Azure Table: ConsumptionLimits — PartitionKey = idUsers (GUID string D), RowKey = idDevice (GUID D), LimitKwh, LimitRsd (opciono).
-- Azure Table: ConsumptionLimitNotified — PartitionKey = idUsers, RowKey = "{deviceId}_{yearMonth}", NotifiedAtUtc.
