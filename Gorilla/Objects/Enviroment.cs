using System.Collections.Generic;

namespace Gorilla.Objects
{
    public class Enviroment
    {
        public Dictionary<string, IObject> Store { get; set; } 
            = new Dictionary<string, IObject>();

        public (IObject, bool) Get(string name)
        {
            var ok = this.Store.TryGetValue(name, out var value);
            return (value, ok);
        }

        public IObject Set(string name, IObject value)
        {
            this.Store.Add(name, value);
            return value;
        }
    }
}
