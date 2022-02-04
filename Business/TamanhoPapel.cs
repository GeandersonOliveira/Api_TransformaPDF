using iText.Kernel.Pdf;
using iText.Kernel.Geom;
namespace Business

{
    public class TamanhoPapel
    {
        public static PdfDocument TipoPapel(string? tamanho, PdfDocument pdfdoc) //metodo chamado caso o parametro tamanho seja passado

        {
            if (tamanho != null) { tamanho = tamanho.ToUpper(); }
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

                case "B0":
                    pdfdoc.SetDefaultPageSize(PageSize.B0);
                    break;

                case "B1":
                    pdfdoc.SetDefaultPageSize(PageSize.B1);
                    break;

                case "B2":
                    pdfdoc.SetDefaultPageSize(PageSize.B2);
                    break;

                case "B3":
                    pdfdoc.SetDefaultPageSize(PageSize.B3);
                    break;

                case "EXECUTIVE":
                    pdfdoc.SetDefaultPageSize(PageSize.EXECUTIVE);
                    break;

                case "LETTER":
                    pdfdoc.SetDefaultPageSize(PageSize.LETTER);
                    break;

                default:
                    pdfdoc.SetDefaultPageSize(PageSize.A4);
                    break;
            }
            return pdfdoc;
        }
    }    
}
