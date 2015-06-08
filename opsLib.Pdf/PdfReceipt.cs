using opsLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opsLib.Pdf
{
    public class PdfReceipt : IReceipt
    {
        public virtual IReceiptHeader Header { get; private set; }
        public virtual IReceiptBody Body{ get; private set; }
        public virtual IReceiptFooter Footer { get; private set; }
        public virtual IDictionary<string, object> Fields { get; private set; }
        public virtual ICollection<IDictionary<string, object>> DataSet { get; private set; }

        public PdfReceipt()
        {
            Header = new PdfReceiptHeader();
            Body = new PdfReceiptBody();
            Footer = new PdfReceiptFooter();
            Fields = new Dictionary<string, object>();
            DataSet = new List<IDictionary<string, object>>();
        }
    }
}
