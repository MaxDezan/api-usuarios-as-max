using APIUsuarios.Infrastructure.Persistence;
using APIUsuarios.Infrastructure.Repositories;
using APIUsuarios.Application.Interfaces;
using APIUsuarios.Application.Services;
using APIUsuarios.Application.DTOs;
using APIUsuarios.Application.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

builder.Services.AddScoped<IUsuarioService, UsuarioService>();

builder.Services.AddScoped<IValidator<UsuarioCreateDto>, UsuarioCreateDtoValidator>();
builder.Services.AddScoped<IValidator<UsuarioUpdateDto>, UsuarioUpdateDtoValidator>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "API de Usuários - Inicializada");

app.Run();