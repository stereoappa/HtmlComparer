using System;

namespace HtmlComparer.Model
{
    public class OutlineNode : IEquatable<OutlineNode>
    {
        public int Position { get; set; }
        public string TagName { get; set; }
        public string Text { get; set; }

        public bool Equals(OutlineNode other)
        {
            if (other is null)
                return false;

            return this.TagName == other.TagName && this.Text == other.Text && this.Position == other.Position;
        }

        public override bool Equals(object obj) => Equals(obj as OutlineNode);
        public override int GetHashCode() => (TagName, Text, Position).GetHashCode();
    }
}
