using Microsoft.OpenApi.Models;
using ExamPortal.Application.Interfaces;
using ExamPortal.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ExamPortal.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Scans the Application assembly to register all CQRS Handlers
            builder.Services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(ExamPortal.Application.Interfaces.IExamPortalDbContext).Assembly));

            // Register ExamPortalDbContext with Dependency Injection
            //builder.Services.AddDbContext<ExamPortalDbContext>(options =>
            //options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddDbContext<IExamPortalDbContext, ExamPortalDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Configure JWT Authentication
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
              .AddJwtBearer(options =>
              {
                  // Extract values using GetSection().Value to avoid bracket stripping
                  var secretKey = builder.Configuration.GetSection("JwtSettings:Secret").Value;
                  var issuer = builder.Configuration.GetSection("JwtSettings:Issuer").Value;
                  var audience = builder.Configuration.GetSection("JwtSettings:Audience").Value;

                  options.TokenValidationParameters = new TokenValidationParameters
                  {
                      ValidateIssuer = true,
                      ValidateAudience = true,
                      ValidateLifetime = true,
                      ValidateIssuerSigningKey = true,

                      ValidIssuer = issuer,
                      ValidAudience = audience,
                      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!))
                  };
              });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Enter YOUR_TOKEN_HERE (without the 'Bearer ' prefix).",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

            // Add services to the container.
            builder.Services.AddAuthorization();
            builder.Services.AddControllers();

            // CORS configuration to allow requests from Angular frontend
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngular", policyBuilder =>
                    policyBuilder.WithOrigins("http://localhost:4200")
                                .AllowAnyMethod()
                                .AllowAnyHeader());
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // for CORS to allow Angular frontend to access the API
            app.UseCors("AllowAngular");

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();
            app.Run();
        }
    }
}
