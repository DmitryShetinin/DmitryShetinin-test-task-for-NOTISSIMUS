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
    readonly static string source1 = "https://simplewine.ru/catalog/shampanskoe_i_igristoe_vino/";
    static string[]? cityName;
    static string[]? cityId;
    static async Task Main()
    {
        parser = new ParserWorker<List<Product>>(
                new HabraParser());

        parser.OnCompleted += Parser_OnCompleted;
        parser.OnNewData += Parser_OnNewData;

        var documentTask = Task.Run(() => GetPageBase());
        var cityInfoTask = Task.Run(() => GetInfoAboutCity());


       
        Console.Write("Введите с какой страницы парсить: ");
        int MinValue = Convert.ToInt32(Console.ReadLine());
        Console.Write("Введите до какой страницы парсить: ");
        int MaxValue = Convert.ToInt32(Console.ReadLine());


        Console.WriteLine("Введите id города которые вы хотите распарсить(по умолчанию Москва - 1): ");
      

        await Task.WhenAll(documentTask, cityInfoTask);
        for(int i = 0; i < cityName.Length; i++)
        {
            Console.WriteLine($"{cityId[i]} - {cityName[i]}");
        }



        int CityId = Convert.ToInt32(Console.ReadLine());
        parser.Settings = new HabraSettings(MinValue, MaxValue, CityId);
        await parser.StartAsync();


    }


    private static async Task<IHtmlDocument> GetPageBase()
    {
        HttpClient client = new HttpClient();
        var response = await client.GetAsync(source1);
        var source2 = await response.Content.ReadAsStringAsync(); 
        var domParser = new HtmlParser();
        var document = await domParser.ParseDocumentAsync(source2);

        return document;
        
    } 


    private static async Task GetInfoAboutCity()
    {

        var document = await GetPageBase();
        var s = document.QuerySelectorAll("a")
                        .Where(item => item.ClassName != null && item.ClassName.Contains("location__link"))
                        .ToList();

        cityName = s.Select(item => item.TextContent.Trim()).ToArray();

        cityId = s.Select(item => item.GetAttribute("href"))
                      .Select(s => s.Substring(s.IndexOf('=') + 1))
                      .ToArray();
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
