using API.Contracts;
using API.Data;
using API.Repositories;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using API.Utilities.Handlers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<BookingManagementDbContext>(option => option.UseSqlServer(connectionString));

// Add repositories to the container.
builder.Services.AddScoped<IUniversityRepository, UniversityRepository>();
builder.Services.AddScoped<IEducationRepository, EducationRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAccountRoleRepository, AccountRoleRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();

// Mendaftarkan layanan Controller dengan konfigurasi penanganan kesalahan validasi kustom
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        // Konfigurasi penanganan kesalahan validasi kustom
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState.Values
                                .SelectMany(v => v.Errors)
                                .Select(v => v.ErrorMessage);

            return new BadRequestObjectResult(new ResponseValidatorHandler(errors));
        };
    });

// Mendaftarkan layanan Controller
builder.Services.AddControllers();

// Mendaftarkan layanan untuk API Explorer (Swagger/OpenAPI)
builder.Services.AddEndpointsApiExplorer();

// Mendaftarkan layanan untuk Swagger
builder.Services.AddSwaggerGen();

// Menambahkan layanan FluentValidation
builder.Services.AddFluentValidationAutoValidation()
       .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
   .AddNegotiate();

builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy.
    options.FallbackPolicy = options.DefaultPolicy;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
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
