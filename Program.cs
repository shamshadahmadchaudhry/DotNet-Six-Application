using JwtWebApiTutorial.Controllers;
using JwtWebApiTutorial.DbConnection;
using JwtWebApiTutorial.Models;
using JwtWebApiTutorial.Service;
using JwtWebApiTutorial.Services.Interface;
using JwtWebApiTutorial.Services.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens; 

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager configuration = builder.Configuration;
//var logger = new LoggerConfiguration()
//.ReadFrom.Configuration(builder.Configuration)
//.CreateLogger();
////Add Logger
//Log.Logger = logger;
//builder.Host.UseSerilog(logger);
builder.Services.AddControllers();
// Add services to the container.
//builder.Services.AddSingleton(IConfiguration);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
 {
     options.SaveToken = false;
     options.RequireHttpsMetadata = false;
     options.TokenValidationParameters = new TokenValidationParameters
     {
         ValidateIssuer = false,
         ValidateAudience = false,
         ValidateIssuerSigningKey = true,
         //ClockSkew = TimeSpan.Zero,
         //ValidAudience = configuration["JWT:ValidAudience"],
         //ValidIssuer = configuration["JWT:ValidIssuer"],
         IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuration["AppSettings:Token"]))
     };
 });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<CustomExceptionMiddleware>();
builder.Services.AddSingleton<IAuthInterface, AuthRepository>();
builder.Services.AddSingleton<IDbInterface, DbConnection>();
builder.Services.AddSingleton<IEmployeeInterface, EmployeeRepository>();

var app = builder.Build();
//IConfiguration configuration = app.Configuration;
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
app.UseMiddleware<CustomExceptionMiddleware>();
app.Run();
