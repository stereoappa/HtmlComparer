
using System;

namespace HtmlComparer.Model
{
    public class Page : IEquatable<Page>
    {
        private string _path;
        public string Path { get { return Clear(_path); } set { _path = value; } }

        private string Clear(string path)
        {
            return path.TrimEnd('/');
        }

        public bool Equals(Page other)
        {
            return this.Path.ToLower() == other.Path.ToLower();
        }

        public override bool Equals(object obj) => Equals(obj as Page);
        public override int GetHashCode() => (Path.ToLower()).GetHashCode();
    }
}
