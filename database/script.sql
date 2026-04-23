
CREATE TABLE Users (
  idUsers VARCHAR(255) NOT NULL,
  email VARCHAR(45) NOT NULL,
  password VARCHAR(255) NOT NULL,
  role VARCHAR(45) NOT NULL,
  accountCreated DATETIME NOT NULL,
  PRIMARY KEY (idUsers));

CREATE TABLE TariffModels (
  id INT IDENTITY(1,1) PRIMARY KEY,
  name VARCHAR(80) NOT NULL,
  isActive BIT NOT NULL,
  createdAt DATETIME NOT NULL,
  greenZoneVtPrice FLOAT NOT NULL,
  greenZoneNtPrice FLOAT NOT NULL,
  blueZoneVtPrice FLOAT NOT NULL,
  blueZoneNtPrice FLOAT NOT NULL,
  redZoneVtPrice FLOAT NOT NULL,
  redZoneNtPrice FLOAT NOT NULL,
  networkCostPerKw FLOAT NOT NULL,
  supplierCost FLOAT NOT NULL,
  approvedPowerKw FLOAT NOT NULL
);

CREATE TABLE MonthlyBills (
  id INT IDENTITY(1,1) PRIMARY KEY,
  deviceId VARCHAR(128) NOT NULL,
  year INT NOT NULL,
  month INT NOT NULL,
  totalKwh FLOAT NOT NULL,
  higherTariffKwh FLOAT NOT NULL,
  lowerTariffKwh FLOAT NOT NULL,
  greenZoneKwh FLOAT NOT NULL,
  blueZoneKwh FLOAT NOT NULL,
  redZoneKwh FLOAT NOT NULL,
  energyCost FLOAT NOT NULL,
  fixedCosts FLOAT NOT NULL,
  totalCost FLOAT NOT NULL,
  billText NVARCHAR(MAX) NOT NULL,
  generatedAtUtc DATETIME NOT NULL,
  CONSTRAINT UQ_MonthlyBills_Device_Period UNIQUE (deviceId, year, month)
);

-- Opcioni seed (pokrenuti kada telemetrija bude spremna i zelite obracun):
-- INSERT INTO TariffModels (
--   name,
--   isActive,
--   createdAt,
--   greenZoneVtPrice,
--   greenZoneNtPrice,
--   blueZoneVtPrice,
--   blueZoneNtPrice,
--   redZoneVtPrice,
--   redZoneNtPrice,
--   networkCostPerKw,
--   supplierCost,
--   approvedPowerKw
-- )
-- VALUES (
--   'Default model',
--   1,
--   GETUTCDATE(),
--   7.20,
--   1.80,
--   10.80,
--   2.70,
--   14.40,
--   3.60,
--   160.0,
--   620.0,
--   6.9
-- );
