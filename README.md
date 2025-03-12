# ricaun.AppBundleTool

[![Visual Studio 2022](https://img.shields.io/badge/Visual%20Studio-2022-blue)](https://github.com/ricaun-io/ricaun.AppBundleTool)
[![Nuke](https://img.shields.io/badge/Nuke-Build-blue)](https://nuke.build/)
[![License MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![Build](https://github.com/ricaun-io/ricaun.AppBundleTool/actions/workflows/Build.yml/badge.svg)](https://github.com/ricaun-io/ricaun.AppBundleTool/actions)
[![Release](https://img.shields.io/nuget/v/ricaun.AppBundleTool?logo=nuget&label=release&color=blue)](https://www.nuget.org/packages/ricaun.AppBundleTool)

Command line tool to Install/Uninstall Autodesk AppBundle.

## Install Tool

```bash
dotnet tool install --global ricaun.AppBundleTool
```

## Tool Commands

The `AppBundleTool` command line tool provides the following commands:

### List

Command `-l` and `--lint` to list all installed bundle.

```bash
AppBundleTool -l
```

### AppBundle information

Command `-a` and `--app` to show information about a bundle.
```bash
AppBundleTool -a <bundle-path.zip or bundle-name>
```

### Install

Command `-i` and `--install` to install a tool.
```bash
AppBundleTool -a <bundle-path.zip> -i
```

### Uninstall

Command `-u` and `--uninstall` to uninstall a tool.
```bash
AppBundleTool -a <bundle-path.zip or bundle-name> -u
```

## Example

The following example shows how to install a bundle from a URL in the [RevitAddin.CommandLoader](https://github.com/ricaun-io/RevitAddin.CommandLoader) releases.

### Latest

```bash
AppBundleTool -a https://github.com/ricaun-io/RevitAddin.CommandLoader/releases/latest/download/RevitAddin.CommandLoader.bundle.zip -i
```

### Specific version

```bash
AppBundleTool -a https://github.com/ricaun-io/RevitAddin.CommandLoader/releases/download/1.1.0/RevitAddin.CommandLoader.bundle.zip -i
```

## Autodesk AppBundle

The AppBundle is a standardized format for Autodesk applications. This package format enables the application or addin to be loaded in many different Autodesk products and versions.

### References

* [AppBundle: Autodesk Products - 2020](https://www.autodesk.com/autodesk-university/class/AppBundle-Cross-Distribution-Autodesk-Products-App-Store-and-Forge-2020)
* [AppBundle Structure: Revit - 2018](https://github.com/jeremytammik/tbc/blob/gh-pages/a/zip/da4r_au_2018/AppBundle.md)
* [Autodesk Autoloader White Paper - 2013](https://adndevblog.typepad.com/autocad/2013/01/autodesk-autoloader-white-paper.html)

## License

This project is [licensed](LICENSE) under the [MIT License](https://en.wikipedia.org/wiki/MIT_License).

---

Do you like this project? Please [star this project on GitHub](https://github.com/ricaun-io/ricaun.AppBundleTool/stargazers)!