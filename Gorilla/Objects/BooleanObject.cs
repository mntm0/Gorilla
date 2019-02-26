namespace Gorilla.Objects
{
    public class BooleanObject : IObject
    {
        public bool Value { get; set; }

        public BooleanObject(bool value) => this.Value = value;
        public string Inspect() => this.Value ? "true" : "false";
        public ObjectType Type() => ObjectType.BOOLEAN;
    }
}
