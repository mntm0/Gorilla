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
        STRING,
        NULL,
        RETURN_VALUE,
        ERROR_OBJ,
        FUNCTION_OBJ,
        BUILTIN_OBJ,
        ARRAY_OBJ,
    }
}
