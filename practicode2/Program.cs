using practicode2;
using System.Diagnostics;
using System.Text.RegularExpressions;

static async Task<string> Load(string url)
{
    HttpClient client = new HttpClient();
    var response = await client.GetAsync(url);
    var html = await response.Content.ReadAsStringAsync();
    return html;
}

static HtmlElement Serializer(List<string> htmlLines)
{
    // צור את אובייקט השורש
    var root = new HtmlElement();

    // צור משתנה שיחזיק את האלמנט הנוכחי בכל איטרציה
    var current = root;

    // רוץ בלולאה על רשימת המחרוזות
    foreach (var line in htmlLines)
    {
        // חתכי את המילה הראשונה במחרוזת
        var tag = line.Split(' ')[0];
        //אם היא "html/" סימן שהגעת לסוף ה-html
        if (tag == "/html")
        {
            break;
        }
        // בדקי אם זו תגית סגירה
        if (tag.StartsWith("/"))
        {
            // עלי לרמה הקודמת בעץ
            current = current.Parent;
        }
        else
        {
            // אם זו תגית פתיחה
            if (HtmlHelper.Instance.Tags.Contains(tag))
            {
                // צור אובייקט חדש
                var element = new HtmlElement();

                // פרקי את המשך המחרוזת
                var restLine = Regex.Replace(line, @"^\w+\s*", "");
                ParseAttributes(restLine, element);
                element.Name = tag;
                element.Parent = current;
                
                // אם התגית סוגרת את עצמה
                if (line.EndsWith("/") && !HtmlHelper.Instance.HtmlVoidTags.Contains(tag))
                {
                    // השאירי את האלמנט הנוכחי כמות שהוא
                    continue;
                }
                else
                {
                    // עדכן את האלמנט הנוכחי
                    current.Children.Add(element);
                    current = element;
                }
            }
            else
            {
                // אם זו לא תגית, עדכן את InnerHtml של האלמנט הנוכחי
                current.InnerHtml = line;
            }
        }
    }
    return root;
}
static void ParseAttributes(string attributeLine, HtmlElement element)
{
    var attributes = new Regex("([^\\s]*?)=\"(.*?)\"").Matches(attributeLine);
    foreach (Match attribute in attributes)
    {
        var attributeName = attribute.Groups[1].Value;
        var attributeValue = attribute.Groups[2].Value;

        if (attributeName == "class")
        {
            // Handle the 'class' attribute by splitting it into parts and updating the 'Classes' property
            element.Classes.AddRange(attributeValue.Split(' ', StringSplitOptions.RemoveEmptyEntries));
        }
        else if (attributeName == "id")
        {
            // Change the 'Id' property to string and update it
            element.Id = attributeValue;
        }
        else
        {
            // Update the 'Attributes' property for other attributes
            element.Attributes.Add($"{attributeName}=\"{attributeValue}\"");
        }
    }
}
static void PrintTree(HtmlElement element)
{
    Console.WriteLine("--------------------------------");
    Console.WriteLine(element.Name);
    Console.WriteLine("Attributes:");
    foreach (var attr in element.Attributes)
    {
        Console.WriteLine(attr);
    }
    if (element.Classes != null)
    {
        Console.WriteLine("Classes:");
        foreach (var cls in element.Classes)
        {
            Console.WriteLine(cls + " ");
        }
    }
    if(element.Id != null)
    {
        Console.WriteLine("id: "+element.Id);
    }
    if (element.Children.Count > 0)
    {
        foreach (var child in element.Children)
        {
            PrintTree(child);
        }
    }
}

var html = await Load("https://learn.malkabruk.co.il/");
var cleanHtml = new Regex("\\s+").Replace(html, " ");
var htmlLines = new Regex("<(.*?)>").Split(cleanHtml).Where(s => s.Length > 0).ToList();
htmlLines.RemoveAll(string.IsNullOrWhiteSpace);
HtmlElement root = Serializer(htmlLines);
//PrintTree(root);
Selector s = Selector.ConvertQueryToSelector("div.home-desktop-menu div#profile-menu a#login-btn");
Selector s1 = Selector.ConvertQueryToSelector("div.home-hero div.home-btn-group a.button.home-hero-button1");
Selector s2 = Selector.ConvertQueryToSelector("div div img.logo");

foreach (var item in root.FindElementsBySelector(s1))
{
    Console.WriteLine(item.Name);
    Console.WriteLine("id:"+item.Id);
    foreach (var cls in item.Classes) { Console.WriteLine(cls+" "); }
    foreach (var anc in HtmlElement.Ancestors(item)) { Console.Write(anc.Name+" "); } ;
};



