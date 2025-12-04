using APIUsuarios.Infrastructure.Persistence;
using APIUsuarios.Infrastructure.Repositories;
using APIUsuarios.Application.Interfaces;
using APIUsuarios.Application.Services;
using APIUsuarios.Application.DTOs;
using APIUsuarios.Application.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

builder.Services.AddScoped<IUsuarioService, UsuarioService>();

builder.Services.AddScoped<IValidator<UsuarioCreateDto>, UsuarioCreateDtoValidator>();
builder.Services.AddScoped<IValidator<UsuarioUpdateDto>, UsuarioUpdateDtoValidator>();

var app = builder.Build();

// Endpoints de Usuários (Minimal APIs)
app.MapGet("/usuarios", async (IUsuarioService service, HttpContext http, CancellationToken ct) =>
{
    var usuarios = await service.ListAsync(ct);
    return Results.Ok(usuarios);
})
.WithName("ListarUsuarios");

app.MapGet("/usuarios/{id:int}", async (int id, IUsuarioService service, CancellationToken ct) =>
{
    var usuario = await service.GetAsync(id, ct);
    return usuario is not null ? Results.Ok(usuario) : Results.NotFound();
})
.WithName("ObterUsuario");

app.MapPost("/usuarios", async (
    UsuarioCreateDto dto,
    IUsuarioService service,
    IValidator<UsuarioCreateDto> validator,
    CancellationToken ct) =>
{
    var validation = await validator.ValidateAsync(dto, ct);
    if (!validation.IsValid)
    {
        return Results.ValidationProblem(validation.ToDictionary());
    }

    try
    {
        var created = await service.CreateAsync(dto, ct);
        return Results.Created($"/usuarios/{created.Id}", created);
    }
    catch (InvalidOperationException ex) when (ex.Message == "EMAIL_DUPLICADO")
    {
        return Results.Conflict(new { error = "Email já cadastrado" });
    }
    catch (Exception)
    {
        return Results.Problem(statusCode: 500, title: "Erro no servidor");
    }
})
.WithName("CriarUsuario");

app.MapPut("/usuarios/{id:int}", async (
    int id,
    UsuarioUpdateDto dto,
    IUsuarioService service,
    IValidator<UsuarioUpdateDto> validator,
    CancellationToken ct) =>
{
    var validation = await validator.ValidateAsync(dto, ct);
    if (!validation.IsValid)
    {
        return Results.ValidationProblem(validation.ToDictionary());
    }

    try
    {
        var updated = await service.UpdateAsync(id, dto, ct);
        if (updated is null)
            return Results.NotFound();
        return Results.Ok(updated);
    }
    catch (InvalidOperationException ex) when (ex.Message == "EMAIL_DUPLICADO")
    {
        return Results.Conflict(new { error = "Email já cadastrado" });
    }
    catch (Exception)
    {
        return Results.Problem(statusCode: 500, title: "Erro no servidor");
    }
})
.WithName("AtualizarUsuario");

app.MapDelete("/usuarios/{id:int}", async (int id, IUsuarioService service, CancellationToken ct) =>
{
    var ok = await service.RemoveAsync(id, ct);
    return ok ? Results.NoContent() : Results.NotFound();
})
.WithName("RemoverUsuario");

app.Run();