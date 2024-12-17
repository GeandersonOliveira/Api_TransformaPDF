using iText.Kernel.Pdf;
using iText.Kernel.Geom;
namespace Business

{
    public class TamanhoPapel
    {
        public static PdfDocument TipoPapel(string? tamanho, PdfDocument pdfdoc)

        {
            if (!string.IsNullOrWhiteSpace(tamanho))
            {
                tamanho = tamanho.ToUpper();
                var tamanhos = new Dictionary<string, PageSize>
                {
                    { "A0", PageSize.A0 },
                    { "A1", PageSize.A1 },
                    { "A2", PageSize.A2 },
                    { "A3", PageSize.A3 },
                    { "A4", PageSize.A4 },
                    { "A5", PageSize.A5 },
                    { "B0", PageSize.B0 },
                    { "B1", PageSize.B1 },
                    { "B2", PageSize.B2 },
                    { "B3", PageSize.B3 },
                    { "EXECUTIVE", PageSize.EXECUTIVE },
                    { "LETTER", PageSize.LETTER }
                };

                if (tamanhos.TryGetValue(tamanho, out var pageSize))
                {
                    pdfdoc.SetDefaultPageSize(pageSize);
                }
                else
                {
                    pdfdoc.SetDefaultPageSize(PageSize.A4); // Padrão
                }
            }
            return pdfdoc;
        }
    }    
}
