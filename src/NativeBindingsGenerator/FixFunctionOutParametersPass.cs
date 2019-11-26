using CppSharp.AST;
using CppSharp.AST.Extensions;
using CppSharp.Passes;

namespace NativeBindingsGenerator
{
    sealed class FixFunctionOutParametersPass : TranslationUnitPass
    {
        public override bool VisitParameterDecl(Parameter parameter)
        {
            // Fix function
            if (parameter.QualifiedType.Type.IsPointerTo<PointerType>(out var innerPointer) &&
                innerPointer.IsPointerTo<TypedefType>(out _))
            {
                innerPointer.QualifiedPointee.Type = new BuiltinType(PrimitiveType.Void);
            }

            return base.VisitParameterDecl(parameter);
        }
    }
}
