using Business;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

//config services
builder.Services.AddCors();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddScoped<JsonData>();

var app = builder.Build();

//config middlewares
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors(options =>
            options.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());
app.UseHttpsRedirection();

//verify health
app.MapGet("/health/live", () =>
{
    return Results.Ok();
});


//URI image to PDF
app.MapPost("/api/ImagePdf", async Task<IResult> (string file, string? tamanho, bool? paisagem, JsonData jsonData) =>
{
    if (string.IsNullOrWhiteSpace(file))
        return Results.BadRequest("File URL cannot be empty.");

    try
    {
        var documentoBytes = await jsonData.GetAndReadByteArrayAsync(file);
        var output = ImagePdf.ImagemToPdf(documentoBytes, tamanho, paisagem);
        return Results.File(output, "application/pdf", "File.pdf");
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

//image to PDF
app.MapPost("/api/ImagePdfByFile", async Task<IResult> (HttpRequest request, string? tamanho, bool? paisagem) =>
{
    if (!request.HasFormContentType)
        return Results.BadRequest("Invalid form content type.");

    var form = await request.ReadFormAsync();
    var formFile = form.Files;

    if (formFile == null)
        return Results.BadRequest("No file was provided.");

    try
    {
        var documentoBytes = await PdfTools.ObterArquivo(formFile);
        var output = ImagePdf.ImagemToPdf(documentoBytes, tamanho, paisagem);
        return Results.File(output, "application/pdf", "File.pdf");
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.Run();