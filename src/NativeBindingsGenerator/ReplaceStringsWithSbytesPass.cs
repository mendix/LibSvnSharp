using CppSharp.AST;
using CppSharp.AST.Extensions;
using CppSharp.Passes;

namespace NativeBindingsGenerator
{
    sealed class ReplaceStringsWithSbytesPass : TranslationUnitPass
    {
        public override bool VisitType(Type type, TypeQualifiers quals)
        {
            if (type.IsConstCharString() &&
                type is PointerType pointerType)
            {
                pointerType.QualifiedPointee.Qualifiers = new TypeQualifiers
                {
                    IsConst = false,
                    IsRestrict = pointerType.QualifiedPointee.Qualifiers.IsRestrict,
                    IsVolatile = pointerType.QualifiedPointee.Qualifiers.IsVolatile
                };
            }

            return base.VisitType(type, quals);
        }
    }
}
