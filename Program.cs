using InternalRequestSystem.Data;
using InternalRequestSystem.Models;
using InternalRequestSystem.Repositories;
using Microsoft.EntityFrameworkCore;
using InternalRequestSystem.Middleware;
using Serilog;
using FluentValidation;
using FluentValidation.AspNetCore;
using InternalRequestSystem.Validators;


var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));
// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddValidatorsFromAssemblyContaining<RequestValidator>();

// Configure Entity Framework Core with SQL Server
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(
   builder.Configuration.GetConnectionString("DefaultConnection")));
// Register the RequestRepository for dependency injection
builder.Services.AddScoped<IRequestRepository, RequestRepository>();

// Add session services
builder.Services.AddSession();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    context.Database.Migrate();

    if (!context.Departments.Any())
    {
        context.Departments.AddRange(
            new Department { DepartmentName = "IT" },
            new Department { DepartmentName = "HR" },
            new Department { DepartmentName = "Finance" }
        );
        context.SaveChanges();
    }

    if (!context.Roles.Any())
    {
        context.Roles.AddRange(
            new Role { RoleName = "Employee" },
            new Role { RoleName = "Manager" },
            new Role { RoleName = "Admin" }
        );

        context.SaveChanges();
    }

    var itDepartment = context.Departments.First(d => d.DepartmentName == "IT");
    var hrDepartment = context.Departments.First(d => d.DepartmentName == "HR");
    var financeDepartment = context.Departments.First(d => d.DepartmentName == "Finance");

    var employeeRole = context.Roles.First(r => r.RoleName == "Employee");
    var managerRole = context.Roles.First(r => r.RoleName == "Manager");
    var adminRole = context.Roles.First(r => r.RoleName == "Admin");

    if (!context.AppUsers.Any(u => u.Email == "lidya.employee@internal.com"))
    {
        context.AppUsers.Add(new AppUser
        {
            FullName = "lidya",
            Email = "lidya.employee@internal.com",
            DepartmentId = itDepartment.Id,
            RoleId = employeeRole.Id
        });
    }

    if (!context.AppUsers.Any(u => u.Email == "ahmad.employee@internal.com"))
    {
        context.AppUsers.Add(new AppUser
        {
            FullName = "ahmad",
            Email = "ahmad.employee@internal.com",
            DepartmentId = itDepartment.Id,
            RoleId = employeeRole.Id
        });
    }

    if (!context.AppUsers.Any(u => u.Email == "sara.manager@internal.com"))
    {
        context.AppUsers.Add(new AppUser
        {
            FullName = "sara",
            Email = "sara.manager@internal.com",
            DepartmentId = hrDepartment.Id,
            RoleId = managerRole.Id
        });
    }

    if (!context.AppUsers.Any(u => u.Email == "admin.admin@internal.com"))
    {
        context.AppUsers.Add(new AppUser
        {
            FullName = "admin",
            Email = "admin.admin@internal.com",
            DepartmentId = financeDepartment.Id,
            RoleId = adminRole.Id
        });
    }

    context.SaveChanges();

}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // Enable session middleware

// Add the global exception handling middleware 
//Request --> GlobalExceptionMiddleware --> Controller --> Repository --> Response   ???? ???
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
