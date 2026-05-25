using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Repositories;
using PRN232.LMS.Repositories.Repositories.Interfaces;
using PRN232.LMS.Services.Services;
using PRN232.LMS.Services.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Database context
var connectionStr = builder.Configuration.GetConnectionString("MyCnn");
builder.Services.AddDbContext<Prn232LmsContext>(options =>
    options.UseSqlServer(connectionStr));

// Register Repositories
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
builder.Services.AddScoped<ISemesterRepository, SemesterRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();

// Register Services
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<ISemesterService, SemesterServiceImplementation>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PRN232 LMS API V1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Auto migrate database on startup
//using (var scope = app.Services.CreateScope())
//{
//    var db = scope.ServiceProvider.GetRequiredService<Prn232LmsContext>();
//    db.Database.Migrate();
//}


app.Run();
