namespace Gorilla.Objects
{
    public class StringObject : IObject
    {
        public string Value { get; set; }

        public StringObject(string value) => this.Value = value; 
        public string Inspect() => this.Value;
        public ObjectType Type() => ObjectType.STRING;
    }
}
