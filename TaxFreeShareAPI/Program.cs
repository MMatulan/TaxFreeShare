using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TaxFreeShareAPI.ChatHub;
using TaxFreeShareAPI.Data;
using TaxFreeShareAPI.Options;
using TaxFreeShareAPI.Services;
using TaxFreeShareAPI.Services.Interface;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Nødvendig tjenester
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Email config
builder.Services.Configure<EmailOptions>(
    builder.Configuration.GetSection(EmailOptions.EmailOptionsKey));
builder.Services.AddScoped<IEmailService, EmailService>();  

// Andre tjenester
builder.Services.AddScoped<ProductService>();

// SignalR for sanntidschat
builder.Services.AddSignalR();


// JWT-konfigurasjon
// Hent JWT-nøkkelen fra appsettings.json
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new Exception("JWT Key is missing in appsettings.json");
var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

// Konfigurer autentisering med JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = securityKey
        };
    });


// Autorisasjon
builder.Services.AddAuthorization();

// Databasekonfigurasjon
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new Exception("Database connection string is missing!");

builder.Services.AddDbContext<TaxFreeDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));


// Swagger med JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TaxFreeShareAPI", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Skriv inn 'Bearer <din_token>' for å autorisere. Bruk e-postadressen din for innlogging."
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new List<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5200") 
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});


var app = builder.Build();

// SignalR-endepunkt
app.MapHub<ChatHub>("/chatHub");

// Importer produkter fra CSV
var productService = app.Services.CreateScope().ServiceProvider.GetRequiredService<ProductService>();
await productService.ImportProductsFromCsv("Data/products.csv");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
