# Temple

## Layers

This is a solution made in compliance with the "Clean Architecture" architectural pattern. The solution consists of the following layers:

### Domain Layer

Specifically, the project **Temple.Domain**.

The innermost layer that just has domain logic and no dependencies.

### Persistence Layer

Specifically, the project **Temple.Persistence**.

This is strictly not a part of the Clean Architecture model, but is often used in conjunction with it, as it is here. It provides a general interface for the Application layer for managing domain objects defined in the domain layer. This interface is persistent ignorant, meaning that it relies on dependency injection of a plugin that represents an arbitrary persistence strategy. For now, only persistence strategies based on Entity Framework Core are supplied.

### Application Layer

Specifically, the project **Temple.Application**.

A layer concerned with use cases. This layer is independent on any presentation frameworks and can act as the basis for a wide range of higher order layers such as APIs, console applications or presentation frameworks such as WPF or Avalonia. Currently, the solution includes the following different higher order layers:

* An API (specifically, the project **Temple.API**)
* A console application (specifically, the project **Temple.UI.Console**)
* A WPF application (specifically, the project **Temple.UI.WPF**)
* An Avalonia application (specifically, the project **Temple.POC.AvaloniaApp**)

The application layer is designed in accordance with the CQRS (Command Query Responsibility Segregation) design pattern. Notice that there is no Application object of any kind.

## Data models

The Temple solution includes the following 3 projects that contain data models in the form of specializations of DbContext:

### Temple.Persistence.EFCore.Dummies

Contains the class DataContext2 that represents a simple data model just constisting of one entity (Smurf) and no relations. This project is intended to serve as a simple demonstration of the persistence design principle and may act as a template for extending the solution with new data models.

### Temple.Persistence.EFCore.AppData

Contains the class PRDbContextBase that represents a more realistic data model with a fairly large number of entities and relations. Notably, the data model doesn't include any entities for managing users and passwords which are required for the API. Users and passwords are handled by the project Temple.Persistence.EFCore.Identity.

### Temple.Persistence.EFCore.Identity

Contains the class DataContext, that inherits from IdentityDbContext\<AppUser\>. The DbContext represents a data model concerned with managing users and passwords, which is utilized in the API project.

## Migrations

The projects with names starting with Temple.Persistence all contain migrations for PostgreSQL, so the solution has what it needs for generating a Postgres database used for persistence during startup of an API or application. During development, it may be useful to operate with e.g. an SQLite database, and in that case the migrations have to be replaced with migrations based on SQLite. In every case, it is relevant to know how to generate new migrations, e.g. after having extended or generally changed a data model.

The technique for generating migrations depend on the IDE used.

### Generating migrations by means of Visual Studio

1) Delete the existing migrations, specifically the folders named "Migrations" in the projects with names starting with Temple.Persistence.EFCore.

(Notice that it is good practice to maintain an entire stack of migrations that build upon each other when maintaining a solution, e.g. to facilitate 	reversion to an earlier version in case of trouble. However, for this project, I usually just generate entirely new migrations replacing any existing ones)

1) Launch Visual Studio and open the "Package Manager Console" window. Make sure you are in the solution folder and execute the following:

```
dotnet ef migrations add InitialMigration -p Temple.Persistenc.EFCore.Dummies -s Temple.API --context DataContext2
dotnet ef migrations add InitialMigration -p Temple.Persistence.EFCore.AppData -s Temple.API --context PRDbContextBase
dotnet ef migrations add InitialMigration -p Temple.Persistence.EFCore.Identity -s Temple.API --context DataContext
```

Notice that you don't have to select anything in the "Default project" dropdown box. That was necessary for older solutions but not this one.

Here, "InitialMigration" is the name of the migration you want to generate. "Temple.Persistence.EFCore.Dummies" (after -p) is the name of the project you want to make the migration for, and Temple.API (after -s) is the name of the project that holds statements like this: `options.UseNpsql(connectionString)`. For the Temple.API project these statements specifically reside in AddApplicationServices method of the ApplicationServiceExtensions class. Notice how that method contains multiple calls to AddIdentityPersistence, specifically one for each of the data model files that are in use for the API project. The other higher order layers depending on the application layer have corresponding calls, such as the project Temple.UI.WPF that calls AddAppDataPersistence in the file App.xaml.cs. In a similar way, the method GetHost in the Program class of the Temple.UI.Console project calls GetRequiredService. Notice that the WPF application and the console application only use the migration of the project Temple.Persistence.EFCore.AppData class. This is because they don't use authentication, and they don't support management of smurfs.

As such, you might use these projects instead of Temple.API when generating migrations with the Console Package Manage, but practically it make sense to use Temple.API since it contains all the datamodels.

Also notice how the multiple calls to AddApplicationServices use the same connection string. This results in the migrations being applied to *the same* database, which is usually suitable.

# User Accesors

Notice how the API as well as the applications all make use of a UserAccessor implementing the IUserAccessor interface residing in the application layer. In this solution, the UserAccessor is just used in conjunction with letting a web application ask the API about the name of the logged in user in order to display it in the user interface. The implementations of the IUserAccessor interfaces in the WPF application and the Console application are just dummy objects that are never used but just there since the application layer requires a user accessor......

(I must admit I don't quite remember the rationale...)



