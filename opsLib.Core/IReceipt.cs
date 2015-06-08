using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opsLib.Core
{
    public interface IReceipt
    {
        IReceiptHeader Header { get; }
        IReceiptBody Body { get; }
        IReceiptFooter Footer { get; }
        IDictionary<string, object> Fields { get; }
        ICollection<IDictionary<string, object>> DataSet { get; }
    }
}
