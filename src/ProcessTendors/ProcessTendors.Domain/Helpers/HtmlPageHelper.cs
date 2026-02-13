namespace ProcessTendors.Domain.Helpers
{
    public static class HtmlPageHelper
    {
        public static string ProcessHtml(string body)
        {
            var removePatterns = new List<string> { "footer", "ads", "tracking" };
            var clonedBody = new HtmlAgilityPack.HtmlDocument();
            clonedBody.LoadHtml(body);

            var nodes = clonedBody.DocumentNode.Descendants().ToList();

            foreach (var node in nodes)
            {
                var classAttr = node.GetAttributeValue("class", null);
                if (classAttr != null)
                {
                    var classList = classAttr.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (classList.Any(className => removePatterns.Any(pattern => className.Contains(pattern))))
                    {
                        node.Remove();
                    }
                }
            }

            return clonedBody.DocumentNode.InnerText;
        }
    }
}
