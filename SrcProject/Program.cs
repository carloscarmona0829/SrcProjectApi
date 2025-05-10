using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using SrcProject.Services.Contract;
using SrcProject.Services.Implement;
using SrcProject.Utilities;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Inyecci�n de dependencias
builder.Services.AddScoped<IAuthentication_Service, Authentication_Service>();
builder.Services.AddScoped<Jwt>();
builder.Services.AddScoped<IAuthorization_Service, Authorization_Service>();

// Configuraci�n de la Autenticaci�n con JWT 
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
        // Par�metros para validaci�n de tokens
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
})
.AddOpenIdConnect("OpenIdConnect", options =>
{
    // Configuraci�n de OpenIdConnect
    options.ClientId = builder.Configuration["AzureAd:ClientId"];
    options.ClientSecret = builder.Configuration["AzureAd:ClientSecret"];
    options.Authority = $"{builder.Configuration["AzureAd:Instance"]}{builder.Configuration["AzureAd:TenantId"]}";
    options.ResponseType = "code";
    options.SaveTokens = true;
    options.CallbackPath = "/Auth/Signin-oidc"; // Aseg�rate de que esto coincida con tu configuraci�n en Azure AD
    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = "name"
    };
    options.Events = new OpenIdConnectEvents
    {
        OnMessageReceived = context =>
        {
            if (string.IsNullOrEmpty(context.ProtocolMessage.State))
            {
                context.HandleResponse();
                context.Response.Redirect("/error?message=State+is+missing+or+invalid");
            }
            return Task.CompletedTask;
        }
    };
});

// Configuraci�n de Cookies
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.Name = ".AspNetCore.Cookies";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
});

// Configuraci�n de Cors
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

app.UseAuthentication(); // Aseg�rate de agregar esto antes de UseAuthorization
app.UseAuthorization();

app.MapControllers();

app.Run();
