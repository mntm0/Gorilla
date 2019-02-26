namespace Gorilla.Objects
{
    public class IntegerObject : IObject
    {
        public int Value { get; set; }

        public IntegerObject(int value) => this.Value = value;
        public string Inspect() => this.Value.ToString();
        public ObjectType Type() => ObjectType.INTEGER;
    }
}
