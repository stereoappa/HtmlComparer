
using System;

namespace HtmlComparer.Model
{
    public class Page : IEquatable<Page>
    {
        public Page(string path)
        {
            if(string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path shouldn't be empty", path);
            }
            _path = path;
        }

        private string _path;
        public string LocalPath => Clear(_path);

        private string Clear(string path)
        {
            if (path[0] != '/')
            {
                return '/' + path;
            }
            return path.TrimEnd('/');
        }

        public bool Equals(Page other)
        {
            return this.LocalPath.ToLower() == other.LocalPath.ToLower();
        }

        public override bool Equals(object obj) => Equals(obj as Page);
        public override int GetHashCode() => (LocalPath.ToLower()).GetHashCode();
    }
}
