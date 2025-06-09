using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SrcProject.Configuration;
using SrcProject.Models.InModels.Security;
using SrcProject.Services.Contract.Security;
using SrcProject.Services.Implement.Security;
using SrcProject.Utilities;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Configuracion EF Core con SqlServer
builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("cnn")));
//options.UseSqlServer(builder.Configuration["ConnectionStrings:cnn"]));

//Configuración de Identity 
builder.Services.AddIdentity<ApplicationUserIM, IdentityRole>(options =>
{
    // User settings
    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters =
   "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;

    // Password settings
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 3;
    options.Password.RequireNonAlphanumeric = false;

    // Lockout settings
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
    options.Lockout.AllowedForNewUsers = true;

})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Inyección de dependencias
builder.Services.AddScoped<IAuthentication_Service, Authentication_Service>();
builder.Services.AddScoped<Jwt>();
builder.Services.AddScoped<IAuthorization_Service, Authorization_Service>();
builder.Services.AddTransient<EmailService>();

// Configuración de la Autenticación con JWT 
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
// Limpia el mapeo de claims por defecto para que funcionen los claims personalizados.

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // Parámetros para validación de tokens
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        ValidAudience = builder.Configuration["JWTSettings:Audience"],
        ValidIssuer = builder.Configuration["JWTSettings:Issuer"],
        RequireExpirationTime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTSettings:JWTKey"])),
        ValidateIssuerSigningKey = true
    };
});

// Configuración de Cors
builder.Services.AddCors(options =>
{
    var appUrl = builder.Configuration["AppUrl"];
    var appUrlDllo = builder.Configuration["AppUrlDllo"];
    var frontendUrl = builder.Configuration["Frontend_Local_Url"];

    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins(appUrl, frontendUrl, appUrlDllo).AllowAnyMethod().AllowAnyHeader();
    });
});

// Agregar otros servicios
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
