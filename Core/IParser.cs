
using AngleSharp.Html.Dom;
using Myparser.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Parser.Core
{
    interface IParser<T> where T : class
    {
        Task<T> Parse(IHtmlDocument document);
    }
}
