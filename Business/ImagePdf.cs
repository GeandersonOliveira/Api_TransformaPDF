using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Kernel.Geom;

namespace Business
{
    public class ImagePdf
    {
        public static byte[] ImagemToPdf(byte[] file, string? tamanho, bool? paisagem) // metódo que usa itext para transformar e manipular o pdf
                                                                                       // parametros : file => a imagem | tamanho => tipo de papel | paisagem => orientação do papel
        {
            using (MemoryStream stream = new MemoryStream())
            {
                //Initialize the PDF document object.            
                ImageData imageData = ImageDataFactory.Create(file);
                PdfDocument pdfDocument = new PdfDocument(new PdfWriter(stream));
                Document document = new Document(pdfDocument);

                Image image = new Image(imageData);

                if (tamanho == null || tamanho.ToUpper() != "AUTO") //tamanho (tipo de papel, pode vir padronizado (A4,A5 ...)) ou Tipo auto, entao a partir da imagem identifica o padrao de papel ideal
                {
                    document = new Document(pdfDocument);
                    pdfDocument = TamanhoPapel.TipoPapel(tamanho, pdfDocument); //define o tipo de Papel que foi Pedido
                }
                else
                {
                    if (tamanho.ToUpper() == "AUTO") // implementa o tipo de papel, caso for pedido tamanho auto

                    {

                        using (MemoryStream memoryStream = new MemoryStream())
                        {

                            PdfDocument pdfdoc = new PdfDocument(new PdfWriter(memoryStream)); //pdfdoc variavel para comparar com o tamanho da imagem, até chegar no padrao ideal, começando com tipo A5.

                            pdfdoc.SetDefaultPageSize(PageSize.A5);
                            pdfDocument.SetDefaultPageSize(PageSize.A4);

                            //Se o tamanho da imagem for maior/igual que uma pagina A5 => documento final será A4 (mínimo)

                            if (image.GetImageScaledHeight() >= pdfdoc.GetDefaultPageSize().GetHeight() && image.GetImageScaledWidth() >= pdfdoc.GetDefaultPageSize().GetWidth())
                            {
                                pdfDocument.SetDefaultPageSize(PageSize.A4);
                                pdfdoc.SetDefaultPageSize(PageSize.A4);

                                //se o tamanho da imagem for maior/igual que pagina A4 => documento final será A3

                                if (image.GetImageScaledHeight() > pdfdoc.GetDefaultPageSize().GetHeight() && image.GetImageScaledWidth() > pdfdoc.GetDefaultPageSize().GetWidth())
                                {
                                    pdfDocument.SetDefaultPageSize(PageSize.A3);
                                    pdfdoc.SetDefaultPageSize(PageSize.A3);

                                    //se o tamanho da imagem for maior/igual que pagina A3 => documento final será A2

                                    if (image.GetImageScaledHeight() > pdfdoc.GetDefaultPageSize().GetHeight() && image.GetImageScaledWidth() > pdfdoc.GetDefaultPageSize().GetWidth())
                                    {
                                        pdfDocument.SetDefaultPageSize(PageSize.A2);
                                        pdfdoc.SetDefaultPageSize(PageSize.A2);

                                        //se o tamanho da imagem for maior/igual que pagina A2 => documento final será A1


                                        if (image.GetImageScaledHeight() > pdfdoc.GetDefaultPageSize().GetHeight() && image.GetImageScaledWidth() > pdfdoc.GetDefaultPageSize().GetWidth())
                                        {
                                            pdfDocument.SetDefaultPageSize(PageSize.A1);
                                            pdfdoc.SetDefaultPageSize(PageSize.A1);

                                            //se o tamanho da imagem for maior/igual que pagina A1 => documento final será A0


                                            if (image.GetImageScaledHeight() > pdfdoc.GetDefaultPageSize().GetHeight() && image.GetImageScaledWidth() > pdfdoc.GetDefaultPageSize().GetWidth())
                                            {
                                                pdfDocument.SetDefaultPageSize(PageSize.A0);
                                                pdfdoc.SetDefaultPageSize(PageSize.A0);

                                            } //Tipo A0 é o padrão de tamanho máximo

                                        }

                                    }
                                }
                            }
                            pdfdoc.Close(); //encerra o pdfdoc, pois só serve para fazer as comparações
                            memoryStream.Close();
                        }

                    }
                }

                if (image.GetImageHeight() >= image.GetImageWidth() || paisagem == false) //condicional para definir a orientação do documento, a partir da imagem, ou  se foi definido no parametro paisagem

                {
                    if (paisagem == true) // se paisagem for true, independente da imagem , o documento final será paisagem
                    {
                        var pdf = pdfDocument.GetDefaultPageSize();             // pegar TipoPapel para mudar orientação para paisagem
                        pdfDocument.SetDefaultPageSize(pdf.Rotate());         //mudar orientação para paisagem

                        if (image.GetImageScaledHeight() >= pdfDocument.GetDefaultPageSize().GetHeight())
                        {
                            image.SetAutoScale(true); // Metodo do itext que ajusta a imagem, caso ela ultrapasse os limites do papel
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
                image.SetRelativePosition(0, 0, 0, 0); //seta a posição da imagem no documento ao canto superior esquerdo
                document.Add(image);
                pdfDocument.Close();

                return stream.ToArray();

            }

        }
    }
}