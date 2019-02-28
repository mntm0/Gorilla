namespace Gorilla.Objects
{
    public interface IObject
    {
        ObjectType Type();
        string Inspect();
    }

    public enum ObjectType
    {
        INTEGER,
        BOOLEAN,
        NULL,
        RETURN_VALUE,
        ERROR_OBJ,
    }
}
