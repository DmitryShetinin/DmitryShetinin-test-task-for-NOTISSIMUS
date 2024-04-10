using AngleSharp.Html.Dom;
using Myparser.Models;

namespace Parser.Core.Habra
{
    class HabraParser : IParser<List<Product>>
    {

        public List<string> FilterElementsByClass(IHtmlDocument document, string tagName, string className)
        {
            return document.QuerySelectorAll(tagName)
                           .Where(item => item.ClassName != null && item.ClassName.Contains(className))
                           .Select(item => item.TextContent)
                           .Select(line => line.Trim().Replace("\n", ""))
                           .ToList();
        }


        public List<decimal> FilterByPrices(IHtmlDocument document, string tagName)
        {
            return document.QuerySelectorAll(".snippet-price")
                           .Select(element => element.QuerySelector(tagName))
                           .Select(snippetPriceDiscount => snippetPriceDiscount != null ? snippetPriceDiscount.TextContent
                           .Replace("₽", "")
                           .Trim()
                           .Replace("\n", "") : "0")
                           .Select(decimal.Parse)
                           .ToList();
        }



        List<Product> IParser<List<Product>>.Parse(IHtmlDocument document)
        {
            var list = new List<Product>();

            var articles = FilterElementsByClass(document, "div", "snippet-article js-copy-article");
            var names = FilterElementsByClass(document, "a", "snippet-name js-dy-slot-click");
            var ratings = FilterElementsByClass(document, "span", "snippet-star__value");
            var prices = FilterByPrices(document, ".snippet-price__total");
            var oldPrices = FilterByPrices(document, ".snippet-price__discount > .snippet-price__old > span");
            var region = document.QuerySelector(".location__current").TextContent.Replace("\n","").Trim();


            var volumes = document.QuerySelectorAll(".snippet-description");
            var urls = document.QuerySelectorAll(".swiper-container").Select(link => link.GetAttribute("Href")).ToList();
            var pictures = document.QuerySelectorAll("div").Where(item => item.ClassName == "swiper-wrapper" && item.ClassName != null).ToList();




            for (int i = 0; i < articles.Count; i++)
            {
                var product = new Product();

                product.article = articles[i];
                product.name = names[i];
                product.rating = ratings[i];
                product.price = prices[i];
                product.oldPrice = oldPrices[i];
                product.region = region;
                product.url = urls[i];
                if (volumes[i].LastElementChild != null)
                {
                    product.volume = volumes[i].LastElementChild.TextContent;
                }
                for (int j = 0; j < pictures[i].ChildElementCount; j++)
                {
                    product.pictures.Add(pictures[i].Children[j].LastElementChild.GetAttribute("src"));
                }
                list.Add(product);
            }

            return list;
        }

       
    }
}
