# VMworld 2020

This is the source code demonstrated at VMworld 2020. The contents of this repo is broken up into 3 branches. 
- base - A basic Web API application written in [.NET Framework 4.8](https://docs.microsoft.com/en-us/dotnet/framework/), [EF 6](https://docs.microsoft.com/en-us/ef/ef6/), and [SQL Server](https://www.microsoft.com/en-us/sql-server)
+ appmod-replatform - this branch transforms the base to enable for containerization with the help of [Steeltoe](https://steeltoe.io/) while leaving the technology stack targeting .NET 4.8 and Windows
+ appmod-modernization - this branch takes the `appmod-replatform` branch and applies bare minimum code changes to modernize the app to target .NET Core 3.1 and Linux.

As an added bonus, and for demonstration purposes, the demo targets [Tanzu TAS](https://tanzu.vmware.com/application-service) platform for the modernization deployment and the [TAS for Windows](https://tanzu.vmware.com/components/tas-for-windows) for the transformation deployment. See the manifest.yaml files for more details located under their perspective project folder.

[Watch the VMworld 2020 session On-Demand](https://my.vmworld.com/widget/vmware/vmworld2020/catalog/session/1589558806996001JxQf)

## EF & Database

### Prerequisite
- Create a project and a code first EF model (i.e. DbContext and POCOs)

### EF migration main commands
- Run `Enable-Migrations` - will create a migrations folder containing:
    + A configuration.cs class
    + An initial migration cs class
- Run `Add-Migration` to scaffold the next migration based on the changes
- Run `Update-Database` to apply any pending migrations to the database

