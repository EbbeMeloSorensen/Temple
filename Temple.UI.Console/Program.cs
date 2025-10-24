using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MediatR;
using CommandLine;
using Craft.Domain;
using Craft.Utils;
using Temple.Application.Core;
using Temple.Application.Interfaces;
using Temple.Domain.BusinessRules.PR;
using Temple.Infrastructure.Pagination;
using Temple.Persistence;
using Temple.Persistence.EFCore.AppData;
using Temple.UI.Console.Verbs;

namespace Temple.UI.Console
{
    internal class Program
    {
        private static IHost? _host = null;

        public static async Task CreatePerson(
            Verbs.PR.Create options)
        {
            System.Console.Write("Creating Person...\nProgress: ");

            options.StartTime.TryParsingAsDateTime(out var startTime);
            options.EndTime.TryParsingAsDateTime(out var endTime);

            var person = new Domain.Entities.PR.Person()
            {
                FirstName = options.FirstName,
                Start = startTime ?? DateTime.UtcNow.Date,
                End = endTime ?? new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc)
            };

            try
            {
                using var scope = (await GetHost()).Services.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                var result = await mediator.Send(new Application.People.Create.Command {Person = person});

                System.Console.Write(result.IsSuccess
                    ? $"\nPerson: \"{person.FirstName}\" created successfully\n"
                    : $"\nPerson: \"{person.FirstName}\" creation failed ({result.Error})\n");
            }
            catch (Exception exception)
            {
                System.Console.Write($"\nException thrown: \"{exception.Message}\"\n");
            }


            // Old - where we targeted a different application layer
            //    var application = GetApplication();
            //    var businessRuleViolations = application.CreateNewPerson_ValidateInput(person);

            //    if (businessRuleViolations.Any())
            //    {
            //        System.Console.WriteLine("\nErrors:");

            //        foreach (var kvp in businessRuleViolations)
            //        {
            //            System.Console.WriteLine($"  {kvp.Value}");
            //        }

            //        return;
            //    }

            //    await application.CreateNewPerson(person, (progress, nameOfSubtask) =>
            //    {
            //        System.Console.SetCursorPosition(10, System.Console.CursorTop);
            //        System.Console.Write($"{progress:F2} %");
            //        return false;
            //    });

            //    System.Console.WriteLine("\nDone");
        }

        public static async Task ListPeople(
            Verbs.PR.List options)
        {
            options.HistoricalTime.TryParsingAsDateTime(out var historicalTime);
            options.DatabaseTime.TryParsingAsDateTime(out var databaseTime);

            try
            {
                using var scope = (await GetHost()).Services.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                //var result = await mediator.Send(new Application.Smurfs.List.Query { Params = new Application.Smurfs.SmurfParams() });
                var result = await mediator.Send(new Application.People.List.Query { Params = new Application.People.PersonParams() });

                System.Console.WriteLine($"\nPeople:");
                foreach (var personDto in result.Value)
                {
                    System.Console.WriteLine($" {personDto.FirstName}");
                }
            }
            catch (HttpRequestException exception)
            {
                System.Console.Write($"\nHttpRequestException thrown: \"{exception.Message}\"\n");
            }
            catch (Exception exception)
            {
                System.Console.Write($"\nException thrown: \"{exception.Message}\"\n");
            }
        }

        public static async Task ListSmurfs(
            Verbs.Smurfs.List options)
        {
            try
            {
                using var scope = (await GetHost()).Services.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                var result = await mediator.Send(new Application.Smurfs.List.Query { Params = new Application.Smurfs.SmurfParams() });

                System.Console.WriteLine($"\nSmurfs:");
                foreach (var smurfDto in result.Value)
                {
                    System.Console.WriteLine($" {smurfDto.Name}");
                }
            }
            catch (HttpRequestException exception)
            {
                System.Console.Write($"\nHttpRequestException thrown: \"{exception.Message}\"\n");
            }
            catch (Exception exception)
            {
                System.Console.Write($"\nException thrown: \"{exception.Message}\"\n");
            }
        }

        public static async Task GetPersonDetails(
            Details options)
        {
            System.Console.Write("Getting person details...\nProgress: ");

            var id = new Guid(options.ID);
            options.DatabaseTime.TryParsingAsDateTime(out var databaseTime);

            try
            {
                //await GetApplication().GetPersonDetails(
                //    id, 
                //    databaseTime,
                //    options.WriteToFile,
                //    (progress, nameOfSubtask) =>
                //{
                //    System.Console.SetCursorPosition(10, System.Console.CursorTop);
                //    System.Console.Write($"{progress:F2} %");
                //    return false;
                //});

                //System.Console.WriteLine("\nDone");
            }
            catch (Exception e)
            {
                System.Console.WriteLine($"\nError getting person details: {e.Message}");
            }
        }

        public static async Task CountPeople(
            Count options)
        {
            System.Console.WriteLine("Coming soon: CountPeople");
            await Task.Delay(200);
        }

        public static async Task ExportPeople(
            Export options)
        {
            //System.Console.Write("Exporting data...\nProgress: ");
            //var dateTime = DateTime.Now;
            //await GetApplication().ExportData(options.FileName, (progress, nameOfSubtask) =>
            //{
            //    System.Console.SetCursorPosition(10, System.Console.CursorTop);
            //    System.Console.Write($"{progress:F2} %");
            //    return false;
            //});
            //System.Console.WriteLine("\nDone");
        }

        public static async Task ImportPeople(
            Import options)
        {
            //System.Console.Write("Importing data...\nProgress: ");
            //await GetApplication().ImportData(
            //    options.FileName, (progress, nameOfSubtask) =>
            //{
            //    System.Console.SetCursorPosition(10, System.Console.CursorTop);
            //    System.Console.Write($"{progress:F2} %");
            //    return false;
            //});
            //System.Console.WriteLine("\nDone");
        }

        public static async Task UpdatePerson(
            Update options)
        {
            //    System.Console.Write("Updating Person...\nProgress: ");

            //    var person = new Person()
            //    {
            //        ID = new Guid(options.ID),
            //        FirstName = options.FirstName
            //    };

            //    var application = GetApplication();
            //    var businessRuleViolations = application.UpdatePerson_ValidateInput(person);

            //    if (businessRuleViolations.Any())
            //    {
            //        System.Console.WriteLine("\nErrors:");

            //        foreach (var kvp in businessRuleViolations)
            //        {
            //            System.Console.WriteLine($"  {kvp.Value}");
            //        }

            //        return;
            //    }

            //    try
            //    {
            //        await application.UpdatePerson(person, (progress, nameOfSubtask) =>
            //        {
            //            System.Console.SetCursorPosition(10, System.Console.CursorTop);
            //            System.Console.Write($"{progress:F2} %");
            //            return false;
            //        });

            //        System.Console.WriteLine("\nDone");
            //    }
            //    catch (Exception e)
            //    {
            //        System.Console.WriteLine($"\nError updating person ({e.Message})");
            //    }
        }

        public static async Task DeletePerson(
            Delete options)
        {
            //System.Console.Write("Deleting...\nProgress: ");

            //var id = new Guid(options.ID);

            //await GetApplication().DeletePerson(id, (progress, nameOfSubtask) =>
            //{
            //    System.Console.SetCursorPosition(10, System.Console.CursorTop);
            //    System.Console.Write($"{progress:F2} %");
            //    return false;
            //});
            //System.Console.WriteLine("\nDone");
        }

        public static async Task MakeBreakfast(
            Breakfast options)
        {
            //System.Console.Write("Making breakfast...\nProgress: ");
            //await GetApplication().MakeBreakfast((progress, nameOfSubtask) =>
            //{
            //    System.Console.SetCursorPosition(10, System.Console.CursorTop);
            //    System.Console.Write($"{progress:F2} %");
            //    return false;
            //});
            //System.Console.WriteLine("\nDone");
        }

        static async Task Main(
            string[] args)
        {
            //args = "breakfast".Split();
            //args = "create --host localhost --user postgres --password L1on8Zebra --firstname Egon".Split();
            //args = "count --user john --password secret".Split();
            //args = "list --host localhost --user postgres --password L1on8Zebra".Split();
            //args = "import --host localhost --user postgres --password L1on8Zebra".Split();
            //args = "export --host localhost --user postgres --password L1on8Zebra".Split();
            //args = "update --user john --password secret --id 67".Split();
            //args = "delete --user john --password secret --id 67".Split();
            //args = "list -h localhost -d PR -u postgres -p L1on8Zebra".Split();
            //args = "import --filename Contacts.json --legacy true --host localhost --user postgres --password L1on8Zebra".Split();

            await Parser.Default.ParseArguments<
                    Verbs.PR.Create,
                    Count,
                    Verbs.PR.List,
                    Verbs.Smurfs.List,
                    Details,
                    Export,
                    Import,
                    Update,
                    Delete,
                    Breakfast>(args)
                .MapResult(
                    (Verbs.PR.Create options) => CreatePerson(options),
                    (Count options) => CountPeople(options),
                    (Verbs.PR.List options) => ListPeople(options),
                    (Verbs.Smurfs.List options) => ListSmurfs(options),
                    (Details options) => GetPersonDetails(options),
                    (Export options) => ExportPeople(options),
                    (Import options) => ImportPeople(options),
                    (Update options) => UpdatePerson(options),
                    (Delete options) => DeletePerson(options),
                    (Breakfast options) => MakeBreakfast(options),
                    errs => Task.FromResult(0));
        }

        private static async Task<IHost> GetHost()
        {
            if (_host == null)
            {
                _host = Host.CreateDefaultBuilder()
                    .ConfigureLogging(logging =>
                    {
                        // Turn off logging entirely
                        //logging.ClearProviders();

                        // Filter EF Core logs
                        logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.None);
                        logging.AddFilter("Microsoft.EntityFrameworkCore.Migrations", LogLevel.None);
                    })
                    .ConfigureServices((context, services) =>
                    {
                        // Load connection string from appsettings.json or environment
                        var connectionString = context.Configuration.GetConnectionString("DefaultConnection");
                        //connectionString = "Data source=babuska3.db";

                        // Postgres - MELO - Basement
                        connectionString = "Server=localhost;Port=5432;User Id=root;Password=root;Database=DB_Temple_UI_Console";

                        // Postgres - Linux in podman
                        //connectionString = "Server=localhost;Port=5432;User Id=myuser;Password=mypassword;Database=DB_Temple_UI_Console";

                        services.AddAppDataPersistence<PRDbContextBase>(options =>
                        {
                            //options.UseSqlite(connectionString);
                            options.UseNpgsql(connectionString);
                        });

                        services.AddApplication(); // registers MediatR and handlers
                        services.AddAutoMapper(assemblies: typeof(MappingProfiles).Assembly);
                        services.AddScoped<IUserAccessor, UserAccessor>();
                        services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();
                        services.AddScoped<IBusinessRuleCatalog, BusinessRuleCatalog>();
                        services.AddScoped<IPagingHandler<Application.Smurfs.SmurfDto>, PagingHandler<Application.Smurfs.SmurfDto>>();
                        services.AddScoped<IPagingHandler<Application.People.PersonDto>, PagingHandler<Application.People.PersonDto>>();
                    })
                    .Build();

                // Run migrations once when the host is created
                using var scope = _host.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<PRDbContextBase>();
                await db.Database.MigrateAsync();
                await Seeding.SeedDatabase(db);
            }

            return _host;
        }
    }
}