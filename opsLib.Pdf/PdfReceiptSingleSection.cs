using opsLib.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opsLib.Pdf
{
    public class PdfReceiptSingleSection : IReceiptSingleSection
    {
        public IDictionary<string, object> Fields { get; set; }

        public string TemplateName { get; set; }

        #region MakeTemp
        protected virtual string MakeTemp()
        {
            return string.Format(@"c:\temp\{0}.pdf", Path.GetRandomFileName());
        }
        #endregion
    }
}
