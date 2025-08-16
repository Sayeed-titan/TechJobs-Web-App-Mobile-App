using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using TechJobs.Application.Interfaces;
using TechJobs.Application.Interfaces.Repositories;
using TechJobs.Application.Interfaces.Services;
using TechJobs.Application.Services;
using TechJobs.Infrastructure.Data;
using TechJobs.Infrastructure.Data.Seed;
using TechJobs.Infrastructure.UnitOfWork;

var builder = WebApplication.CreateBuilder(args);

// SQL Server + EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var cs = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(cs);
});

// DI: UnitOfWork + Services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IJobService, JobService>();

builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        opt.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    }); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");

app.MapControllers();

// ---- DB Migrate + Seed ----
using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbSeeder.SeedAsync(ctx);
}

app.Run();
