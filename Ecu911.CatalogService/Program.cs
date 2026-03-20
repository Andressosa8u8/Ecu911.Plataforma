using Ecu911.CatalogService.Data;
using Microsoft.EntityFrameworkCore;
using Ecu911.CatalogService.Repositories;
using Ecu911.CatalogService.Services;
using Ecu911.CatalogService.Interfaces;
using Ecu911.CatalogService.Validators;
using Ecu911.CatalogService.Middlewares;
using Ecu911.CatalogService.Configuration;
using Ecu911.CatalogService.Services.FileStorage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using FluentValidation;
using FluentValidation.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

// Servicios
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateDocumentItemDtoValidator>();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<AuditService>();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese el token JWT en formato: Bearer {token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IDocumentItemRepository, DocumentItemRepository>();
builder.Services.AddScoped<IDocumentItemService, DocumentItemService>();
builder.Services.AddScoped<IDocumentTypeRepository, DocumentTypeRepository>();
builder.Services.AddScoped<IDocumentTypeService, DocumentTypeService>();

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.Configure<FileStorageOptions>(
    builder.Configuration.GetSection("FileStorage"));
builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();

builder.Services.AddScoped<IDocumentFileRepository, DocumentFileRepository>();
builder.Services.AddScoped<IDocumentFileService, DocumentFileService>();

builder.Services.AddScoped<IOrganizationalUnitRepository, OrganizationalUnitRepository>();
builder.Services.AddScoped<IOrganizationalUnitService, OrganizationalUnitService>();

builder.Services.AddScoped<IRepositoryNodeRepository, RepositoryNodeRepository>();
builder.Services.AddScoped<IRepositoryNodeService, RepositoryNodeService>();

builder.Services.AddScoped<INodePermissionRepository, NodePermissionRepository>();
builder.Services.AddScoped<INodePermissionService, NodePermissionService>();

builder.Services.AddScoped<IDownloadAuditRepository, DownloadAuditRepository>();
builder.Services.AddScoped<IDownloadAuditService, DownloadAuditService>();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();