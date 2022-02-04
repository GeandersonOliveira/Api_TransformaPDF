using Microsoft.AspNetCore.Http;

namespace Infrastructure
{
    public class PdfTools
    {
        public static async Task<IEnumerable<byte[]>> ObterArquivos(IFormFileCollection arquivos)
        {
            var arquivosBytes = new List<byte[]>();

            foreach (var arquivo in arquivos)
            {
                if (arquivo.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await arquivo.CopyToAsync(memoryStream);
                        arquivosBytes.Add(memoryStream.ToArray());
                    }
                }
            }

            return arquivosBytes;
        }
        public static async Task<byte[]> ObterArquivo(IFormFileCollection arquivos)
        {
            byte[]? arquivoBytes = null;

            foreach (var arquivo in arquivos)
            {
                if (arquivo.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await arquivo.CopyToAsync(memoryStream);
                        arquivoBytes = memoryStream.ToArray();
                    }
                }
            }

            return arquivoBytes;
        }
    }
}
