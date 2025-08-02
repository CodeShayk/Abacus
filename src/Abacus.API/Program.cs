using Abacus.API.Contracts;
using Abacus.API.Data;
using Abacus.API.Repositories.Impl;
using Abacus.Core;
using Abacus.Core.Model;
using Abacus.Core.Services;
using Abacus.Core.Services.Impl;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskInstance = Abacus.Core.Model.TaskInstance;
using WorkflowInstance = Abacus.Core.Model.WorkflowInstance;
using WorkflowTemplate = Abacus.Core.Model.WorkflowTemplate;

var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.

//builder.Services.AddControllers();
//// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.MapOpenApi();
//}

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

//app.Run();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Entity Framework
builder.Services.AddDbContext<WorkflowDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Main")));

builder.Services.AddAbacus(configure =>
{
    configure
        .WithTemplates(c => new TemplateRepository(c.GetRequiredService<WorkflowDbContext>()))
        .WithInstances(c => new InstanceRepository(c.GetRequiredService<WorkflowDbContext>()))
        .WithTasks(c => new TaskInstanceRepository(c.GetRequiredService<WorkflowDbContext>()));
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();
    context.Database.Migrate();
}

// https://localhost:32769/swagger/index.html

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.UseAuthorization();
app.MapControllers();

app.Run();