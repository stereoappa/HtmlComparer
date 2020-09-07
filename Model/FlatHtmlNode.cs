using System;

namespace HtmlComparer.Model
{
    public class OutlineNode : IEquatable<OutlineNode>
    {
        public int Position { get; set; }
        public string Tag { get; set; }
        public string Text { get; set; }

        public bool Equals(OutlineNode other)
        {
            if (other is null)
                return false;

            return this.Tag == other.Tag && this.Text == other.Text && this.Position == other.Position;
        }

        public override bool Equals(object obj) => Equals(obj as OutlineNode);
        public override int GetHashCode() => (Tag, Text, Position).GetHashCode();
    }
}
