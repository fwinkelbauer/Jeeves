# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/) and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [Unreleased]

- An instance of `JeevesHost` is now created using a `JeevesHostBuilder`. The builder can be used to add logging and user authentification
- Renamed the `PutValue(...)` method in `IDataStore` to `StoreValue(...)`
- Moved the `RetrieveUser(...)` method of `IDataStore` into `IUserAuthenticator`
- Renamed the `Debug(...)` method in `IJeevesLog` to `Information(...)`
- Authentication can now only be used with HTTPS

## [0.1.0] - 2017-09-03

### Added

- Initial release
