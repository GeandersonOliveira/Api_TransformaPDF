using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Kernel.Geom;

namespace Business
{
    public class ImagePdf
    {
        public static byte[] ImagemToPdf(byte[] file, string? tamanho, bool? paisagem) 
                                                                                      
        {
            using var stream = new MemoryStream();

            var imageData = ImageDataFactory.Create(file);

            using var pdfWriter = new PdfWriter(stream);
            using var pdfDocument = new PdfDocument(pdfWriter);
            var document = new Document(pdfDocument);

            var image = new Image(imageData);

            // Define tamanho de papel
            DefineTamanhoPapel(tamanho, pdfDocument, image);

            // Define orientação do papel
            AjustaOrientacao(paisagem, pdfDocument, image);

            // Adiciona imagem ao documento
            image.SetRelativePosition(0, 0, 0, 0);
            document.Add(image);

            document.Close();
            
            return stream.ToArray();

            }

        private static void DefineTamanhoPapel(string? tamanho, PdfDocument pdfDocument, Image image)
        {
            if (string.IsNullOrWhiteSpace(tamanho) || tamanho.ToUpper() == "AUTO")
            {
                AjustaTamanhoAuto(pdfDocument, image);
            }
            else
            {
                TamanhoPapel.TipoPapel(tamanho, pdfDocument);
            }
        }

        private static void AjustaTamanhoAuto(PdfDocument pdfDocument, Image image)
        {
            var tamanhos = new List<PageSize> { PageSize.A5, PageSize.A4, PageSize.A3, PageSize.A2, PageSize.A1, PageSize.A0 };

            foreach (var tamanho in tamanhos)
            {
                pdfDocument.SetDefaultPageSize(tamanho);

                if (image.GetImageScaledHeight() <= tamanho.GetHeight() && image.GetImageScaledWidth() <= tamanho.GetWidth())
                {
                    break; // Tamanho ideal encontrado
                }
            }
        }
        private static void AjustaOrientacao(bool? paisagem, PdfDocument pdfDocument, Image image)
        {
            var pageSize = pdfDocument.GetDefaultPageSize();

            if (paisagem == true || (image.GetImageWidth() > image.GetImageHeight()))
            {
                pdfDocument.SetDefaultPageSize(pageSize.Rotate());
            }

            if (image.GetImageScaledHeight() > pageSize.GetHeight() || image.GetImageScaledWidth() > pageSize.GetWidth())
            {
                image.SetAutoScale(true);
            }
        }

    }
}
