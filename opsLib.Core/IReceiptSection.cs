using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opsLib.Core
{
    public interface IReceiptSection
    {
        string TemplateName { get; set; }
    }

    public interface IReceiptSingleSection : IReceiptSection
    {
    }

    public interface IReceiptRepeatingSection : IReceiptSection
    {
    }

    public interface IReceiptHeader : IReceiptSingleSection
    {

    }

    public interface IReceiptBody : IReceiptRepeatingSection
    {

    }

    public interface IReceiptFooter : IReceiptSingleSection
    {

    }
}
