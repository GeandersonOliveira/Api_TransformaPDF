using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Business;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddScoped<JsonData>();
builder.Services.AddScoped<TransformaPdfCore>();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors(options =>
            options.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());
app.UseHttpsRedirection();

// Concatenar PDFs a partir de URLs (JSON)
app.MapPost("/api/ConcatenaPdfsByUrl", async Task<IResult> ([FromBody] IEnumerable<string> urls, TransformaPdfCore transforma) =>
{
    var output = await transforma.PdfConcatenation(urls);
    return Results.File(output, "application/octet-stream","FileMerged.pdf");

}).WithTags("ConcatenarPdfsByUrl");

// Endpoint: Concatenar PDFs enviados como arquivo
app.MapPost("/api/ConcatenarPdfs", async (HttpRequest req) =>
{
    if (!req.HasFormContentType)
        return Results.BadRequest();

    var form = await req.ReadFormAsync();
    var arquivos = form.Files;

    if (arquivos.Count > 1)

    {
        var arquivosBytes = await PdfTools.ObterArquivos(arquivos);
        var output = TransformaPdfCore.PdfConcatenation(arquivosBytes);
        return Results.File(output, "application/octet-stream", "FileMerged.pdf");
    }

    return Results.BadRequest();

}).Accepts<IFormFileCollection>("multipart/form-data").WithTags("ConcatenarPdfs");

//endpoint para concatenar utilizando arquivos(IFormFile) e url(esta por querystring)

// Endpoint: Concatenar PDFs combinando arquivos e URL
app.MapPost("/api/ConcatenaUrlEArquivo", async Task<IResult> (
    HttpRequest request,
    string url,
    TransformaPdfCore transforma) =>
{
    if (string.IsNullOrWhiteSpace(url))
        return Results.BadRequest("URL não fornecida.");

    if (!request.HasFormContentType)
        return Results.BadRequest();

    var form = await request.ReadFormAsync();
    var arquivos = form.Files;

    if (arquivos == null || arquivos.Count == 0)
        return Results.BadRequest("Nenhum arquivo fornecido.");

    var arquivosBytes = await PdfTools.ObterArquivo(arquivos);
    var output = await transforma.ConcatenarUrlEArquivo(url, arquivosBytes);
    return Results.File(output, "application/octet-stream", "FileMerged.pdf");

}).WithTags("ConcatenaUrlEArquivo");

app.MapGet("/health/live", () =>

{ return Results.Ok(); }).WithTags("Teste");

app.Run();