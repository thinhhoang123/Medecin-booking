using Doctor.Application.Interfaces;
using Doctor.Application.Mappings;
using Doctor.Application.Services;
using Doctor.Domain.Interfaces;
using Doctor.Infrastructure.Data;
using Doctor.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Database Context

// Configure Database with Connection String
builder.Services.AddDbContext<DoctorDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptionsAction: sqlOptions =>
        {
            sqlOptions.MigrationsAssembly(typeof(DoctorDbContext).Assembly.FullName);
            sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
        }));


// Repositories
builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
builder.Services.AddScoped<IDoctorScheduleRepository, DoctorScheduleRepository>();

// Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Services
builder.Services.AddScoped<IDoctorService, DoctorService>();

// AutoMapper
builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile));

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Doctor Service API v1");
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
// app.UseAuthorization();
app.MapControllers();

app.Run();
