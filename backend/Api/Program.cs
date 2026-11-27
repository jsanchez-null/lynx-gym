using System.Text;
using Api.Auth;
using Api.Middleware;
using Api.Repositories;
using Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
    ?? throw new InvalidOperationException("Falta la configuración Jwt en appsettings.json.");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Lynx Gym API",
        Version = "v1",
        Description = "API REST en 3 capas para Lynx Gym (Equipo LYNXWARE)."
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Pega tu token JWT aquí (sin escribir 'Bearer ')."
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// CORS abierto en desarrollo para que Expo Go en dispositivo físico pueda consumir la API.
builder.Services.AddCors(options =>
{
    options.AddPolicy("ExpoDev", policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

// Repositorios: en memoria para pruebas inmediatas en Swagger, o SQL Server cuando UseSqlServer sea true.
var useSqlServer = builder.Configuration.GetValue<bool>("Database:UseSqlServer");
if (useSqlServer)
{
    builder.Services.AddScoped<IUsuarioRepository, SqlUsuarioRepository>();
    builder.Services.AddScoped<IMembresiaRepository, SqlMembresiaRepository>();
    builder.Services.AddScoped<IPromoRepository, SqlPromoRepository>();
}
else
{
    builder.Services.AddSingleton<IUsuarioRepository, InMemoryUsuarioRepository>();
    builder.Services.AddSingleton<IMembresiaRepository, InMemoryMembresiaRepository>();
    builder.Services.AddSingleton<IPromoRepository, InMemoryPromoRepository>();
}

builder.Services.AddSingleton<JwtTokenService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<MembresiaService>();
builder.Services.AddScoped<PromoService>();

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("ExpoDev");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
