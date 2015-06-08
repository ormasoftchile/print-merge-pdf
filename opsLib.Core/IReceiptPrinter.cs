using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opsLib.Core
{
    public interface IReceiptPrinter
    {
        Task Print(IReceipt receipt, IReceiptPrinterConfiguration configuration);
        Task PrintToFile(IReceipt receipt, string filePath);
    }
}
