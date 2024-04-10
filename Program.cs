using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Myparser.Models;
using Newtonsoft.Json;
using Parser.Core;
using Parser.Core.Habra;

using Formatting = Newtonsoft.Json.Formatting;

class Program
{
    static ParserWorker<List<Product>> parser;
    readonly static string url = "https://simplewine.ru/catalog/shampanskoe_i_igristoe_vino/";
    static Dictionary<string, string> cityDictionary; 
    static async Task Main()
    {
        var documentTask = Task.Run(() => GetPageBaseAsync());
        var cityInfoTask = Task.Run(() => GetInfoAboutCity());
        parser = new ParserWorker<List<Product>>(
                new HabraParser());

        parser.OnCompleted += Parser_OnCompleted;
        parser.OnNewData += Parser_OnNewData;



       
        Console.Write("Введите с какой страницы парсить: ");
        int MinValue = Convert.ToInt32(Console.ReadLine());
   
        Console.Write("Введите до какой страницы парсить: ");
        int MaxValue = Convert.ToInt32(Console.ReadLine());


        Console.WriteLine("Введите id города которые вы хотите распарсить(по умолчанию Москва - 1): ");
        await Task.WhenAll(documentTask, cityInfoTask);



        foreach (var pair in cityDictionary)
        {
            Console.WriteLine($"{pair.Key} - {pair.Value}");
        }




        int CityId = Convert.ToInt32(Console.ReadLine());
        parser.Settings = new HabraSettings(MinValue, MaxValue, CityId);
        await parser.StartAsync();


    }





    private static async Task<IHtmlDocument> GetPageBaseAsync()
    {
        
        
        HttpClient client = new HttpClient();
        var response = await client.GetAsync(url);
        var source = await response.Content.ReadAsStringAsync(); 
        var domParser = new HtmlParser();
        var document = await domParser.ParseDocumentAsync(source);
        client.Dispose();

        return document;

       

    } 


    private static async Task GetInfoAboutCity()
    {

        var document = await GetPageBaseAsync();
        var s = document.QuerySelectorAll("a")
                        .Where(item => item.ClassName != null && item.ClassName.Contains("location__link"));

        cityDictionary = s.ToDictionary(
                    item => item.GetAttribute("href").Substring(item.GetAttribute("href").IndexOf('=') + 1),
                    item => item.TextContent.Trim());


    }

    private static void Parser_OnNewData(object arg1, List<Product> list)
    {
        string json = JsonConvert.SerializeObject(list, Formatting.Indented);
        Console.WriteLine(json);
        File.WriteAllText("Products.json", json);
        

    }

    private static void Parser_OnCompleted(object obj)
    {
        Console.WriteLine("Ok");
        Console.WriteLine("JSON строка сохранена в файле Products.json");
    }

    



}
