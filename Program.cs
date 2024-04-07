using Myparser.Models;
using Newtonsoft.Json;
using Parser.Core;
using Parser.Core.Habra;
using System.Xml;
using Formatting = Newtonsoft.Json.Formatting;

class Program
{
    static ParserWorker<List<Product>> parser;
    static async Task Main()
    {
        parser = new ParserWorker<List<Product>>(
                new HabraParser());

        parser.OnCompleted += Parser_OnCompleted;
        parser.OnNewData += Parser_OnNewData;

        Console.Write("Введите с какой страницы парсить: ");
        int MinValue = Convert.ToInt32(Console.ReadLine());
        Console.Write("Введите до какой страницы парсить: ");
        int MaxValue = Convert.ToInt32(Console.ReadLine());

        parser.Settings = new HabraSettings(MinValue, MaxValue);
        await parser.StartAsync();



    }

    private static void Parser_OnNewData(object arg1, List<Product> list)
    {
        string json = JsonConvert.SerializeObject(list, Formatting.Indented);
        Console.WriteLine(json);
        File.WriteAllText("Products.json", json);
        Console.WriteLine("JSON строка сохранена в файле Products.json");

    }

    private static void Parser_OnCompleted(object obj)
    {
        Console.WriteLine("Ok");
    }

    



}
