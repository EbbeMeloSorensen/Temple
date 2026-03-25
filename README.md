# Temple

[TOC]

## Introduction

People management, Smurf management, Dungeons and Dragons adventure game.

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

## User Accessors

Notice how the API as well as the applications all make use of a UserAccessor implementing the IUserAccessor interface residing in the application layer. In this solution, the UserAccessor is just used in conjunction with letting a web application ask the API about the name of the logged in user in order to display it in the user interface. The implementations of the IUserAccessor interfaces in the WPF application and the Console application are just dummy objects that are never used but just there since the application layer requires a user accessor......

(I must admit I don't quite remember the rationale...)

## How to establish a local database

Use the local Postgres server on MELO_HOME

Use Docker on MELO_BASEMENT

Use podman on the Linux laptop

## How to run the API

In Visual Studio, just set Temple.API to the active project and press F5. Use Postman to verify that it works.

1. In VS Code, open a terminal window  and navigate to the Temple.API folder.
2. Optionally execute `dotnet build` to make sure that the API compiles.
3. Launch the API by executing `dotnet watch run`. Use Postman to verify that it works.

## How to run the WebClient locally

1) In VS Code, open a second terminal window (after having launched the API from one terminal window) and navigate to the Temple.WebClient folder.
2) Build the web client by executing `npm install`. Notice that there is a postbuild step in the package.json file that moves the contents of the build folder to a folder named "wwwroot" in the Temple.API folder. This is in order to bundle the web application with the API when deploying it to Heroku.
3) Launch the web client by executing `npm start`
4) VS Code should now open a web page in Chrome with this URL: http://localhost:3000

Notice that you may see an error related to GetCurrentUser. Try to just log out and in again, using `bob@test.com` and password `Super-long-very-secure-secret-key-that-is-at-least-64-bytes-in-length!!!!`

4. Verify that you can retrieve people, create a new person, use the filter etc.

## How to deploy to Heroku

As is the case with the migrations mentioned earlier, the solution already contains a production build of the client application residing in the folder "Temple.WebClient", specifically in the folder wwwroot under the folder Temple.API. It is, however, important to know how to make a production build, which is e.g. relevant after having modified the client application.

In order to make a new production build of the client application, follow these steps:

1) Optionally, delete the wwwroot folder under the Temple.API folder
2) Optionally, navigate to the Temple.WebClient folder, and execute: `npm run build`. Notice that the postbuild step in the package.json file will have to be adapted to the operating system that is being used when deploying. If it is Windows, then you should use the "move" command, and when it is Linux, you have to use the "mv" command instead.
3) Copy the contents of the Temple folder into a temporary folder such as C:\Temp\DeployTemple
4) In the temporary folder, delete the .git folder (to detach it from the git repo). Then open the solution in Visual Studio.
5) Remove these projects from the Temple solution
   * Temple.UI.WPF
   * Temple.UI.Console
   * Temple.POC.WPFApp
   * Temple.POC.ConsoleApp
   * Temple.POC.AvaloniaApp
   * Temple.Infrastructure.UnitTest
   * Temple.Application.UnitTest
   * Temple.Domain.UnitTest
6) Also delete the folders for these projects.
7) Make sure to save the Temple solution file to get totally rid of the mentioned projects
8) Open the Heroku site in a web browser such as Chrome, and log in. Notice that you have to use the Salesforce authentication app on your phone.
9) Click the purple "Create New App" button
10) Enter a name for the app such as temple
11) Select a location such as Europe
12) Click the purple "Create app" button
13) At this point, you may have to install the Heroku CLI (Command line Interface) unless you have already done that.
14) Navigate to the "Resources" tab
15) Click in the "Add-on Services" field, type postgres, and select "Heroku Postgres"
16) Select Essential 0 (Max 5$ a month), and click the purple "Submit Order Form" button. It then takes a short while for Heroku to provision the database.
17) Navigate to the "Settings" tab. Here, you should ***not*** click the "Add buildpack button", but notice how it says "Buildpacks will appear here"
18) Open a command prompt, and navigate to the temporary folder.
19) Execute `git init` to initialize an empty git repository in the folder.
20) Execute `git add *` and then `git commit -m` to populate the local git repository.
21) Execute `heroku login`. Then a browser window opens, where you can log in.
22) Execute `heroku git:remote -a temple` to attach the local git repository to the application, you just created on Heroku (temple is the name you chose for the application).
23) Execute `heroku buildpacks:set https://github.com/jincod/dotnetcore-buildpack` to configure a build pack for the application. Notice how it appears in the Heroku web page (you may need to refresh the page by pressing F5).
24) In Heroku, click the white "Reveal Config Vars" button. Notice how there already is a key named `DATABASE_URL`
25) Add a key named `ASPNETCORE_ENVIRONMENT` with the value `Production`
26) Add a key named `GITHUB_TOKEN` with the same value as your local environment variable with the same name. This is what facilitates retrieving your personal Nuget packages (Craft) from GitHub packages.
27) Add a key named `TokenKey` with a value that is a 64 characters long string generated with a password generator such as LastPass.
28) Back at the command prompt, execute `git push heroku master`. Notice, that sometimes it is apparently `main` instead of `master`.

Now you should see a build log ending with "Compressing", "Done", "Launching", "Released", and a URL

29. Copy/paste the URL into a page in a Web browser such as Chrome.

Now you should see the login page of your web application

Log in as `bob@test.com` with password `Super-long-very-secure-secret-key-that-is-at-least-64-bytes-in-length!!!!` Verify that the application works by creating a new person, editing one, deleting one, and using the filters.









