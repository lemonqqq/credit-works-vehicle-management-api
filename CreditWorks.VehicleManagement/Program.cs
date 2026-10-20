using CreditWorks.VehicleManagement.Application.Contracts;
using CreditWorks.VehicleManagement.Application.Services;
using CreditWorks.VehicleManagement.Application.Services.Impl;
using CreditWorks.VehicleManagement.Infrastructure.Common;
using CreditWorks.VehicleManagement.Infrastructure.DataStore;
using CreditWorks.VehicleManagement.Infrastructure.Ef;
using CreditWorks.VehicleManagement.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services.AddMemoryCache();

builder.Services.AddScoped(typeof(ILoggingService<>), typeof(LoggingService<>));
builder.Services.AddSingleton<ICacheService, CacheService>();

builder.Services.AddScoped<IVehicleStore, VehicleStore>();
builder.Services.AddScoped<ICategoryStore, CategoryStore>();
builder.Services.AddScoped<IManufacturerStore, ManufacturerStore>();

builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IVehicleCategoryService, VehicleCategoryService>();

builder.Services.AddDbContext<VehicleManagementDbContext>(options =>
    options.UseSqlServer(DatabaseConfiguration.GetConnectionString(builder.Configuration)));

var requestQueueService = new RequestQueueService();

builder.Services.AddHostedService<RequestWorkerService>();
builder.Services.AddSingleton<IRequestQueueWriteService>(requestQueueService);
builder.Services.AddSingleton<IRequestQueuedService>(requestQueueService);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<CorrelationIdMiddleware>();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy"
})).AllowAnonymous();

app.Run();
