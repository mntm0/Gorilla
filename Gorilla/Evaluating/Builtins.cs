using Gorilla.Objects;
using System.Collections.Generic;

namespace Gorilla.Evaluating
{
    public static class Builtins
    {
        public static Dictionary<string, BuiltinFunctionObject> BuiltinFunctions
            = new Dictionary<string, BuiltinFunctionObject>()
            {
                {"len", new BuiltinFunctionObject(Len) }
            };

        public static IObject Len(List<IObject> args)
        {
            if (args.Count != 1)
            {
                return new Error("引数の数が間違っています。(引数は1つ)");
            }

            var arg = args[0];
            switch (arg)
            {
                case StringObject stringObject:
                    var length = stringObject.Value.Length;
                    return new IntegerObject(length);
            }

            return new Error($"len の引数に対応していない型です。({arg.Type()})");
        }
    }
}
