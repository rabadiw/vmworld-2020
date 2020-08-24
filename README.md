# vmworld-2020


## EF & Database

### Prerequisite
- Create a project and a code first EF model (i.e. DbContext and POCOs)

### EF migration main commands
- Run `Enable-Migrations` - will create a migrations folder containing:
    + A configuration.cs class
    + An initial migration cs class
- Run `Add-Migration` to scaffold the next migration based on the changes
- Run `Update-Database` to apply any pending migrations to the database

