using Ecommerce.API;
using Ecommerce.API.Helpers;
using Ecommerce.API.Services;
using Ecommerce.API.Services.IServices;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
                      policy =>
                      {
                          policy.WithOrigins("http://127.0.0.1:5500",
                                              "http://localhost:5500",
                                              "http://localhost:4200")
                                        .AllowAnyMethod()
                                        .AllowAnyHeader()
                                        .AllowCredentials();
                      });
});


#region External Configuration

StripeConfiguration.ApiKey =
    builder.Configuration["Stripe:SecretKey"];

#endregion

#region Controllers & OpenAPI

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

#endregion

#region Database

builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
    option.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"));
});

#endregion

#region Identity

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = true;
    options.Password.RequiredLength = 8;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

#endregion

#region Repositories

builder.Services.AddScoped<IRepository<Category>, Repository<Category>>();
builder.Services.AddScoped<IRepository<Brand>, Repository<Brand>>();
builder.Services.AddScoped<IRepository<Cart>, Repository<Cart>>();
builder.Services.AddScoped<IProductSubImgsRepository, ProductSubImgsRepository>();
builder.Services.AddScoped<IRepository<Ecommerce.API.Models.Product>, Repository<Ecommerce.API.Models.Product>>();
builder.Services.AddScoped<IRepository<ApplicationUserOTP>, Repository<ApplicationUserOTP>>();
builder.Services.AddScoped<IRepository<UserReview>, Repository<UserReview>>();
builder.Services.AddScoped<IRepository<Promotion>, Repository<Promotion>>();
builder.Services.AddScoped<IRepository<Order>, Repository<Order>>();
builder.Services.AddScoped<IRepository<OrderItem>, Repository<OrderItem>>();

#endregion

#region Application Services

//builder.builder.Services.AddScoped<SessionService>(_ => new SessionService());

builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddScoped<IJWTHandler, JWTHandler>();
builder.Services.AddScoped<IReviewService, Ecommerce.API.Services.ReviewService>();
builder.Services.AddScoped<IFileHandler, FileHandler>();

#endregion

#region Infrastructure Services

// DB Initializer
builder.Services.AddScoped<DbIntializer>();

#endregion

#region Authentication

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
            ValidIssuer = builder.Configuration["JWT:Issuer"],

            ValidateAudience = true,
            ValidAudience = builder.Configuration["JWT:Audience"],

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]!))
        };
    });

#endregion

var app = builder.Build();

#region Development Tools

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

#endregion

#region Database Initialization

using (var scope = app.Services.CreateScope())
{
    var dbInitializer = scope.ServiceProvider.GetRequiredService<DbIntializer>();
    await dbInitializer.DbIntilizer();
}

#endregion

#region Middleware Pipeline

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

#endregion

app.Run();