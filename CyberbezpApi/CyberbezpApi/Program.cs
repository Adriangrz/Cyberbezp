using CyberbezpApi.Database;
using CyberbezpApi.Database.Entities;
using CyberbezpApi.Middleware;
using CyberbezpApi.Services.Interfaces;
using CyberbezpApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using CyberbezpApi.Authorization;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        b => b.WithOrigins(builder.Configuration["AllowedOrigins"])
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "OnlineLearningApi", Version = "v1.0.0" });

    var securitySchema = new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", securitySchema);

    var securityRequirement = new OpenApiSecurityRequirement
                {
                    { securitySchema, new[] { "Bearer" } }
                };

    c.AddSecurityRequirement(securityRequirement);

});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseInMemoryDatabase(databaseName: "Test");
});

builder.Services.AddIdentity<User, IdentityRole>(
    opts =>
    {
        opts.Password.RequireNonAlphanumeric = false;
        opts.Password.RequireUppercase = false;
        opts.Password.RequiredLength = 1;
        opts.Password.RequireDigit = false;
        opts.Password.RequireLowercase = false;
        opts.Password.RequiredUniqueChars = 0;
        opts.SignIn.RequireConfirmedAccount = false; //development

        opts.User.RequireUniqueEmail = true;
        opts.User.AllowedUserNameCharacters = "Aa•πBbCc∆ÊDdEe ÍFfGgHhIiJjKkLl£≥MmNn—ÒOo”ÛPpQqRrSsåúTtUuvWwxYyZzèüØø0123456789-._@+/!#$%^&*~`?|/=";

    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false; //false only in development
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Jwt:SecretKey"])),
    };
});
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAuthorizationHandler, ResourceOperationRequirementHandler>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthorizationInitializer, AuthorizationInitializer>();
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddScoped<ErrorHandlingMiddleware>();
builder.Services.AddSingleton<SettingsService>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

SeedData(app);

//Seed Data
void SeedData(IHost app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    using (var scope = scopedFactory.CreateScope())
    {
        var service2 = scope.ServiceProvider.GetService<IAuthorizationInitializer>();
        service2.GenerateAdminAndRoles().Wait();
    }
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
