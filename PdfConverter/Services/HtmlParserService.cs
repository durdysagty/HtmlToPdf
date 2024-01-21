using HtmlAgilityPack;
using PdfConverter.Services.Interfaces;

namespace PdfConverter.Services
{
    public class HtmlParserService : IHtmlParser
    {
        public bool IsValid(string content)
        {
            HtmlDocument document = new();
            document.LoadHtml(content);
            return !document.ParseErrors.Any();
        }
    }
}
