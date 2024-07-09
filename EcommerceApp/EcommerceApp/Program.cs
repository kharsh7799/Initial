using EcommerceApp.Data;
using Microsoft.EntityFrameworkCore;
using EcommerceApp.Controllers;
using EcommerceApp.Repositories.Contracts;
using EcommerceApp.Repositories.Implementation;
using EcommerceApp.Mapping;
using Serilog;
using Microsoft.OpenApi.Models;
using System.Reflection;
using EcommerceApp.Services.Contracts;
using EcommerceApp.Services.Implementations;
using System.Diagnostics.CodeAnalysis;
/// <summary>
/// Program class
/// </summary>

[ExcludeFromCodeCoverage]
public class Program {
    /// <summary>
    /// Main Method
    /// </summary>
    /// <param name="args"></param>
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "ECommerceSite APIs",
                Version = "v1",
                TermsOfService = new Uri("https://swagger.io/docs/specification/api-general-info/"),
                Contact = new OpenApiContact
                {
                    Name = "Harshad Kulkarni",
                    Email = "harshadku@cybage.com",
                    Url = new Uri("https://help.com")
                }
            });

            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });
        builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseSqlServer(builder.Configuration.GetConnectionString("AppDbConnectionString")));

        builder.Services.AddScoped<IProductRepository, ProductRepository>();
        builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
        builder.Services.AddScoped<ICategoryWithProductsRepo, CategoryWithProductsRepo>();


        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<ICategoryService, CategoryService>();
        builder.Services.AddScoped<ICategoryWithProductsService, CategoryWithProducts>();

        builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

        builder.Logging.ClearProviders();
        builder.Services.AddSerilog(config =>
        {
            config.ReadFrom.Configuration(builder.Configuration);
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();

    }
}