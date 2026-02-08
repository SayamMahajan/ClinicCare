using ClinicCare.Api.Infrastructure.Security;
using ClinicCare.Api.Middlewares;
using ClinicCare.Business.Interfaces;
using ClinicCare.Business.Services;
using ClinicCare.Business.Services.Interfaces;
using ClinicCare.Business.Utils;
using ClinicCare.DataAccess.Data;
using ClinicCare.DataAccess.Repositories;
using ClinicCare.DataAccess.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

namespace ClinicCare.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(
                    new JsonStringEnumConverter()
                );
            });

            builder.Services.AddDbContextPool<AppDbContext>(
                options => options.UseSqlServer(builder.Configuration.GetConnectionString("ClinicDBConnection")));

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddScoped<ICurrentUser, CurrentUser>();
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IAppointmentService, AppointmentService>();
            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            builder.Services.AddScoped<IPatientService, PatientService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<IEmployeeService, EmployeeService>();
            builder.Services.AddScoped<IPrescriptionService, PrescriptionService>();
            builder.Services.AddScoped<ISpecializationService, SpecializationService>();
            builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
            builder.Services.AddScoped<IPrescriptionRepository, PrescriptionRepository>();
            builder.Services.AddScoped<IPatientRepository, PatientRepository>();
            builder.Services.AddScoped<ISpecializationRepository, SpecializationRepository>();

            builder.Services.AddHostedService<AppointmentStatusService>();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(opt =>
            {
                opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using Bearer scheme. Example: \"{token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer"
                });

                opt.AddSecurityRequirement(new OpenApiSecurityRequirement
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

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidAudience = builder.Configuration["JWT:Audience"],
                    ValidIssuer = builder.Configuration["JWT:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]!))
                };
            });

            builder.Services.AddAuthorizationBuilder();
            //.SetFallbackPolicy(new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
            //    .RequireAuthenticatedUser()
            //    .Build());

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("frontend", p =>
                    p.WithOrigins("http://localhost:4200")
                     .AllowAnyHeader()
                     .AllowAnyMethod());
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseHttpsRedirection();

            app.UseCors("frontend");

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
