using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace practicode2
{
    public class HtmlHelper
    {
        private readonly static HtmlHelper _instance= new HtmlHelper();
        public static HtmlHelper Instance => _instance;
        public string[] Tags { get; set; }
        public string[] HtmlVoidTags { get; set; }

        private HtmlHelper()
        {
            string tagsJason = File.ReadAllText("jsonFiles/HtmlTags.json");
            string VoidTagsJson= File.ReadAllText("jsonFiles/HtmlVoidTags.json");
            Tags = JsonSerializer.Deserialize<string[]>(tagsJason);
            HtmlVoidTags = JsonSerializer.Deserialize<string[]>(VoidTagsJson);
        }
    }
}
