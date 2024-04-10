using AngleSharp.Io;
using System.IO.Compression;
using System.Net;

namespace Parser.Core
{
    class HtmlLoader
    {
        readonly HttpClient client;
        readonly string url;

        public HtmlLoader(IParserSettings settings)
        {
            client = new HttpClient();
            url = $"{settings.BaseUrl}{settings.Prefix}/?setVisitorCityId={settings.CityId}";
        }

        public async Task<string> GetSourceByPageId(int id)
        {

            var currentUrl = url.Replace("{CurrentId}", id.ToString());
            var response = await client.GetAsync(currentUrl);


            string source = null;


            if (response.Content.Headers.ContentEncoding.Contains("gzip"))
            {
                using (Stream contentStream = await response.Content.ReadAsStreamAsync())
                {
                    using (MemoryStream decompressedStream = new MemoryStream())
                    {
                        using (GZipStream gzip = new GZipStream(contentStream, CompressionMode.Decompress))
                        {
                            await gzip.CopyToAsync(decompressedStream);
                        }

                        decompressedStream.Seek(0, SeekOrigin.Begin);

                        using (StreamReader reader = new StreamReader(decompressedStream))
                        {
                            source = await reader.ReadToEndAsync();
                  
                        }
                    }
                }
            }
            else // Если содержимое не было сжато, просто читаем его как обычно
            {
                 source = await response.Content.ReadAsStringAsync();
            }



                return source;
        }
    }
}
