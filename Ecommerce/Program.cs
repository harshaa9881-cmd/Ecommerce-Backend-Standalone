using Scalar.AspNetCore;
using Ecommerce.Data;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Repositories;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services;
using Ecommerce.Services.Interfaces;
var builder = WebApplication.CreateBuilder(args);

// Register controllers
builder.Services.AddControllers();
// Register OpenAPI
builder.Services.AddOpenApi();

// Configure SQL Server and DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// Register authorization services
builder.Services.AddAuthorization();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

builder.Services.AddScoped<ICategoryService, CategoryService>();
var app = builder.Build();

// Configure HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
