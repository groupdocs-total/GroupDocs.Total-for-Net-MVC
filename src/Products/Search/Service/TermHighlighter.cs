using Aspose.Html;
using Aspose.Html.Dom;
using System;
using System.Collections.Generic;

namespace GroupDocs.Total.MVC.Products.Search.Service
{
    internal class TermHighlighter
    {
        private readonly HTMLDocument document;
        private readonly string[] terms;
        private List<Text> result;

        public TermHighlighter(HTMLDocument document, string[] terms)
        {
            this.document = document;
            this.terms = terms;
        }

        public void Run()
        {
            if (result != null)
            {
                throw new InvalidOperationException("The operation cannot be run twice.");
            }

            result = new List<Text>();
            foreach (var child in document.Children)
            {
                Find(child);
            }

            foreach (var node in result)
            {
                foreach (var term in terms)
                {
                    Replace(node, term);
                }
            }
        }

        private void Find(Node node)
        {
            if (node.NodeName.Equals("STYLE", StringComparison.InvariantCultureIgnoreCase) ||
                node.NodeName.Equals("TITLE", StringComparison.InvariantCultureIgnoreCase) ||
                node.NodeName.Equals("HEAD", StringComparison.InvariantCultureIgnoreCase) ||
                node.NodeName.Equals("SCRIPT", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            if (node.NodeType == 3)
            {
                var text = node.TextContent;
                if (!string.IsNullOrEmpty(text))
                {
                    result.Add((Text)node);
                }
            }

            foreach (var child in node.ChildNodes)
            {
                Find(child);
            }
        }

        private static void Replace(Text node, string term)
        {
            HTMLDocument document = (HTMLDocument)node.OwnerDocument;
            Text currentNode = node;
            while (true)
            {
                int index = currentNode.TextContent.IndexOf(term, StringComparison.InvariantCultureIgnoreCase);
                if (index < 0)
                {
                    break;
                }

                currentNode = currentNode.SplitText(index);
                var lastTextNode = currentNode.SplitText(term.Length);

                var span = document.CreateElement("span");
                span.ClassName = "gd-found-term";

                currentNode.ParentNode.ReplaceChild(span, currentNode);
                span.AppendChild(currentNode);

                currentNode = lastTextNode;
            }
        }
    }
}
