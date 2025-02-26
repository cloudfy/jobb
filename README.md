# Jobb
Jobb enables export of SQL Schemas and data from Microsoft SQL Server or Azure SQL server.

Jobb is both provided as a command-line client (CLI) or a Visual Studio file generator supported from Visual Studio 17+. Upon generation, a .sql file is generated from the options provided in the jobb file.

## Jobb CLI
[![NuGet version (Jobb.Cli)](https://img.shields.io/nuget/v/Jobb.Cli.svg?style=plastic)](https://www.nuget.org/packages/Jobb.Cli/)

Command line to generate export.

### Install as a global tool
```
dotnet tool install --global Jobb.Cli --version 1.0.8
dotnet tool install --global Jobb.Cli --version 1.0.8 --add-source ./Jobb.Cli.1.0.8.nupkg

```

### Update global tool after install
```
dotnet tool update -g jobb
```

## File format: .jobb files
Jobb files are text based files with a .jobb extension. Essentially a jobb file is a JSON file providing settings on how the SQL schema export should be generated.

**Example**
``` json
{
    "ConnectionString" : "",
    "ExportSettings" : {
    }
}
```