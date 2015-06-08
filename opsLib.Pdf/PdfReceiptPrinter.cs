using opsLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp;
using iTextSharp.text;
using System.IO;
using iTextSharp.text.pdf;
using PdfPrintingNet;
using System.Drawing.Printing;

namespace opsLib.Pdf
{
    public class PdfReceiptPrinter : IReceiptPrinter
    {
        public async Task Print(IReceipt receipt, IReceiptPrinterConfiguration configuration)
        {
            const float InchMM = 25.4f;
            const float WidthMM = 60f;
            var width = (int)(WidthMM / InchMM * 100.0f);

            var printerSettings = new PrinterSettings();
            using (var stream = new MemoryStream())
            {
                var docHeight = await PrintToStream(stream, receipt, configuration);
                var printLibrary = new PdfPrint("Pasamonte Labs", "");
                printLibrary.PrinterName = configuration.PrinterName;
                printLibrary.Scale = PdfPrint.ScaleTypes.Shrink;
                printLibrary.Collate = false;
                printLibrary.Copies = 1;
                printLibrary.DuplexType = Duplex.Simplex;
                printLibrary.IsLandscape = false;
                var height = (int)(docHeight);
                var paperSize = new PaperSize("Pasamonte Custom", width, height);
                printLibrary.PaperSize = paperSize;
                var result = PdfPrint.Status.OK;
                result = printLibrary.Print(stream.ToArray());
            }
            await Task.Delay(10);

        }

        public async Task PrintToFile(IReceipt receipt, string filePath)
        {
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await PrintToStream(stream, receipt, null);
                stream.Close();
            }
        }

        virtual protected async Task<float> PrintToStream(Stream outputStream, IReceipt receipt, IReceiptPrinterConfiguration configuration)
        {
            // get input document
            var renderedPdfHeader = await RenderSection(receipt.Header, receipt);
            var headerPdfReader = new PdfReader(renderedPdfHeader);
            var renderedPdfFooter = receipt.Footer.TemplateName == null ? null : await RenderSection(receipt.Footer, receipt);
            var footerPdfReader = renderedPdfFooter == null ? null : new PdfReader(renderedPdfFooter);
            var bodyPdfReader = receipt.Body.TemplateName == null ? null : new PdfReader(receipt.Body.TemplateName);

            var pdfDoc = new Document();
            // create the output writer 
            var outputWriter =
                PdfWriter.GetInstance(pdfDoc, outputStream);
            pdfDoc.Open();

            var pdfCb = outputWriter.DirectContent;

            var topOffset = 1f;

            var franjasDetalle = receipt.DataSet == null ? 0 : receipt.DataSet.Count;
            var bodyBandSize = bodyPdfReader == null ? null : bodyPdfReader.GetPageSizeWithRotation(1);
            var bodyHeight = bodyPdfReader == null ? 0 : franjasDetalle * bodyBandSize.Height;

            var headerPageSize = headerPdfReader.GetPageSizeWithRotation(1);
            var footerPageSize = footerPdfReader == null ? null : footerPdfReader.GetPageSizeWithRotation(1);
            var footerPageHeight = footerPageSize == null ? 0 : footerPageSize.Height;
            // calcular altura del body

            var outputPageSize = new Rectangle(0, 0, headerPageSize.Width, headerPageSize.Height + footerPageHeight + bodyHeight);
            pdfDoc.SetPageSize(outputPageSize);
            pdfDoc.NewPage();
            if (footerPdfReader != null)
            {
                var footerPage = outputWriter.GetImportedPage(footerPdfReader, 1);
                pdfCb.AddTemplate(footerPage, 0, topOffset);
                topOffset += footerPageSize.Height;
            }
            // render del detalle
            if (receipt.DataSet != null && bodyPdfReader != null)
                foreach (var data in receipt.DataSet.Reverse())
                {
                    var outStream = new MemoryStream();
                    if (bodyPdfReader == null)
                        bodyPdfReader = new PdfReader(receipt.Body.TemplateName);
                    var stamper = new PdfStamper(bodyPdfReader, outStream) { FormFlattening = true };
                    var form = stamper.AcroFields;
                    var fieldKeys = form.Fields.Keys;
                    foreach (var pair in data)
                    {
                        if (fieldKeys.Any(f => f == pair.Key))
                        {
                            form.SetField(pair.Key, pair.Value.ToString());
                        }
                    }
                    stamper.Close();
                    var tmpReader = new PdfReader(outStream.ToArray());
                    var tmpPage = outputWriter.GetImportedPage(tmpReader, 1);
                    pdfCb.AddTemplate(tmpPage, 0, topOffset);
                    bodyPdfReader.Close();
                    bodyPdfReader = null;
                    topOffset += bodyBandSize.Height;
                }
            var headerPage = outputWriter.GetImportedPage(headerPdfReader, 1);
            pdfCb.AddTemplate(headerPage, 0, topOffset);
            pdfDoc.Close();
            topOffset += headerPageSize.Height;
            return topOffset;
        }

        virtual protected async Task<byte[]> RenderSection(IReceiptSingleSection section, IReceipt receipt)
        {
            var reader = new PdfReader(section.TemplateName);
            var outStream = new MemoryStream();
            var stamper = new PdfStamper(reader, outStream) { FormFlattening = true };

            var form = stamper.AcroFields;
            //form.GenerateAppearances = true; //Added this line, fixed my problem
            var fieldKeys = form.Fields.Keys;

            if (receipt.Fields != null)
                foreach (var pair in receipt.Fields)
                {
                    if (fieldKeys.Any(f => f == pair.Key))
                    {
                        form.SetField(pair.Key, pair.Value.ToString());
                    }
                }
            stamper.Close();
            reader.Close();

            return outStream.ToArray();
        }
    }
}
