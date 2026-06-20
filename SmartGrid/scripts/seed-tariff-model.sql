-- Tarifni model: AZURE SQL baza SmartGrid, tabela TariffModels
-- Pokreni ceo fajl jednom (SSMS, Azure Data Studio ili sqlcmd).

USE SmartGrid;
GO

-- 1) Dodaj kolone za pragove zona (bezbedno za ponovno pokretanje)
IF COL_LENGTH('TariffModels', 'greenZoneMaxKwh') IS NULL
BEGIN
    ALTER TABLE TariffModels
    ADD greenZoneMaxKwh FLOAT NOT NULL
        CONSTRAINT DF_TariffModels_greenZoneMaxKwh DEFAULT 350;
END;
GO

IF COL_LENGTH('TariffModels', 'blueZoneMaxKwh') IS NULL
BEGIN
    ALTER TABLE TariffModels
    ADD blueZoneMaxKwh FLOAT NOT NULL
        CONSTRAINT DF_TariffModels_blueZoneMaxKwh DEFAULT 1200;
END;
GO

IF COL_LENGTH('TariffModels', 'updatedAt') IS NULL
BEGIN
    ALTER TABLE TariffModels
    ADD updatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_TariffModels_updatedAt DEFAULT GETUTCDATE();
END;
GO

-- 2) Popuni pragove za postojece redove ako su prazni
UPDATE TariffModels
SET greenZoneMaxKwh = 350
WHERE greenZoneMaxKwh IS NULL OR greenZoneMaxKwh <= 0;
GO

UPDATE TariffModels
SET blueZoneMaxKwh = 1200
WHERE blueZoneMaxKwh IS NULL OR blueZoneMaxKwh <= greenZoneMaxKwh;
GO

UPDATE TariffModels
SET updatedAt = COALESCE(updatedAt, createdAt, GETUTCDATE())
WHERE updatedAt IS NULL OR updatedAt = '0001-01-01';
GO

-- 3) Seed aktivnog tarifnog modela ako ne postoji
IF NOT EXISTS (SELECT 1 FROM TariffModels WHERE isActive = 1)
BEGIN
    INSERT INTO TariffModels (
        name, isActive, createdAt, updatedAt,
        greenZoneVtPrice, greenZoneNtPrice,
        blueZoneVtPrice, blueZoneNtPrice,
        redZoneVtPrice, redZoneNtPrice,
        networkCostPerKw, supplierCost, approvedPowerKw,
        greenZoneMaxKwh, blueZoneMaxKwh
    )
    VALUES (
        N'Standardni tarifni model', 1, GETUTCDATE(), GETUTCDATE(),
        8.5, 4.2,
        11.0, 6.5,
        14.5, 9.0,
        350, 280, 6.9,
        350, 1200
    );
END;
GO
