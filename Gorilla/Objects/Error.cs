namespace Gorilla.Objects
{
    public class Error : IObject
    {
        public string Message { get; set; }
        public Error(string message) => this.Message = message;
        public string Inspect() => this.Message;
        public ObjectType Type() => ObjectType.ERROR_OBJ;
    }
}
