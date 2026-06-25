using TicketingAPI.Database;
using TicketingAPI.Repositories;
using TicketingAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TicketingAPI.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingresá: Bearer {token}"
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
            Array.Empty<string>()
        }
    });
});

builder.Services.AddSingleton<DatabaseConnection>();

builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<EventoRepository>();
builder.Services.AddScoped<EstadioRepository>();
builder.Services.AddScoped<SectorRepository>();
builder.Services.AddScoped<TokenRepository>();
builder.Services.AddScoped<ComisionRepository>();
builder.Services.AddScoped<CompraRepository>();
builder.Services.AddScoped<EntradaRepository>();
builder.Services.AddScoped<TransferenciaRepository>();
builder.Services.AddScoped<EquipoRepository>();
builder.Services.AddScoped<EstadisticasRepository>();
builder.Services.AddScoped<DispositivoRepository>();
builder.Services.AddScoped<ValidacionRepository>();

builder.Services.AddSingleton<JwtService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

var jwtSecret = builder.Configuration["Jwt:Secret"]!;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSecret)
            ),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();