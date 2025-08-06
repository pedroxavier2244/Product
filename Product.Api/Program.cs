using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Product.Api.Middleware;
using Product.Application.Interfaces;
using Product.Application.Mappings;
using Product.Application.Services;
using Product.Domain.Interfaces;
using Product.Infrastructure.Data;
using Product.Infrastructure.Repositories;
using Serilog; 

var builder = WebApplication.CreateBuilder(args);


builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration) 
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console()
);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();


builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(typeof(Product.Application.Mappings.ProductProfile).Assembly);
});


builder.Services.AddValidatorsFromAssembly(typeof(Product.Application.Validators.CreateProductDtoValidator).Assembly);

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseSerilogRequestLogging(options =>
{
    
    options.GetLevel = (httpContext, elapsed, ex) =>
    {
        
        if (elapsed > 1000 || ex != null || httpContext.Response.StatusCode > 499)
        {
            return Serilog.Events.LogEventLevel.Error;
        }
        return Serilog.Events.LogEventLevel.Information;
    };

    
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
        diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"]);
        diagnosticContext.Set("RemoteIP", httpContext.Connection.RemoteIpAddress);
    };
});

app.UseAuthorization();

app.MapControllers();

app.Run();