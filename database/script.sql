-- Local development setup:
-- Start SQL Server container: docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=SmartGrid123!" -p 1433:1433 --name smartgrid-sql -d mcr.microsoft.com/mssql/server:2022-latest
-- The SA password in appsettings.json must match: "Password": "SmartGrid123!"
-- If you use a different password, update appsettings.json accordingly.

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
  
  CONSTRAINT FK_EmailActivation_Users
  FOREIGN KEY (idUsers)
  REFERENCES Users(idUsers)
  ON DELETE CASCADE
  );
  
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

-- Razmotritii dodavanje kaskadnog brisanja (pogotovo zbog user-a)
CREATE TABLE Properties (
  Id UNIQUEIDENTIFIER NOT NULL,
  UserId UNIQUEIDENTIFIER NOT NULL,
  Name NVARCHAR(100) NOT NULL,
  City NVARCHAR(100) NOT NULL,
  Address NVARCHAR(255) NOT NULL,
  Description NVARCHAR(500) NULL,
  PropertyType NVARCHAR(20) NOT NULL,
  CreatedAt DATETIME NOT NULL,
  PRIMARY KEY (Id),
  FOREIGN KEY (UserId) REFERENCES Users(idUsers)
);

CREATE TABLE SmartMeters (
  Id UNIQUEIDENTIFIER NOT NULL,
  PropertyId UNIQUEIDENTIFIER NOT NULL,
  Label NVARCHAR(100) NOT NULL,
  ConnectionType NVARCHAR(20) NOT NULL,
  MaxApprovedPower FLOAT NOT NULL,
  Note NVARCHAR(500) NULL,
  SerialNumber NVARCHAR(20) NULL,
  PairingStatus NVARCHAR(20) NOT NULL DEFAULT 'Unpaired',
  DeviceUUID NVARCHAR(50) NULL,
  AccessToken NVARCHAR(255) NULL,
  CreatedAt DATETIME NOT NULL,
  PRIMARY KEY (Id),
  FOREIGN KEY (PropertyId) REFERENCES Properties(Id)
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
