# ricaun.AppBundleTool

The `.cmd` files is for install and uninstall the `ricaun.AppBundleTool` when developing.

## Create a tool

This project is a sample based in the [Tutorial: Create a .NET tool using the .NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/global-tools-how-to-create).

### Info Global Tool

```bash
dotnet tool list --global
```

or 

```bash
dotnet tool list -g
```

### Install Global Tool

```bash
dotnet tool install --global --add-source ../bin/Release ricaun.AppBundleTool --version *-*
```

or

```bash
dotnet tool install --global ricaun.AppBundleTool --version *-*
```

### Uninstall Global Tool
```bash
dotnet tool uninstall --global ricaun.AppBundleTool
```

## Execute Tool
```bash
AppBundleTool
```
or
```bash
AppBundleTool --help
```