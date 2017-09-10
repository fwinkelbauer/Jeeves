INSERT INTO User (Apikey, UserName, Application, CanWrite) VALUES('guykey', 'guy', 'app01', 1);

INSERT INTO Configuration (UserName, Application, Key, Value, Created) VALUES('', 'app01', 'config', '{ "value" : "foo" }', DateTime('now'));
INSERT INTO Configuration (UserName, Application, Key, Value, Created) VALUES('guy', 'app01', 'config', '{ "value" : "bar" }', DateTime('now'));
