@ECHO OFF
dotnet tool uninstall --global ricaun.AppBundleTool
dotnet tool install --global --add-source ../bin/Release ricaun.AppBundleTool --version *-*
AppBundleTool -l
AppBundleTool -a https://github.com/ricaun-io/RevitAddin.CommandLoader/releases/latest/download/RevitAddin.CommandLoader.bundle.zip -i
timeout 5