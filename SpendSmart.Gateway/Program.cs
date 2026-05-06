var builder = WebApplication.CreateBuilder(args);

// Add YARP Reverse Proxy
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Typical production setup: Add CORS for Angular frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy =>
        {
            var allowedOrigins = new List<string> { "http://localhost:4200" };
            var prodUrl = builder.Configuration["AllowedOrigin"];
            if (!string.IsNullOrEmpty(prodUrl))
                allowedOrigins.Add(prodUrl);

            policy.WithOrigins(allowedOrigins.ToArray())
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

var app = builder.Build();

app.UseCors("AllowAngularApp");

// Setup the proxy endpoints
app.MapReverseProxy();

app.Run();
