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

//endpoint para concatenar utilizando apenas urls de pdf. Devem vir em formato JSON => ["exemple_1.pdf","example_2.pdf"]

app.MapPost("/api/ConcatenaPdfsByUrl", async Task<IResult> ([FromBody] IEnumerable<string> urls, TransformaPdfCore transforma) =>
{
    var output = await transforma.PdfConcatenation(urls);
    return Results.File(output, "application/octet-stream");

}).WithTags("ConcatenarPdfsByUrl");

//endpoint para concatenar utilizando arquivos pdf (IFormFileCollection)

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
        return Results.File(output, "application/octet-stream");
    }

    return Results.BadRequest();

}).Accepts<IFormFileCollection>("multipart/form-data").WithTags("ConcatenarPdfs");

//endpoint para concatenar utilizando arquivos(IFormFile) e url(esta por querystring)

app.MapPost("/api/ConcatenaUrlEArquivo", async Task<IResult> (HttpRequest request, string url, TransformaPdfCore transforma) =>
{
    if (!request.HasFormContentType)
        return Results.BadRequest();

    var file = await request.ReadFormAsync();
    var formFile = file.Files;

    if (formFile != null && formFile.Count() != 0)
    {
        var arquivosBytes = await PdfTools.ObterArquivo(formFile);
        var output = await transforma.ConcatenarUrlEArquivo(url, arquivosBytes);
        return Results.File(output, "application/octet-stream");
    }
    return Results.BadRequest();

}).WithTags("ConcatenaUrlEArquivo");

app.MapGet("/health/live", () =>

{ return Results.Ok(); }).WithTags("Teste");

app.Run();