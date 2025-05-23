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
using FileAnalysisService.Data;
using FileAnalysisService.Services;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "File Analysis API",
        Version = "v1",
        Description = "Статистика текста и Jaccard similarity"
    });
});

var connectionString = configuration["ConnectionStrings:Default"]
                      ?? "Data Source=/app/data/analysis.db";

builder.Services.AddDbContext<AnalysisDbContext>(opt =>
    opt.UseSqlite(connectionString));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "File Analysis API V1");
});

string fileRoot = "/app/files";
Directory.CreateDirectory(fileRoot);

app.MapPost("/api/analyze/{fileId}", async (string fileId, AnalysisDbContext db) =>
{
    var path = Path.Combine(fileRoot, $"{fileId}.txt");
    if (!File.Exists(path))
        return Results.NotFound("File not found");

    var content = await File.ReadAllTextAsync(path);
    var analysis = TextAnalysisService.Analyze(fileId, content);

    var similarities = new Dictionary<string, double>();
    var previous = await db.Results.ToListAsync();
    foreach (var prev in previous)
    {
        var otherPath = Path.Combine(fileRoot, $"{prev.FileId}.txt");
        if (!File.Exists(otherPath)) continue;
        var otherContent = await File.ReadAllTextAsync(otherPath);
        similarities[prev.FileId] = TextAnalysisService.JaccardSimilarity(content, otherContent);
    }

    db.Results.Add(analysis);
    await db.SaveChangesAsync();

    return Results.Ok(new { analysis, similarities });
});

app.MapGet("/api/analyze/{fileId}", async (string fileId, AnalysisDbContext db) =>
{
    var result = await db.Results
                         .FirstOrDefaultAsync(r => r.FileId == fileId);
    return result is null ? Results.NotFound() : Results.Ok(result);
});

app.Run();

