using opsLib.Pdf;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opsConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var receipt = new PdfReceipt();
            receipt.Header.TemplateName = "headerTemplate.pdf";
            receipt.Footer.TemplateName = "footerTemplate.pdf";
            receipt.Body.TemplateName = "bodyTemplate.pdf";
            receipt.Fields["titulo"] = "prueba de impresion";
            receipt.Fields["fecha"] = "10 de diciembre de 2014";
            receipt.Fields["paciente"] = "Carlos Truchermann Martinez";
            receipt.Fields["centro"] = "Donde Lo Prado";
            // dataset
            var random = new Random();
            for (var i = 1; i <= 15; i++)
            {
                var entrada = new Dictionary<string, object>();
                entrada["nombre"] = "* Este es un item #" + i.ToString();
                entrada["cantidad"] = random.Next(100);
                entrada["indicaciones"] = "1 comprimido al desayumo, 1 al almuerzo y 1 con la cena.";
                receipt.DataSet.Add(entrada);
            }
            var receiptPrinter = new PdfReceiptPrinter();
            var receiptPrinterConfiguration = new PdfReceiptPrinterConfiguration()
            {
                PrinterName = "Zebra KR203"
            };
            receiptPrinter.Print(receipt, receiptPrinterConfiguration);
            //receiptPrinter.PrintToFile(receipt, "a.pdf");
        }
    }
}
