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