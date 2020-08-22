# vmworld-2020


## EF & Database

### Prerequisite
- Create a project and a code first EF model (i.e. DbContext and POCOs)

### EF migration main commands
- Run `Enable-Migrations` - will create a migrations folder containing:
    + A configuration.cs class - used for seed data
    + An initial migration cs class
- Run `Add-Migration` to scaffold the next migration based on the changes
- Run `Update-Database` to apply any pending migrations to the database


## EF Coref & Database

dotnet tool install --global dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet ef migrations add InitialCreate

Note: Seed data using the OnModelCreating or custom initialization logic


## Deployment

On a Cloud Foundry Platform 

Pipeline variables
- $target_env - the target environment (development, qa, staging, production)
- $target_dbstring - connection string to the database

Deploy command 

```powershell 
cf push -f manifest.yaml -p bin\app.publish  --vars ASPNETCORE_ENVIRONMENT=$target_env --vars dbstring=$target_dbstring 
```
