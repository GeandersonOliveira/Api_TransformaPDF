using Microsoft.AspNetCore.Mvc;
using iText.Html2pdf;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors(options =>
            options.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());

app.MapPost("/api/HtmlToPdf", ([FromBody] string html) => //A string deve ser em formato JSON. Caso contrario retornara 400.
{
    if (string.IsNullOrWhiteSpace(html))
        return Results.BadRequest(new ApiResponse<string>(400, "HTML input is empty"));
    try
    {
        var output = TransformaToPdf.HtmlPdf(html);
        return Results.Ok(new ApiResponse<byte[]>(200, "success", output));
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.MapGet("/health/live", () =>
{
    return Results.Ok("API is live and healthy");
});

app.Run();

internal record TransformaToPdf
{
    public static byte[] HtmlPdf(string html)
    {
        using var output = new MemoryStream();
        HtmlConverter.ConvertToPdf(html, output);
        return output.ToArray();
    }
}
internal class ApiResponse<T>
{
    public int StatusCode { get; set; }
    public string Message { get; set; }
    public string StackTrace { get; set; }
    public T Data { get; set; }


    public ApiResponse(int statusCode, string message = default, T data = default, string stackTrace = default)
    {
        StatusCode = statusCode;
        Message = message ?? GetDefaultMessageForStatusCode(statusCode);
        Data = data;
        StackTrace = stackTrace;
    }

    private string GetDefaultMessageForStatusCode(int statusCode)
    {
        return statusCode switch
        {
            400 => "Bad request",
            401 => "Unauthorized",
            404 => "Resource not found",
            500 => "Internal server error",
            _ => "An error occurred"
        };
    }

}