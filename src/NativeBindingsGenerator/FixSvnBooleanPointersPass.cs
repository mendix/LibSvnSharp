using CppSharp.AST;
using CppSharp.AST.Extensions;
using CppSharp.Passes;

namespace NativeBindingsGenerator
{
    sealed class FixSvnBooleanPointersPass : TranslationUnitPass
    {
        public override bool VisitType(Type type, TypeQualifiers quals)
        {
            if (type is PointerType pointerType &&
                pointerType.IsPointerTo<TypedefType>(out var typedefType) &&
                typedefType.Declaration.OriginalName == "svn_boolean_t")
            {
                pointerType.QualifiedPointee.Type = new BuiltinType(PrimitiveType.Int);
            }

            return base.VisitType(type, quals);
        }
    }
}
