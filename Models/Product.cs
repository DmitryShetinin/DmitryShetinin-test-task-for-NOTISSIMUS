using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Myparser.Models
{
    class Product
    {
        public string name { get; set; }
        public decimal price { get; set; }
        public decimal oldPrice { get; set; }
        public string rating { get; set; }
        public string article { get; set; }
        public string volume { get; set; }
        public string region { get; set; }
        public string url { get; set; }
        public List<string> pictures { get; set; } = new List<string>();

    }
}
