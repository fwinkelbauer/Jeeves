# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/) and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [Unreleased]

- **Breaking Change:** To instantiate an object of `JeevesSettings` a base URL as well as a security option (HTTP - meaning no security, HTTPS, HTTPS with user authentication) is needed. Prior to this change `Jeeves.Core` could be configured to use user authentication without HTTP which would create a false sense of security
- **Breaking Change:** Renamed the `PutValue(...)` method in `IDataStore` to `StoreValue(...)`
- **Breaking Change:** Renamed the `Debug(...)` method in `IJeevesLog` to `Information(...)`

## [0.1.0] - 2017-09-03

### Added

- Initial release
