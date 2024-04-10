
using AngleSharp.Html.Dom;

namespace Parser.Core
{
    interface IParser<T> where T : class
    {
        Task<T> Parse(IHtmlDocument document);
    }
}
