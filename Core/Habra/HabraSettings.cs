
namespace Parser.Core.Habra
{
    class HabraSettings : IParserSettings
    {
        public HabraSettings()
        {
        }

        public HabraSettings(int start, int end, int cityid = 1)
        {
            StartPoint = start;
            EndPoint = end;
            CityId = cityid; 
        }

        public string BaseUrl { get; set; } = "https://simplewine.ru/catalog/shampanskoe_i_igristoe_vino/";

        public string Prefix { get; set; } = "page{CurrentId}";

        public int StartPoint { get; set; }

        public int EndPoint { get; set; }

        public int CityId { get; set; }
    }
}
