using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gorilla.Objects
{
    public class ArrayObject : IObject
    {
        public List<IObject> Elements { get; set; }

        public ArrayObject(List<IObject> elements) => this.Elements = elements;

        public string Inspect()
        {
            var elements = this.Elements.Select(e => e.Inspect());
            var builder = new StringBuilder();
            builder.Append("[");
            builder.Append(string.Join(", ", elements));
            builder.Append("]");
            return builder.ToString();
        }

        public ObjectType Type() => ObjectType.ARRAY_OBJ;
    }
}
