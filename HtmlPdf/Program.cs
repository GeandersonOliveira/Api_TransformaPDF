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
    var output = TransformaToPdf.HtmlPdf(html);
    return Results.Ok(new ApiResponse<byte[]>(200, "success", output));
});

app.MapGet("/health/live", () =>
{
    return Results.Ok();
});

app.Run();

internal record TransformaToPdf
{
    public static byte[] HtmlPdf(string html)
    {
        using var output = new MemoryStream();
        HtmlConverter.ConvertToPdf(html, output);
        var array = output.ToArray();
        output.Close();
        return array;
    }
}
class ApiResponse<T>
{
    public int StatusCode { get; set; }
    public string Message { get; set; }
    public string StackTrace { get; set; }
    public T Data { get; set; }


    public ApiResponse(ApiResponse<object> errorResponse)
    {
        StatusCode = errorResponse.StatusCode;
        Message = errorResponse.Message;
    }

    public ApiResponse(int statusCode, string message = default, T data = default, string stackTrace = default)
    {
        StatusCode = statusCode;
        Message = message ?? GetDefaultMessageForStatusCode(statusCode);
        Data = data;
        StackTrace = stackTrace;
    }

    private string GetDefaultMessageForStatusCode(int statusCode)
    {
        switch (statusCode)
        {
            case 400:
                return "Bad request";
            case 401:
                return "Authorized";
            case 404:
                return "Resource not found";
            case 500:
                return "Internal server error";
            default:
                return null;
        }
    }

}