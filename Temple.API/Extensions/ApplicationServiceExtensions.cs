using Craft.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Temple.Application.Core;
using Temple.Application.Interfaces;
using Temple.Application.People;
using Temple.Application.Smurfs;
using Temple.Domain.BusinessRules.PR;
using Temple.Infrastructure.Pagination;
using Temple.Infrastructure.Security;
using Temple.Persistence;
using Temple.Persistence.EFCore.AppData;
using Temple.Persistence.EFCore.Dummies;
using Temple.Persistence.EFCore.Identity;

namespace Temple.API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services,
            IConfiguration config,
            bool deployingToHeroku)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebAPIv5", Version = "v1" });
            });

            var connectionString = string.Empty;

            if (deployingToHeroku)
            {
                Console.WriteLine("\nDEPLOYING TO HEROKU!!!\n");
                var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                Console.WriteLine($"\nENVIRONMENT IS: {env}!!!\n");

                // Depending on if in development or production, use either Heroku-provided
                // connection string, or development connection string from env var.
                if (env == "Development")
                {
                    Console.WriteLine("\nReading connection string from appsettings.json\n");

                    // Use connection string from file.
                    connectionString = config.GetConnectionString("DefaultConnection");
                }
                else
                {
                    Console.WriteLine("\nReading connection string from Heroku config variable: DATABASE_URL\n");

                    // Use connection string provided at runtime by Heroku.
                    var connUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

                    // Parse connection URL to connection string for Npgsql
                    connUrl = connUrl.Replace("postgres://", string.Empty);
                    var pgUserPass = connUrl.Split("@")[0];
                    var pgHostPortDb = connUrl.Split("@")[1];
                    var pgHostPort = pgHostPortDb.Split("/")[0];
                    var pgDb = pgHostPortDb.Split("/")[1];
                    var pgUser = pgUserPass.Split(":")[0];
                    var pgPass = pgUserPass.Split(":")[1];
                    var pgHost = pgHostPort.Split(":")[0];
                    var pgPort = pgHostPort.Split(":")[1];

                    connectionString = $"Server={pgHost};Port={pgPort};User Id={pgUser};Password={pgPass};Database={pgDb}; SSL Mode=Require; Trust Server Certificate=true";
                }
            }
            else
            {
                Console.WriteLine("\nDEPLOYING LOCALLY!!!\n");
                connectionString = config.GetConnectionString("DefaultConnection");
            }

            Console.WriteLine($"\nCONNECTION STRING IS: {connectionString}!!!\n");

            services.AddIdentityPersistence<DataContext>(options =>
            {
                //options.UseSqlite(connectionString);
                options.UseNpgsql(connectionString);
                //options.UseSqlServer(connectionString);
            });

            services.AddDummyPersistence<DataContext2>(options =>
            {
                //options.UseSqlite(connectionString);
                options.UseNpgsql(connectionString);
                //options.UseSqlServer(connectionString);
            });

            services.AddAppDataPersistence<PRDbContextBase>(options =>
            {
                //options.UseSqlite(connectionString);
                options.UseNpgsql(connectionString);
                //options.UseSqlServer(connectionString);
            });

            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", policy =>
                {
                    policy
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                        .WithOrigins("http://localhost:3000");
                });
            });
            services.AddApplication();
            services.AddAutoMapper(assemblies: typeof(MappingProfiles).Assembly);
            services.AddScoped<IUserAccessor, UserAccessor>();
            services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();
            services.AddScoped<IBusinessRuleCatalog, BusinessRuleCatalog>();
            services.AddScoped<IPagingHandler<SmurfDto>, PagingHandler<SmurfDto>>();
            services.AddScoped<IPagingHandler<PersonDto>, PagingHandler<PersonDto>>();

            return services;
        }
    }
}