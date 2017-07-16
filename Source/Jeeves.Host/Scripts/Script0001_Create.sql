CREATE TABLE Configuration
(
  ID            INTEGER     PRIMARY KEY,
  UserName      TEXT        COLLATE NOCASE NOT NULL,
  Application   TEXT        COLLATE NOCASE NOT NULL,
  Key           TEXT        COLLATE NOCASE NOT NULL,
  Value         TEXT        NOT NULL
);

CREATE TABLE User
(
  ID            INTEGER     PRIMARY KEY,
  Apikey        TEXT        UNIQUE,
  UserName      TEXT        COLLATE NOCASE NOT NULL,
  Application   TEXT        COLLATE NOCASE NOT NULL,
  CanWrite      BOOLEAN     NOT NULL
);
