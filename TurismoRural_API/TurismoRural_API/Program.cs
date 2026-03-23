var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Dapper context and repositories
builder.Services.AddSingleton<TurismoRural_API.Repositories.DapperContext>();
builder.Services.AddScoped<TurismoRural_API.Interfaces.IUserRepository, TurismoRural_API.Repositories.UserRepository>();
builder.Services.AddScoped<TurismoRural_API.Interfaces.IExperienceRepository, TurismoRural_API.Repositories.ExperienceRepository>();
builder.Services.AddScoped<TurismoRural_API.Interfaces.IReservationRepository, TurismoRural_API.Repositories.ReservationRepository>();
// Password helper/service
builder.Services.AddScoped<TurismoRural_API.Services.IPasswordHelper, TurismoRural_API.Services.PasswordHelper>();

// Authentication - JWT
builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.RequireHttpsMetadata = false;
        opt.SaveToken = true;
        opt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("Jwt:Key") ?? string.Empty)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
    });

// Swagger (Swashbuckle)
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Serve Swagger UI at application root during development
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TurismoRural API V1");
        c.RoutePrefix = string.Empty; // serve at '/'
    });
    // Try to open Swagger UI in Microsoft Edge when the app starts (development only)
    try
    {
        // Determine URL automatically by reading Properties/launchSettings.json if available
        string swaggerUrl = "https://localhost:7054/";
        try
        {
            var contentRoot = app.Environment.ContentRootPath;
            var launchPath = System.IO.Path.Combine(contentRoot, "Properties", "launchSettings.json");
            if (System.IO.File.Exists(launchPath))
            {
                using var fs = System.IO.File.OpenRead(launchPath);
                var doc = System.Text.Json.JsonDocument.Parse(fs);
                if (doc.RootElement.TryGetProperty("profiles", out var profiles))
                {
                    foreach (var prop in profiles.EnumerateObject())
                    {
                        if (prop.Value.TryGetProperty("applicationUrl", out var appUrlEl))
                        {
                            var appUrl = appUrlEl.GetString();
                            if (!string.IsNullOrEmpty(appUrl))
                            {
                                var first = appUrl.Split(';')[0];
                                if (!first.StartsWith("http", System.StringComparison.OrdinalIgnoreCase))
                                    first = "https://" + first;
                                swaggerUrl = first.EndsWith("/") ? first : first + "/";
                                break;
                            }
                        }
                    }
                }
            }
        }
        catch
        {
            // ignore and use default
        }

        var psi = new System.Diagnostics.ProcessStartInfo
        {
            FileName = "msedge",
            Arguments = swaggerUrl,
            UseShellExecute = true
        };
        System.Diagnostics.Process.Start(psi);
    }
    catch
    {
        // If msedge is not available, fall back to default browser
        try
        {
            // reuse same logic to get URL or fallback to default
            string swaggerUrl = "https://localhost:7054/";
            try
            {
                var contentRoot = app.Environment.ContentRootPath;
                var launchPath = System.IO.Path.Combine(contentRoot, "Properties", "launchSettings.json");
                if (System.IO.File.Exists(launchPath))
                {
                    using var fs = System.IO.File.OpenRead(launchPath);
                    var doc = System.Text.Json.JsonDocument.Parse(fs);
                    if (doc.RootElement.TryGetProperty("profiles", out var profiles))
                    {
                        foreach (var prop in profiles.EnumerateObject())
                        {
                            if (prop.Value.TryGetProperty("applicationUrl", out var appUrlEl))
                            {
                                var appUrl = appUrlEl.GetString();
                                if (!string.IsNullOrEmpty(appUrl))
                                {
                                    var first = appUrl.Split(';')[0];
                                    if (!first.StartsWith("http", System.StringComparison.OrdinalIgnoreCase))
                                        first = "https://" + first;
                                    swaggerUrl = first.EndsWith("/") ? first : first + "/";
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                // ignore
            }

            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo { FileName = swaggerUrl, UseShellExecute = true });
        }
        catch
        {
            // ignore
        }
    }
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();
app.Run();
