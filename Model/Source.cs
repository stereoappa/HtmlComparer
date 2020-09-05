using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaComparer.Model
{
    public class Source
    {
        public string BaseUrl { get; set; }
        public CompareRole CompareRole { get; set; }
        public bool ReturnUrlIsLowerCase { get; set; }
    }

    public enum CompareRole
    {
        Origin,
        Target
    }
}
