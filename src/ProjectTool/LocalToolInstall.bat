@echo off
dotnet pack -c Shipping
dotnet tool uninstall -g Brewery.ProjectTool
dotnet tool install --global --add-source .\bin\Shipping Brewery.ProjectTool
pause