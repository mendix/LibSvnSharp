using CppSharp.AST;
using CppSharp.Generators;
using CppSharp.Generators.CSharp;
using CppSharp.Types;

namespace NativeBindingsGenerator
{
    [TypeMap("svn_boolean_t", GeneratorKind = GeneratorKind.CSharp)]
    public class SvnBooleanTypeMap : TypeMap
    {
        public override CppSharp.AST.Type CSharpSignatureType(TypePrinterContext ctx)
        {
            if (ctx.Kind == TypePrinterContextKind.Managed)
                return new CILType(typeof(bool));

            return new CILType(typeof(int));
        }

        public override void CSharpMarshalToManaged(CSharpMarshalContext ctx)
        {
            ctx.Return.Write("({0} != 0)", ctx.ReturnVarName);
        }

        public override void CSharpMarshalToNative(CSharpMarshalContext ctx)
        {
            ctx.Return.Write("({0} ? 1 : 0)", ctx.Parameter.Name);
        }
    }
}
