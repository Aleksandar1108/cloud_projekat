
CREATE TABLE Users (
  idUsers INT NOT NULL,
  email VARCHAR(45) NOT NULL,
  password VARCHAR(255) NOT NULL,
  role VARCHAR(45) NOT NULL,
  accountCreated DATETIME NOT NULL,
  PRIMARY KEY (idUsers));
