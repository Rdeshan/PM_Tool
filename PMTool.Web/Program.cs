using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using PMTool.Application.Services.Auth;
using PMTool.Application.Services.RBAC;
using PMTool.Application.Services.Project;
using PMTool.Application.Services.Product;
using PMTool.Application.Services.SubProject;
using PMTool.Application.Services.Backlog;
using PMTool.Application.Services.Team;
using PMTool.Application.Services.Admin;
using PMTool.Application.DTOs.Product;
using PMTool.Application.DTOs.SubProject;
using PMTool.Application.Validators.Product;
using PMTool.Application.Validators.SubProject;
using PMTool.Infrastructure.Data;
using PMTool.Infrastructure.Repositories;
using PMTool.Infrastructure.Repositories.Interfaces;
using PMTool.Infrastructure.Services;
using PMTool.Infrastructure.Services.Interfaces;
using PMTool.Infrastructure.Settings;
using PMTool.Application.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// FluentValidation
builder.Services.AddScoped<IValidator<CreateProductRequest>, CreateProductRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateProductRequest>, UpdateProductRequestValidator>();
builder.Services.AddScoped<IValidator<CreateSubProjectRequest>, CreateSubProjectRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateSubProjectRequest>, UpdateSubProjectRequestValidator>();

// Email Settings
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Email"));

// Sessions
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Infrastructure Services
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ISubProjectRepository, SubProjectRepository>();
builder.Services.AddScoped<IProjectBacklogRepository, ProjectBacklogRepository>();
builder.Services.AddScoped<IProductBacklogRepository, ProductBacklogRepository>();
builder.Services.AddScoped<IUserAdminRepository, UserAdminRepository>();
builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<DataSeedingService>();

// Application Services
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ISubProjectService, SubProjectService>();
builder.Services.AddScoped<IBacklogService, BacklogService>();
builder.Services.AddScoped<IProductBacklogService, ProductBacklogService>();
builder.Services.AddScoped<IUserAdminService, UserAdminService>();
builder.Services.AddScoped<ITeamService, TeamService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    // Initialize default roles and permissions
    var roleService = scope.ServiceProvider.GetRequiredService<IRoleService>();
    await roleService.InitializeDefaultRolesAsync();

    // Seed test users for each role
    var seedingService = scope.ServiceProvider.GetRequiredService<DataSeedingService>();
    await seedingService.SeedTestUsersAsync();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

// Add sessions and authentication middleware
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
