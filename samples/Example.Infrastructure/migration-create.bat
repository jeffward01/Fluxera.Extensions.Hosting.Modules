set "NewMigrationName=ABCD"
set /P NewMigrationName="Please enter the name of the migration: "
dotnet ef migrations add %NewMigrationName%
