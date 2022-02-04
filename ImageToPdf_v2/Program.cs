using Business;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddScoped<JsonData>();
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors(options =>
            options.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());
app.UseHttpsRedirection();

app.MapGet("/health/live", () =>
{
    return Results.Ok();
});

//endpoint que recebe a imagem como url e faz a transformação para Pdf

app.MapPost("/api/ImagePdf", async Task<IResult> (string file, string? tamanho, bool? paisagem, JsonData jsonData) =>
{
    var documentoBytes = await jsonData.GetAndReadByteArrayAsync(file);
    var output = ImagePdf.ImagemToPdf(documentoBytes, tamanho, paisagem);
    return Results.File(output, "application/octet-stream", "File.pdf");
});

//endpoint que recebe a imagem como arquivo (IFormFile) e faz a transformação para PDF

app.MapPost("/api/ImagePdfByFile", async Task<IResult> (HttpRequest request, string? tamanho, bool? paisagem) =>
{
    if (!request.HasFormContentType)
        return Results.BadRequest();

    var file = await request.ReadFormAsync();
    var formFile = file.Files;

    if (formFile != null)
    {
        var documentoBytes = await PdfTools.ObterArquivo(formFile);
        var output = ImagePdf.ImagemToPdf(documentoBytes, tamanho, paisagem);
        return Results.File(output, "application/octet-stream", "File.pdf");
    }
    return Results.BadRequest();

});

app.Run();