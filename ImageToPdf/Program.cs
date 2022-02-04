//prototipo para Image2PDF. Cria documentos com tamanho padrão, ou de acordo com o tamanho da imagem. Mas este sem tamanho padronizado.
//Usar Projeto ImageToPdf_v2

using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Kernel.Geom;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddScoped<JsonData>();
builder.Services.AddScoped<PdfTools>();



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


app.MapPost("/api/ImagePdf", async Task<IResult> (string url, string? tamanho, bool? paisagem, JsonData jsonData) =>
{
    var documentoBytes = await jsonData.GetAndReadByteArrayAsync(url);
    var output = ImagePdf.ImagemToPdf(documentoBytes, tamanho, paisagem);
    return Results.File(output, "application/octet-stream", "File.pdf");
});

app.MapPost("/api/ImagePdfByFile", async Task<IResult>(HttpRequest request, string? tamanho, bool? paisagem) =>
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

public class ImagePdf
{
    public static byte[] ImagemToPdf(byte[] file, string? tamanho, bool? paisagem)
    {
        using (MemoryStream stream = new MemoryStream())
        {
            //Initialize the PDF document object.

            ImageData imageData = ImageDataFactory.Create(file);
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(stream));
            Document document = new Document(pdfDocument);

            if (tamanho != null) { pdfDocument = TamanhoPapel.TipoPapel(tamanho.ToUpper(), pdfDocument); } //define o tipo de Papel

            Image image = new Image(imageData);

            if (image.GetImageHeight() >= image.GetImageWidth() || paisagem == false)

            {
                if (paisagem == true)
                {
                    var pdf = pdfDocument.GetDefaultPageSize();             // pegar TipoPapel para mudar orientação para paisagem
                    pdfDocument.SetDefaultPageSize(pdf.Rotate());         //mudar orientação para paisagem

                    if (image.GetImageScaledHeight() >= pdfDocument.GetDefaultPageSize().GetHeight())
                    {
                        image.SetAutoScale(true);
                    }
                }
                else

                {
                    if (image.GetImageScaledHeight() >= pdfDocument.GetDefaultPageSize().GetHeight()) { image.SetAutoScale(true); } //Auto Resize

                }

            }
            else
            {
                var pdf = pdfDocument.GetDefaultPageSize();             // pegar TipoPapel para mudar orientação para paisagem
                pdfDocument.SetDefaultPageSize(pdf.Rotate());           //mudar orientação para paisagem

                if (image.GetImageScaledHeight() >= pdfDocument.GetDefaultPageSize().GetHeight() - 20 || image.GetImageScaledWidth() >= pdfDocument.GetDefaultPageSize().GetWidth() - 20)
                {
                    image.SetAutoScale(true);
                }

            }

            //image.SetHorizontalAlignment(HorizontalAlignment.CENTER);
            image.SetRelativePosition(0, 0, 0, 0);
            document.Add(image);
            pdfDocument.Close();

            return stream.ToArray();

        }

    }



}
public class TamanhoPapel
{
    public static PdfDocument TipoPapel(string? tamanho, PdfDocument pdfdoc)

    {
        switch (tamanho)
        {
            case "A0":
                pdfdoc.SetDefaultPageSize(PageSize.A0);
                break;

            case "A1":
                pdfdoc.SetDefaultPageSize(PageSize.A1);
                break;

            case "A2":
                pdfdoc.SetDefaultPageSize(PageSize.A2);
                break;

            case "A3":
                pdfdoc.SetDefaultPageSize(PageSize.A3);
                break;

            case "A4":
                pdfdoc.SetDefaultPageSize(PageSize.A4);
                break;

            case "A5":
                pdfdoc.SetDefaultPageSize(PageSize.A5);
                break;

            default:
                pdfdoc.SetDefaultPageSize(PageSize.A4);
                break;
        }
        return pdfdoc;
    }
}