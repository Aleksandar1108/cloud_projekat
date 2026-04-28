CREATE TABLE Users (
  idUsers UNIQUEIDENTIFIER NOT NULL,
  email VARCHAR(45) NOT NULL,
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

-- ManualReadings are stored in Azure Table (Azurite: ManualReadings table),
-- while raw/optimized images are stored in Azure Blob (manual-readings container).

-- Monthly bill metadata is stored in Azure Table (Azurite: MonthlyBills table),
-- while full bill content is stored in Azure Blob (monthly-bills container).

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
