using System;
using System.Collections.Generic;

namespace Gorilla.Objects
{
    using BuiltinFunction = Func<List<IObject>, IObject>;

    public class BuiltinFunctionObject : IObject
    {
        public BuiltinFunction Function { get; set; }
        public BuiltinFunctionObject(BuiltinFunction fn) => this.Function = fn;
        public string Inspect() => "builtin function";
        public ObjectType Type() => ObjectType.BUILTIN_OBJ;
    }
}
