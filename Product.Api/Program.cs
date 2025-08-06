using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Product.Api.Middleware;
using Product.Application.Interfaces;
using Product.Application.Mappings;
using Product.Application.Services;
using Product.Domain.Interfaces;
using Product.Infrastructure.Data;
using Product.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Repositories and Services
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

// FORMA CORRETA E DEFINITIVA PARA VERSÕES RECENTES
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(typeof(Product.Application.Mappings.ProductProfile).Assembly);
});
// Register FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(Product.Application.Validators.CreateProductDtoValidator).Assembly);


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ErrorHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();