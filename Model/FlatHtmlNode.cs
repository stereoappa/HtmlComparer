using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlComparer.Model
{
    public class FlatHtmlNode : IEquatable<FlatHtmlNode>
    {
        public int Position { get; set; }
        public string Tag { get; set; }
        public string Text { get; set; }

        public bool Equals(FlatHtmlNode other)
        {
            if (other is null)
                return false;

            return this.Tag == other.Tag && this.Text == other.Text && this.Position == other.Position;
        }

        public override bool Equals(object obj) => Equals(obj as FlatHtmlNode);
        public override int GetHashCode() => (Tag, Text, Position).GetHashCode();
    }
}
