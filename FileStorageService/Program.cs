using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "File Storage API", Version = "v1" });
});

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

string fileRoot = "/app/files";
Directory.CreateDirectory(fileRoot);

app.MapPost("/api/files", async (HttpRequest request) =>
{
    if (!request.HasFormContentType) return Results.BadRequest("No form");
    var form = await request.ReadFormAsync();
    var file = form.Files.FirstOrDefault();
    if (file == null) return Results.BadRequest("No file");
    if (Path.GetExtension(file.FileName).ToLower() != ".txt")
        return Results.BadRequest("Only .txt allowed");

    var id = Guid.NewGuid().ToString();
    var filePath = Path.Combine(fileRoot, $"{id}.txt");
    using var stream = File.Create(filePath);
    await file.CopyToAsync(stream);
    return Results.Ok(new { id, originalName = file.FileName });
});

app.MapGet("/api/files/{id}", (string id) =>
{
    var filePath = Path.Combine(fileRoot, $"{id}.txt");
    if (!System.IO.File.Exists(filePath)) return Results.NotFound();
    return Results.File(filePath, "text/plain", $"{id}.txt");
});

app.Run();
