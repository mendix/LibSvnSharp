using System.Collections.Generic;
using CppSharp.AST;
using CppSharp.AST.Extensions;
using CppSharp.Passes;

namespace NativeBindingsGenerator
{
    sealed class IgnoreUnneededAprDeclarationsPass : TranslationUnitPass
    {
        const string _moduleName = "AprUnmanagedApi";

        public override bool VisitFunctionDecl(Function function)
        {
            if (function.TranslationUnit.Module?.LibraryName == _moduleName)
            {
                if (!_declaredFunctions.TryGetValue(function.TranslationUnit.FileNameWithoutExtension, out var functionList))
                {
                    function.ExplicitlyIgnore();
                }
                else if (!functionList.Contains(function.OriginalName))
                {
                    function.ExplicitlyIgnore();
                }
            }

            return base.VisitFunctionDecl(function);
        }

        public override bool VisitClassDecl(Class @class)
        {
            if (@class.TranslationUnit.Module?.LibraryName == _moduleName)
            {
                if (!_declaredStructs.Contains(@class.OriginalName))
                {
                    @class.ExplicitlyIgnore();
                }
            }

            return base.VisitClassDecl(@class);
        }

        public override bool VisitEnumDecl(Enumeration @enum)
        {
            if (@enum.TranslationUnit.Module?.LibraryName == _moduleName)
            {
                @enum.ExplicitlyIgnore();
            }

            return base.VisitEnumDecl(@enum);
        }

        public override bool VisitTypedefDecl(TypedefDecl typedef)
        {
            if (typedef.TranslationUnit.Module?.LibraryName == _moduleName)
            {
                if (typedef.Type.Desugar() is FunctionType)
                {
                    typedef.ExplicitlyIgnore();
                }
                else if (typedef.Type.Desugar().IsPointerTo<FunctionType>(out _))
                {
                    typedef.ExplicitlyIgnore();
                }
            }

            return base.VisitTypedefDecl(typedef);
        }

        public override bool VisitVariableDecl(Variable variable)
        {
            variable.ExplicitlyIgnore();

            return base.VisitVariableDecl(variable);
        }

        static Dictionary<string, IList<string>> _declaredFunctions = new Dictionary<string, IList<string>>
        {
            {
                "apr_allocator",
                new[] {"apr_allocator_max_free_set"}
            },
            {
                "apr_base64",
                new[]
                {
                    "apr_base64_decode",
                    "apr_base64_decode_len"
                }
            },
            {
                "apr_general",
                new[] {"apr_initialize"}
            },
            {
                "apr_hash",
                new[]
                {
                    "apr_hash_first",
                    "apr_hash_next",
                    "apr_hash_this",
                    "apr_hash_make",
                    "apr_hash_get",
                    "apr_hash_set",
                }
            },
            {
                "apr_pools",
                new[]
                {
                    "apr_pool_allocator_get",
                    "apr_pool_clear",
                    "apr_pool_destroy",
                    "apr_pool_cleanup_register",
                    "apr_pool_cleanup_null",
                    "apr_palloc"
                }
            },
            {
                "apr_strings",
                new[] {"apr_pstrdup"}
            },
            {
                "apr_tables",
                new[]
                {
                    "apr_array_make",
                    "apr_array_push",
                }
            }
        };

        static IList<string> _declaredStructs = new[]
        {
            "apr_pool_t",
            "apr_allocator_t",
            "apr_hash_t",
            "apr_array_header_t",
            "apr_hash_index_t"
        };
    }
}
