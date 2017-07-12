INSERT INTO User (Apikey, UserName, Application, CanWrite, IsAdmin) VALUES('guykey', 'guy', 'app01', 1, 0);

INSERT INTO Configuration (Application, UserName, Key, Value) VALUES('app01', '', 'config', '{ "value": "foo" }');
INSERT INTO Configuration (Application, UserName, Key, Value) VALUES('app01', 'guy', 'config', '{ "value": "bar" }');
