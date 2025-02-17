@ECHO OFF
dotnet tool uninstall --global ricaun.AppBundleTool
dotnet tool install --global --add-source ./bin/Release ricaun.AppBundleTool --version *-*
timeout 5