using BankApp.Entity.Security;
using BankApp.Services.Repositories.Implementations;
using BankApp.Services.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using Microsoft.AspNetCore.Identity;

using Microsoft.EntityFrameworkCore;

using Microsoft.IdentityModel.Tokens;

using Microsoft.OpenApi.Models;

using System.Text;
using WebApp.BankingApi.Entity.Models;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>

options.UseSqlServer(connectionString));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(option =>

{

    option.Password.RequiredLength = 7;

    option.Password.RequireDigit = true;

    option.Password.RequireUppercase = true;

    option.Password.RequireNonAlphanumeric = true;

}).AddEntityFrameworkStores<ApplicationDbContext>()

.AddDefaultTokenProviders();

// ⭐ Configure JWT Authentication

builder.Services.AddAuthentication(options =>

{

    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

})

.AddJwtBearer(options =>

{

    options.TokenValidationParameters = new TokenValidationParameters

    {

        ValidIssuer = builder.Configuration["Jwt:Issuer"],

        ValidAudience = builder.Configuration["Jwt:Audience"],

        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),

        ValidateIssuer = true,

        ValidateAudience = true,

        ValidateLifetime = true,

        ValidateIssuerSigningKey = true

    };

});

builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();

builder.Services.AddScoped<IApplicationRepository, ApplicationRepository>();

builder.Services.AddScoped<IAccountTypeRepository, AccountTypeRepository>();

builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();

builder.Services.AddScoped<IBankAccountRepository, BankAccountRepository>();

builder.Services.AddAuthorization();

builder.Services.AddSwaggerGen(options =>

{

    options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme

    {

        Scheme = "bearer",

        Name = "Authorization",

        In = ParameterLocation.Header,

        Type = SecuritySchemeType.Http,

        Description = "Enter JWT bearer token to access API"

    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement

    {

        {

            new OpenApiSecurityScheme

            {

                Reference = new OpenApiReference

                {

                    Type = ReferenceType.SecurityScheme,

                    Id = "bearer"

                }

            },

            new string[] {}

        }

    });

});

var app = builder.Build();

if (app.Environment.IsDevelopment())

{

    app.UseSwagger();

    app.UseSwaggerUI();

}

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
