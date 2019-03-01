using System.Collections.Generic;

namespace Gorilla.Objects
{
    public class Enviroment
    {
        public Dictionary<string, IObject> Store { get; set; }
            = new Dictionary<string, IObject>();
        public Enviroment Outer { get; set; }

        public static Enviroment CreateNewEnclosedEnviroment(Enviroment outer)
        {
            var enviroment = new Enviroment();
            enviroment.Outer = outer;
            return enviroment;
        }

        public (IObject, bool) Get(string name)
        {
            var ok = this.Store.TryGetValue(name, out var value);
            if (!ok && this.Outer != null)
            {
                (value, ok) = this.Outer.Get(name);
            }

            return (value, ok);
        }

        public IObject Set(string name, IObject value)
        {
            this.Store[name] = value;
            return value;
        }
    }
}
