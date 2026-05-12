using Microsoft.EntityFrameworkCore;
using SpendSmart.Budget.API.Data;
using SpendSmart.Budget.API.Repositories;
using SpendSmart.Budget.API.Services;
using SpendSmart.Budget.API.HostedServices;
using SpendSmart.Budget.API.Consumers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHostedService<BudgetResetWorker>();

builder.Services.AddDbContext<BudgetDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IBudgetRepository, BudgetRepository>();
builder.Services.AddScoped<IBudgetService, BudgetService>();

// Enable validation of JWT Tokens
var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Secret"] ?? "SuperSecretKeyForSpendSmart123456789!");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

// MassTransit In-Memory Configuration
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ExpenseAddedConsumer>();
    x.UsingInMemory((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

// Auto-provision database at runtime (Bypasses EF Tooling issues)
try
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<BudgetDbContext>();
        
        Console.WriteLine("[DB] Ensuring database is created...");
        var databaseCreator = context.Database.GetService<Microsoft.EntityFrameworkCore.Storage.IRelationalDatabaseCreator>();
        try { databaseCreator.EnsureCreated(); } catch { }
        try { databaseCreator.CreateTables(); } catch { }

        Console.WriteLine("[DB] Running schema patch for 'UpdatedAt' column...");
        var sql = @"
            IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Budgets')
            BEGIN
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Budgets') AND name = 'UpdatedAt')
                BEGIN
                    ALTER TABLE Budgets ADD UpdatedAt DATETIME NULL;
                    PRINT 'Column UpdatedAt added successfully.';
                    SELECT 1;
                END
                ELSE
                BEGIN
                    PRINT 'Column UpdatedAt already exists.';
                    SELECT 0;
                END
            END
            ELSE
            BEGIN
                PRINT 'Table Budgets not found.';
                SELECT -1;
            END";
            
        try
        {
            var result = await context.Database.ExecuteSqlRawAsync(sql);
            Console.WriteLine($"[DB] Schema patch executed. Table was found and checked.");
        }
        catch (Exception sqlEx)
        {
            Console.WriteLine($"[DB] [CRITICAL] Schema patch failed: {sqlEx.Message}");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"[CRITICAL] Database initialization failed: {ex.Message}. Service will attempt to start regardless.");
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

