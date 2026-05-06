using Microsoft.EntityFrameworkCore;
using SpendSmart.Income.API.Data;
using SpendSmart.Income.API.Repositories;
using SpendSmart.Income.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient("ExpenseApi", client =>
{
    // Hit Expense API directly on port 5003 to avoid re-entry issues
    client.BaseAddress = new Uri("http://localhost:5003"); 
});



builder.Services.AddDbContext<IncomeDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IIncomeRepository, IncomeRepository>();
builder.Services.AddScoped<IIncomeService, IncomeService>();

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

var app = builder.Build();

// Auto-provision database at runtime (Bypasses EF Tooling issues)
try
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<IncomeDbContext>();
        context.Database.EnsureCreated();
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

