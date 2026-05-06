using Microsoft.EntityFrameworkCore;
using SpendSmart.Auth.API.Data;
using SpendSmart.Auth.API.Repositories;
using SpendSmart.Auth.API.Services;
using Microsoft.AspNetCore.Identity;
using SpendSmart.Auth.API.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// Enable validation of JWT Tokens so [Authorize] attributes function normally
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


builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddHttpClient();


// MassTransit In-Memory Configuration
builder.Services.AddMassTransit(x =>
{
    x.UsingInMemory((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

// Auto-provision database and seed Admin at runtime
try
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        context.Database.EnsureCreated();

        var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

        var adminEmail = "admin@spendsmart.com";
        if (!await userRepo.ExistsByEmailAsync(adminEmail))
        {
            var admin = new User
            {
                FullName = "Platform Admin",
                Email = adminEmail,
                Role = "Admin",
                Currency = "USD"
            };
            admin.PasswordHash = passwordHasher.HashPassword(admin, "AdminPassword123!");
            await userRepo.AddUserAsync(admin);
            await userRepo.SaveChangesAsync();
            Console.WriteLine("[SEED] Admin user created: admin@spendsmart.com / AdminPassword123!");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"[CRITICAL] Auth Database initialization failed: {ex.Message}");
    Console.WriteLine("The service will continue to run, but features requiring the database will fail.");
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

