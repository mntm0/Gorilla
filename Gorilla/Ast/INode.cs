namespace Gorilla.Ast
{
    public interface INode
    {
        string TokenLiteral();
        string ToCode();
    }
}
