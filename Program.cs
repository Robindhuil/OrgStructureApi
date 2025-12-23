using Microsoft.EntityFrameworkCore;
using OrgStructureApi.Data;
using System.Linq;
using OrgStructureApi.Extensions;
using OrgStructureApi.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using Scalar.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "OrgStructure API",
        Version = "v1"
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(kvp => kvp.Value?.Errors?.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).Where(s => !string.IsNullOrEmpty(s)).ToArray()
            );

        var problem = new ApiErrorResponse(StatusCodes.Status400BadRequest, "Validation Error")
        {
            Errors = errors
        };

        return new BadRequestObjectResult(problem);
    };
});
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        "Server=localhost,1433;Database=OrgDb;User Id=sa;Password=Heslo123!;TrustServerCertificate=True"
    ));


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("OrgStructure API")
            .WithTheme(ScalarTheme.Solarized)
            .WithOpenApiRoutePattern("/swagger/{documentName}/swagger.json")
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });

}


app.UseApiExceptionHandler();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
