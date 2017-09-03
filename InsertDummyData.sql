INSERT INTO User (Apikey, UserName, Application, CanWrite) VALUES('guykey', 'guy', 'app01', 1);

INSERT INTO Configuration (UserName, Application, Key, Value) VALUES('', 'app01', 'config', '{ "value" : "foo" }');
INSERT INTO Configuration (UserName, Application, Key, Value) VALUES('guy', 'app01', 'config', '{ "value" : "bar" }');
