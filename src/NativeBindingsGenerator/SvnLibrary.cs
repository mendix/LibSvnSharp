using System.IO;
using CppSharp;
using CppSharp.AST;
using CppSharp.Generators;
using CppSharp.Passes;
using NativeBindingsGenerator.Properties;

namespace NativeBindingsGenerator
{
    sealed class SvnLibrary : ILibrary
    {
        public void Preprocess(Driver driver, ASTContext ctx)
        {
            driver.Context.TranslationUnitPasses.Passes.RemoveAll(p => p is CaseRenamePass);

            // CppSharp doesn't know how to generate these, so we won't insist
            ctx.IgnoreClassWithName("svn_opt_subcommand_desc2_t");
            ctx.IgnoreClassWithName("svn_opt_subcommand_desc_t");

            // These functions benefit from string parameters passed/returned
            ctx.ExcludeFromPass("svn_wc_get_adm_dir", typeof(ReplaceStringsWithSbytesPass));
            ctx.ExcludeFromPass("svn_wc_set_adm_dir", typeof(ReplaceStringsWithSbytesPass));
            ctx.ExcludeFromPass("svn_error_create", typeof(ReplaceStringsWithSbytesPass));
            ctx.ExcludeFromPass("svn_auth_get_platform_specific_provider", typeof(ReplaceStringsWithSbytesPass));

            // These enums will be used in the code
            ctx.GenerateEnumFromMacros("svn_client_commit_item_enum_t", "SVN_CLIENT_COMMIT_ITEM_*");
            ctx.GenerateEnumFromMacros("svn_dirent_enum_t", "SVN_DIRENT_*");
            ctx.GenerateEnumFromMacros("svn_auth_ssl_enum_t", "SVN_AUTH_SSL_*");

            // These functions return parameters
            ctx.SetFunctionParameterUsage("svn_opt_parse_path", 1, ParameterUsage.Out);
        }

        public void Postprocess(Driver driver, ASTContext ctx)
        {
        }

        public void Setup(Driver driver)
        {
            driver.Options.GeneratorKind = GeneratorKind.CSharp;
            driver.Options.CompileCode = true;
            driver.Options.OutputDir = Path.Combine(driver.Options.OutputDir, "Generated Files");
            driver.Options.StripLibPrefix = false;

            var aprModule = driver.Options.AddModule("AprUnmanagedApi");
            aprModule.OutputNamespace = "LibSvnSharp.Interop.Apr";
            aprModule.IncludeDirs.Add(Settings.Default.apr_include_dir);
            aprModule.IncludeDirs.Add(Settings.Default.aprutil_include_dir);
            aprModule.SharedLibraryName = "LibSvnSharp.Native";
            aprModule.Headers.Add("apr_allocator.h");
            aprModule.Headers.Add("apr_base64.h");
            aprModule.Headers.Add("apr_general.h");
            aprModule.Headers.Add("apr_hash.h");
            aprModule.Headers.Add("apr_pools.h");
            aprModule.Headers.Add("apr_strings.h");
            aprModule.Headers.Add("apr_tables.h");

            var svnModule = driver.Options.AddModule("SvnUnmanagedApi");
            svnModule.OutputNamespace = "LibSvnSharp.Interop.Svn";
            svnModule.Dependencies.Add(aprModule);
            svnModule.IncludeDirs.Add(Settings.Default.apr_include_dir);
            svnModule.IncludeDirs.Add(Settings.Default.aprutil_include_dir);
            svnModule.IncludeDirs.Add(Settings.Default.subversion_include_dir);
            svnModule.IncludeDirs.Add(Path.Combine(Settings.Default.subversion_include_dir, "private"));
            svnModule.IncludeDirs.Add(Settings.Default.libsvnsharp_include_dir);
            svnModule.SharedLibraryName = "LibSvnSharp.Native";
            svnModule.Headers.Add("svn_auth.h");
            svnModule.Headers.Add("svn_cache_config.h");
            svnModule.Headers.Add("svn_checksum.h");
            svnModule.Headers.Add("svn_client.h");
            svnModule.Headers.Add("svn_compat.h");
            svnModule.Headers.Add("svn_config.h");
            svnModule.Headers.Add("svn_dirent_uri.h");
            svnModule.Headers.Add("svn_dso.h");
            svnModule.Headers.Add("svn_error.h");
            svnModule.Headers.Add("svn_io.h");
            svnModule.Headers.Add("svn_opt.h");
            svnModule.Headers.Add("svn_path.h");
            svnModule.Headers.Add("svn_pools.h");
            svnModule.Headers.Add("svn_props.h");
            svnModule.Headers.Add("svn_ra.h");
            svnModule.Headers.Add("svn_sorts.h");
            svnModule.Headers.Add("svn_sorts_private.h");
            svnModule.Headers.Add("svn_time.h");
            svnModule.Headers.Add("svn_types.h");
            svnModule.Headers.Add("svn_utf.h");
            svnModule.Headers.Add("svn_version.h");
            svnModule.Headers.Add("svn_wc.h");
            svnModule.Headers.Add("libsvnsharp_auth.h");
            svnModule.Headers.Add("libsvnsharp_client.h");
            svnModule.Headers.Add("libsvnsharp_wc_private.h");
        }

        public void SetupPasses(Driver driver)
        {
            driver.AddTranslationUnitPass(new ReplaceStringsWithSbytesPass());
            driver.AddTranslationUnitPass(new FixFunctionOutParametersPass());
            driver.AddTranslationUnitPass(new FixSvnBooleanPointersPass());

            driver.AddTranslationUnitPass(new IgnoreUnneededAprDeclarationsPass());
            driver.AddTranslationUnitPass(new IgnoreUnneededSvnDeclarationsPass());
        }
    }
}
