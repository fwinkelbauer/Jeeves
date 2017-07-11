CREATE TABLE IF NOT EXISTS Configuration
(
  ID            INTEGER     PRIMARY KEY,
  Application   TEXT        COLLATE NOCASE NOT NULL,
  Host          TEXT        COLLATE NOCASE NOT NULL,
  Revision      INTEGER     NOT NULL,
  Key           TEXT        COLLATE NOCASE NOT NULL,
  Value         TEXT        NOT NULL
);

CREATE TABLE IF NOT EXISTS User
(
  ID            INTEGER     PRIMARY KEY,
  Apikey        TEXT        UNIQUE,
  UserName      TEXT        COLLATE NOCASE NOT NULL,
  Application   TEXT        COLLATE NOCASE NOT NULL,
  CanWrite      BOOLEAN     NOT NULL,
  IsAdmin       BOOLEAN     NOT NULL
);
