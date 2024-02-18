using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace practicode2
{
    public class Selector
    {
        public string TagName { get; set; }
        public string Id { get; set; }
        public List<string> Classes { get; set; }
        public Selector Child { get; set; }
        public Selector Parent { get; set; }

        public Selector()
        {
            Classes = new List<string>();
        }
        public static Selector ConvertQueryToSelector(string queryString)
        {
            var selectorParts = queryString.Split(' ');

            Selector rootSelector = new Selector();
            Selector currentSelector = rootSelector;

            foreach (var part in selectorParts)
            {
                string remainingPart = part;

                while (!string.IsNullOrEmpty(remainingPart))
                {
                    int endIndex = remainingPart.Substring(1).IndexOfAny(new char[] { '#', '.' })+1;
                    if (endIndex == 0)
                    {
                        endIndex = remainingPart.Length;
                    }
                    string selector = remainingPart.Substring(0, endIndex);
                    remainingPart = remainingPart.Substring(endIndex);
                    if (selector.StartsWith("#"))
                    {
                        currentSelector.Id = selector.Substring(1);
                    }
                    else if (selector.StartsWith("."))
                    {
                        currentSelector.Classes.Add(selector.Substring(1));
                    }
                    else if (HtmlHelper.Instance.Tags.Contains(selector))
                    {
                        currentSelector.TagName = selector;
                    }
                }

                Selector newSelector = new Selector();
                currentSelector.Child = newSelector;
                newSelector.Parent = currentSelector;
                currentSelector = newSelector;
            }

            // Remove the last empty child selector
            currentSelector.Parent.Child = null;
            return rootSelector;
        }

    }
}

