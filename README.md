# ricaun.AppBundleTool

[![Visual Studio 2022](https://img.shields.io/badge/Visual%20Studio-2022-blue)](https://github.com/ricaun-io/ricaun.AppBundleTool)
[![Nuke](https://img.shields.io/badge/Nuke-Build-blue)](https://nuke.build/)
[![License MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![Build](https://github.com/ricaun-io/ricaun.AppBundleTool/actions/workflows/Build.yml/badge.svg)](https://github.com/ricaun-io/ricaun.AppBundleTool/actions)
[![Release](https://img.shields.io/nuget/v/ricaun.AppBundleTool?logo=nuget&label=release&color=blue)](https://www.nuget.org/packages/ricaun.AppBundleTool)

Install/Uninstall Autodesk AppBundle command line tool.

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

## License

This project is [licensed](LICENSE) under the [MIT License](https://en.wikipedia.org/wiki/MIT_License).

---

Do you like this project? Please [star this project on GitHub](https://github.com/ricaun-io/ricaun.AppBundleTool/stargazers)!