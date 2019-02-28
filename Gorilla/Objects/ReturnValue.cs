namespace Gorilla.Objects
{
    public class ReturnValue : IObject
    {
        public IObject Value { get; set; }

        public ReturnValue(IObject value) => this.Value = value;
        public string Inspect() => this.Value.Inspect();
        public ObjectType Type() => ObjectType.RETURN_VALUE;
    }
}
