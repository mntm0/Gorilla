namespace Gorilla.Objects
{
    public class NullObject : IObject
    {
        public string Inspect() => "null";
        public ObjectType Type() => ObjectType.NULL;
    }
}
