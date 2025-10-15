using iText.Barcodes;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Layout;
using iText.Layout.Element;
using System;
using System.IO;


namespace PDFLibrary
{
    public class clsModifyPDF
    {
        private Document doc;
        private MemoryStream msFile;
        private PdfDocument srcDoc;
        private PdfDocument pdfDoc;
        Rectangle pageSize;

        public clsModifyPDF()
        {
            try
            {
                msFile = new MemoryStream();
                pageSize = new PageSize(600, 400);

                doc = new Document(pdfDoc, new PageSize(60, 140));
                doc.SetMargins(5, 5, 5, 5);

                srcDoc = new PdfDocument(new PdfReader(new MemoryStream()));
                pdfDoc = new PdfDocument(new PdfWriter(msFile));
                pdfDoc.SetDefaultPageSize(new PageSize(pageSize));
                pdfDoc.AddNewPage();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public clsModifyPDF(string sInputPDF)
        {
            try
            {
                msFile = new MemoryStream();
                srcDoc = new PdfDocument(new PdfReader(new MemoryStream(Convert.FromBase64String(sInputPDF))));
                pageSize = srcDoc.GetFirstPage().GetPageSize();

                pdfDoc = new PdfDocument(new PdfWriter(msFile));
                pdfDoc.SetDefaultPageSize(new PageSize(pageSize));
                pdfDoc.AddNewPage();

                doc = new Document(pdfDoc, new PageSize(60, 140));
                doc.SetMargins(5, 5, 5, 5);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ChangeTicketPayment()
        {
            try
            {
                Rectangle toMove = new Rectangle(0, 0, 600, 600);

                PdfFormXObject t_canvas1 = new PdfFormXObject(pageSize);
                PdfCanvas canvas1 = new PdfCanvas(t_canvas1, pdfDoc);

                PdfFormXObject pageXObject = srcDoc.GetFirstPage().CopyAsFormXObject(pdfDoc);

                canvas1.Rectangle(0, 0, pageSize.GetWidth(), pageSize.GetHeight());
                canvas1.Rectangle(toMove.GetLeft(), toMove.GetBottom(),
                        toMove.GetWidth(), toMove.GetHeight());
                canvas1.EoClip();
                canvas1.EndPath();
                canvas1.AddXObjectAt(pageXObject, 0, 0);

                PdfFormXObject t_canvas2 = new PdfFormXObject(pageSize);
                PdfCanvas canvas2 = new PdfCanvas(t_canvas2, pdfDoc);
                PdfFont fuente = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.COURIER);


                canvas2.Rectangle(toMove.GetLeft(), toMove.GetBottom(),
                        toMove.GetWidth(), toMove.GetHeight());
                canvas2.SetFontAndSize(fuente, 8);
                canvas2.SetStrokeColor(ColorConstants.BLACK);
                canvas2.SetTextRenderingMode(PdfCanvasConstants.TextRenderingMode.STROKE);
                canvas2.Clip();
                canvas2.EndPath();
                canvas2.AddXObjectAt(pageXObject, 0, 0);

                pageSize = pdfDoc.GetFirstPage().GetPageSize();

                for (int p = 1; p <= pdfDoc.GetNumberOfPages(); p++)
                {
                    PdfPage page = pdfDoc.GetPage(p);

                    int rotate = page.GetRotation();
                    if (rotate == 0)
                    {
                        page.SetRotation(90);
                    }
                    else
                    {
                        page.SetRotation((rotate + 90) % 360);
                    }

                }

                //Adicionar contenido de la Boleta
                Image image = new Image(t_canvas2);
                image.SetFixedPosition(150, 20);
                image.SetRotationAngle(Math.PI / 2f);
                image.ScaleToFit(700, 600);
                doc.Add(image);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AddImage(string sPath, float fX, float fY, float fWidth, float fHeight, bool rotation)
        {
            try
            {
                ImageData imageData = ImageDataFactory.Create(sPath);
                Image img = new Image(imageData);
                img.SetFixedPosition(fX, fY);
                img.ScaleToFit(fWidth, fHeight);
                if (rotation) img.SetRotationAngle(Math.PI / 2f);
                doc.Add(img);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AddQRCode(string sText, float fX, float fY, float fWidth, float fHeight, bool rotation)
        {
            try
            {
                BarcodeQRCode qrCode = new BarcodeQRCode(sText);
                Rectangle rect = qrCode.GetBarcodeSize();
                PdfFormXObject qrCodeObject = new PdfFormXObject(new Rectangle(rect.GetWidth(), rect.GetHeight() + 10));
                PdfCanvas pdfCanvas = new PdfCanvas(qrCodeObject, pdfDoc);
                new Canvas(pdfCanvas, new Rectangle(rect.GetWidth(), rect.GetHeight() + 10));
                qrCode.PlaceBarcode(pdfCanvas, ColorConstants.BLACK);
                Image qrCodeImage = new Image(qrCodeObject);
                qrCodeImage.SetFixedPosition(fX, fY);
                qrCodeImage.ScaleToFit(fWidth, fHeight);
                if (rotation) qrCodeImage.SetRotationAngle(Math.PI / 2f);
                doc.Add(qrCodeImage);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public void AddText(string sText, float fX, float fY, float fW, int iFontSize, bool rotation)
        {
            try
            {
                PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                Paragraph p = new Paragraph(sText).SetFont(font).SetFontSize(iFontSize).SetFixedPosition(fX, fY, fW);
                if (rotation) p.SetRotationAngle(Math.PI / 2f);
                doc.Add(p);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AddTextBold(string sText, float fX, float fY, float fW, int iFontSize, bool rotation)
        {
            try
            {
                PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                Paragraph p = new Paragraph(sText).SetFont(font).SetFontSize(iFontSize).SetFixedPosition(fX, fY, fW);
                if (rotation) p.SetRotationAngle(Math.PI / 2f);
                doc.Add(p);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void AddTextBoldUnderline(string sText, float fX, float fY, float fW, int iFontSize, bool rotation)
        {
            try
            {
                PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                Paragraph p = new Paragraph(sText).SetFont(font).SetFontSize(iFontSize).SetFixedPosition(fX, fY, fW);
                if (rotation) p.SetRotationAngle(Math.PI / 2f);
                p.SetUnderline();
                doc.Add(p);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetBase64PDF()
        {
            string strBase64 = "";
            try
            {
                doc.Close();
                pdfDoc.Close();
                if (srcDoc != null) srcDoc.Close();
                if (msFile != null)
                {
                    byte[] byteFile = msFile.ToArray();
                    strBase64 = Convert.ToBase64String(byteFile);
                    msFile.Close();
                    msFile.Dispose();
                }

                return strBase64;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Close()
        {
            doc.Close();
            pdfDoc.Close();
            if (srcDoc != null) srcDoc.Close();
            if (msFile != null)
            {
                msFile.Close();
                msFile.Dispose();
            }
        }

    }
}
