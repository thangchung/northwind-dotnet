@echo off

IF "%1"=="s" dotnet ef migrations add InitialProductCatalogDb -c MainDbContext -o Data/Migrations & goto exit
IF "%1"=="c" dotnet ef dbcontext optimize -c MainDbContext -o Data/CompiledModels -n ProductCatalog --verbose & goto exit

:exit
