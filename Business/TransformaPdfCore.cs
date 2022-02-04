using Infrastructure;
using iText.Kernel.Pdf;

namespace Business
{
    public class TransformaPdfCore
    {
        public static byte[] PdfConcatenation(IEnumerable<byte[]> files)
        {
            using (var outputMemoryStream = new MemoryStream())
            using (var outputPdfWriter = new PdfWriter(outputMemoryStream))
            using (var outputPdfDocument = new PdfDocument(outputPdfWriter))
            {
                foreach (var file in files)
                {
                    using (var fileMemoryStream = new MemoryStream(file))
                    using (var filePdfReader = new PdfReader(fileMemoryStream))
                    {
                        // ignorando as restrições de segurança do documento
                        // https://kb.itextpdf.com/home/it7kb/faq/how-to-read-pdfs-created-with-an-unknown-random-owner-password
                        filePdfReader.SetUnethicalReading(true);
                        using (var filePdfDocument = new PdfDocument(filePdfReader))
                        {
                            filePdfDocument.CopyPagesTo(1, filePdfDocument.GetNumberOfPages(), outputPdfDocument);
                            filePdfDocument.Close();
                        }
                        filePdfReader.Close();
                        fileMemoryStream.Close();
                    }
                }

                outputPdfDocument.Close();
                outputPdfWriter.Close();
                outputMemoryStream.Close();

                return outputMemoryStream.ToArray();
            }
        }

        private readonly JsonData jsonData;
        public TransformaPdfCore(JsonData json)
        {
            jsonData = json;
        }
        public async Task<byte[]> PdfConcatenation(IEnumerable<string> urls)
        {

            List<byte[]> arquivos = new List<byte[]>();
            try
            {
                //JsonData x = new JsonData(...);
                foreach (var url in urls)

                    arquivos.Add(await jsonData.GetAndReadByteArrayAsync(url));
            }
            catch (Exception)
            {
                throw new Exception($"Não foi possível obter o documento.");
            }

            var arquivoFinal = PdfConcatenation(arquivos);

            return arquivoFinal;
        }

        public async Task<byte[]> ConcatenarUrlEArquivo(string url, byte[] documentoMetadados)
        {
            byte[] documentoFromUrl;
            try
            {
                documentoFromUrl = await jsonData.GetAndReadByteArrayAsync(url);
            }
            catch (Exception)
            {
                throw new Exception($"Não foi possível obter o documento.");
            }

            var arquivoFinal = PdfConcatenation(new List<byte[]>() { documentoFromUrl, documentoMetadados });

            return arquivoFinal;
        }

    }
    
}
