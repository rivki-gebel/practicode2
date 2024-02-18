using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace practicode2
{
    public class HtmlElement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> Attributes { get; set; }
        public List<string> Classes { get; set; }
        public string InnerHtml { get; set; }
        public HtmlElement Parent { get; set; }
        public List<HtmlElement> Children { get; set; }
        public HtmlElement()
        {
            Children = new List<HtmlElement>();
            Attributes = new List<string>();
            Classes = new List<string>();
        }
        
        public static IEnumerable<HtmlElement> Descendants(HtmlElement element)
        {
            // יצירת תור חדש
            Queue<HtmlElement> queue = new Queue<HtmlElement>();

            // דחיפת האלמנט הנוכחי לתור
            queue.Enqueue(element);

            // לולאה כל עוד התור לא ריק
            while (queue.Count > 0)
            {
                // שליפה של האלמנט הראשון בתור
                HtmlElement currentElement = queue.Dequeue();

                // החזרת האלמנט הנוכחי
                yield return currentElement;

                // הוספת כל ילדי האלמנט הנוכחי לתור
                foreach (HtmlElement child in currentElement.Children)
                {
                    queue.Enqueue(child);
                }
            }
        }
        public static IEnumerable<HtmlElement> Ancestors(HtmlElement element)
        {
            HtmlElement currentElement = element;

            while (currentElement.Parent != null)
            {
                currentElement = currentElement.Parent;
                yield return currentElement;
            }
        }
    }
    public static class HtmlElementExtensions
    {
        public static List<HtmlElement> FindElementsBySelector(this HtmlElement rootElement, Selector selector)
        {
            HashSet<HtmlElement> result = new HashSet<HtmlElement>();
            FindElementsRecursively(rootElement, selector, result);
            return result.ToList();
        }

        private static void FindElementsRecursively(HtmlElement currentElement, Selector currentSelector, HashSet<HtmlElement> result)
        {
            // תנאי עצירה - אם הגענו לסלקטור האחרון
            if (currentSelector == null)
            {
                result.Add(currentElement);
                return;
            }
            // חפש את כל הצאצאים שעונים לקריטריונים של הסלקטור הנוכחי
            var filteredDescendants = HtmlElement.Descendants(currentElement)
                                                   .Where(descendant => MatchesSelector(descendant, currentSelector));

            // הפעל ריקורסיה על כל הצאצאים המסוננים עם הסלקטור הבא (הבן של הנוכחי)
            foreach (var descendant in filteredDescendants)
            {
                FindElementsRecursively(descendant, currentSelector.Child, result);
            }
        }
        private static bool MatchesSelector(HtmlElement element, Selector selector)
        {
            // בדיקה האם האלמנט עונה על קריטריונים הסלקטור
            bool matches = true;

            if (!string.IsNullOrEmpty(selector.TagName) &&
                !string.Equals(element.Name, selector.TagName, StringComparison.OrdinalIgnoreCase))
            {
                matches = false;
            }

            if (matches && !string.IsNullOrEmpty(selector.Id) &&
                !string.Equals(element.Id, selector.Id, StringComparison.OrdinalIgnoreCase))
            {
                matches = false;
            }
            if (matches && !selector.Classes.All(c => element.Classes.Contains(c)))
            {
                matches = false;
            }

            return matches;
        }
    }

}

