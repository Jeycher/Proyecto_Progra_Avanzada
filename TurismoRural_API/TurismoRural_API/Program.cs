var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Dapper context and repositories
builder.Services.AddSingleton<TurismoRural_API.Repositories.DapperContext>();
builder.Services.AddScoped<TurismoRural_API.Interfaces.IUserRepository, TurismoRural_API.Repositories.UserRepository>();
builder.Services.AddScoped<TurismoRural_API.Interfaces.IExperienciaRepository, TurismoRural_API.Repositories.ExperienciaRepository>();
builder.Services.AddScoped<TurismoRural_API.Interfaces.IExperienciaConcurrenciaRepository, TurismoRural_API.Repositories.ExperienciaConcurrenciaRepository>();
builder.Services.AddScoped<TurismoRural_API.Interfaces.IReservationRepository, TurismoRural_API.Repositories.ReservationRepository>();
builder.Services.AddScoped<TurismoRural_API.Interfaces.IComunidadRepository, TurismoRural_API.Repositories.ComunidadRepository>();
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

// Swagger (Swashbuckle) with JWT support for Swagger UI
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "TurismoRural API",
        Version = "v1"
    });

    // JWT bearer support so Swagger UI can Authorize requests
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer {token}'"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

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
    // Avoid opening browser when running under debugger to prevent Visual Studio and this code
    // from both launching the same URL.
    if (!System.Diagnostics.Debugger.IsAttached)
    {
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
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
