# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [1.0.2] / 2025-07-30
### Updates
- Update `CreateDataColumns` to support more values in the `DataTable`.
- Add `ConsoleTaskExtension` to wait task to finish and show progress bar.
- Update `AppBundleUtils` with `FindAppBundle` to find `AppBundle` by app name or bundle folder.
- Update `CACHE_TOTAL_MINUTES` in `DownloadUtils` to 1 second.

## [1.0.1] / 2025-05-31
### Features
- Support `Autodesk` native bundle with admin privileges.
- Show list of bundles in table format using `DataTable`.
- Support to find app by `BundleName` or `AppName`.
- Show `AppBundle` information in table format.
- Support Console color output in table format in `Admin` as red color.
- Support `ProgramFilesX86` folder for `AutoCAD` bundles.
### Updates
- Add `AppBundleAccess` to show bundle access information (`Allow` or `Admin`)
- Update uninstall to use `UninstallAppBundle` with `RecycleBinUtils`.
- Add `RecycleBinUtils` with methods to recycle files and folders.
- Add `AppNameSpace` and `AppUpgradeCode` in the `ApplicationPackage` class.
### Fixes
- Fix `PackageContents.xml` requires admin privileges to read. (Fix: #1)

## [1.0.0] / 2025-02-17
### Features
- Install/Uninstall bundle.
### Updates
- Show information about bundle.
- Add `PackageContents` to parse Autodesk `PackageContents.xml` file.
- Add `DownloadUtils` to download zip files of bundle.
- Add `CopyFilesRecursively` to copy files from one directory to another.
- Update `DownloadUtils` to support local file.

[vNext]: ../../compare/1.0.0...HEAD
[1.0.2]: ../../compare/1.0.1...1.0.2
[1.0.1]: ../../compare/1.0.0...1.0.1
[1.0.0]: ../../compare/1.0.0