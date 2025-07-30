@ECHO OFF
dotnet tool uninstall --global ricaun.AppBundleTool
dotnet tool install --global --add-source ../bin/Release ricaun.AppBundleTool --version *-*
AppBundleTool -l
AppBundleTool -a RevitAddin.CommandLoader -u
AppBundleTool -a https://github.com/ricaun-io/RevitAddin.CommandLoader/releases/download/1.1.0/RevitAddin.CommandLoader.bundle.zip -i
AppBundleTool -a https://github.com/ricaun-io/RevitAddin.CommandLoader/releases/latest/download/RevitAddin.CommandLoader.bundle.zip -i
AppBundleTool -a RevitAddin.CommandLoader.bundle -u
AppBundleTool -a RevitAddin.CommandLoader
AppBundleTool -a https://github.com/ricaun-io/RevitAddin.CommandLoader/releases/latest/download/RevitAddin.CommandLoader.bundle.zip -i
timeout 10