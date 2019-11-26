using System.Collections.Generic;
using CppSharp.AST;
using CppSharp.AST.Extensions;
using CppSharp.Passes;

namespace NativeBindingsGenerator
{
    sealed class IgnoreUnneededSvnDeclarationsPass : TranslationUnitPass
    {
        const string _moduleName = "SvnUnmanagedApi";

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
                if (!_declaredEnums.Contains(@enum.OriginalName))
                {
                    @enum.ExplicitlyIgnore();
                }
            }

            return base.VisitEnumDecl(@enum);
        }

        public override bool VisitTypedefDecl(TypedefDecl typedef)
        {
            if (typedef.TranslationUnit.Module?.LibraryName == _moduleName)
            {
                if (typedef.Type.Desugar() is FunctionType ||
                    typedef.Type.Desugar().IsPointerTo<FunctionType>(out _))
                {
                    if (!_declaredDelegates.Contains(typedef.OriginalName))
                    {
                        typedef.ExplicitlyIgnore();
                    }
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
                "svn_auth",
                new[]
                {
                    "svn_auth_open",
                    "svn_auth_get_ssl_client_cert_file_provider",
                    "svn_auth_get_ssl_client_cert_prompt_provider",
                    "svn_auth_get_simple_provider2",
                    "svn_auth_get_platform_specific_provider",
                    "svn_auth_get_simple_prompt_provider",
                    "svn_auth_get_ssl_server_trust_file_provider",
                    "svn_auth_get_platform_specific_provider",
                    "svn_auth_get_ssl_server_trust_prompt_provider",
                    "svn_auth_get_username_provider",
                    "svn_auth_get_username_prompt_provider",
                    "svn_auth_set_parameter",
                }
            },
            {
                "svn_cache_config",
                new[]
                {
                    "svn_cache_config_get",
                    "svn_cache_config_set"
                }
            },
            {
                "svn_checksum",
                new[] {"svn_checksum_to_cstring"}
            },
            {
                "svn_client",
                new[]
                {
                    "svn_client_add5",
                    "svn_client_checkout3",
                    "svn_client_commit6",
                    "svn_client_delete4",
                    "svn_client_info4",
                    "svn_client_list3",
                    "svn_client_log5",
                    "svn_client_move7",
                    "svn_client_propget5",
                    "svn_client_propset_local",
                    "svn_client_status6",
                    "svn_client_version",
                    "svn_client_get_repos_root",
                    "svn_client_url_from_path2",
                    "svn_client_get_wc_root",
                    "svn_client_create_context2",
                    "svn_client_update4",
                    "svn_client_cat3",
                    "svn_client_export5",
                    "svn_client_copy7",
                }
            },
            {
                "svn_compat",
                new[]
                {
                    "svn_compat_log_revprops_in",
                    "svn_compat_log_revprops_out",
                }
            },
            {
                "svn_config",
                new[]
                {
                    "svn_config_get",
                    "svn_config_get_bool",
                    "svn_config_get_config",
                    "svn_config_ensure",
                    "svn_config_set",
                    "svn_config_set_bool",
                }
            },
            {
                "svn_dirent_uri",
                new[]
                {
                    "svn_dirent_basename",
                    "svn_dirent_canonicalize",
                    "svn_dirent_is_absolute",
                    "svn_dirent_join",
                    "svn_dirent_local_style",
                    "svn_dirent_skip_ancestor",
                    "svn_uri_canonicalize",
                }
            },
            {
                "svn_dso",
                new[] {"svn_dso_initialize2"}
            },
            {
                "svn_error",
                new[]
                {
                    "svn_error_create",
                    "svn_error_clear",
                    "svn_err_best_message",
                    "svn_error_set_malfunction_handler",
                }
            },
            {
                "svn_io",
                new[]
                {
                    "svn_io_parse_mimetypes_file",
                    "svn_stream_create",
                    "svn_stream_set_close",
                    "svn_stream_set_mark",
                    "svn_stream_set_read",
                    "svn_stream_set_seek",
                    "svn_stream_set_write",
                }
            },
            {
                "svn_opt",
                new[]
                {
                    "svn_opt_parse_path",
                    "svn_opt_revision_t"
                }
            },
            {
                "svn_path",
                new[]
                {
                    "svn_path_is_url",
                    "svn_path_url_add_component2",
                }
            },
            {
                "svn_pools",
                new[]
                {
                    "svn_pool_create_ex",
                    "svn_pool_clear",
                    "svn_pool_destroy",
                }
            },
            {
                "svn_props",
                new[]
                {
                    "svn_prop_is_boolean",
                    "svn_prop_name_is_valid",
                    "svn_prop_needs_translation",
                }
            },
            {
                "svn_ra",
                new[] {"svn_ra_initialize"}
            },
            {
                "svn_sorts",
                new[] {"svn_sort_compare_items_as_paths"}
            },
            {
                "svn_sorts_private",
                new[] {"svn_sort__hash"}
            },
            {
                "svn_time",
                new[] {"svn_time_from_cstring"}
            },
            {
                "svn_types",
                new[] {"svn_mime_type_is_binary"}
            },
            {
                "svn_utf",
                new[] {"svn_utf_initialize2"}
            },
            {
                "svn_wc",
                new[]
                {
                    "svn_wc_context_create",
                    "svn_wc_create_conflict_result",
                    "svn_wc_get_adm_dir",
                    "svn_wc_set_adm_dir",
                }
            },
            {
                "libsvnsharp_client",
                new[] {"svn_client__get_private_ctx"}
            },
            {
                "libsvnsharp_wc_private",
                new[] {"svn_wc__status2_from_3"}
            },
        };

        static IList<string> _declaredStructs = new[]
        {
            "svn_log_entry_t",
            "svn_error_t",
            "svn_sort__item_t",
            "svn_log_changed_path2_t",
            "svn_wc_conflict_description2_t",
            "svn_client_commit_item3_t",
            "svn_wc_notify_t",
            "svn_auth_baton_t",
            "svn_auth_cred_ssl_server_trust_t",
            "svn_auth_ssl_server_cert_info_t",
            "svn_auth_cred_ssl_server_trust_t",
            "svn_auth_provider_object_t",
            "svn_wc_conflict_version_t",
            "svn_dirent_t",
            "svn_merge_range_t",
            "svn_lock_t",
            "svn_string_t",
            "svn_commit_info_t",
            "svn_client_status_t",
            "svn_client_info2_t",
            "svn_client_ctx_t",
            "svn_wc_entry_t",
            "svn_wc_context_t",
            "svn_stream_t",
            "svn_stream_mark_t",
            "svn_wc_info_t",
            "svn_checksum_t",
            "svn_auth_cred_simple_t",
            "svn_client__private_ctx_t",
            "svn_wc_status2_t",
            "svn_wc_status3_t",
            "svn_auth_cred_username_t",
            "svn_opt_revision_t",
            "svn_opt_revision_range_t",
            "svn_opt_revision_value_t",
            "svn_auth_provider_t",
            "svn_auth_cred_ssl_client_cert_t",
            "svn_wc_conflict_result_t",
            "svn_config_t",
            "svn_cache_config_t",
            "svn_version_t",
            "svn_client_copy_source_t",
        };

        static IList<string> _declaredEnums = new[]
        {
            "svn_wc_conflict_choice_t",
            "svn_opt_revision_kind",
            "svn_wc_conflict_reason_t",
            "svn_node_kind_t",
            "svn_wc_notify_action_t",
            "svn_wc_status_kind",
            "svn_wc_notify_lock_state_t",
            "svn_depth_t",
            "svn_wc_schedule_t",
            "svn_dirent_enum_t",
            "svn_wc_operation_t",
            "svn_client_commit_item_enum_t",
            "svn_auth_ssl_enum_t",
            "svn_wc_conflict_action_t",
            "svn_wc_notify_state_t",
            "svn_wc_conflict_kind_t",
            "svn_tristate_t",
        };

        static IList<string> _declaredDelegates = new[]
        {
            "svn_auth_simple_prompt_func_t",
            "svn_auth_username_prompt_func_t",
            "svn_auth_plaintext_prompt_func_t",
            "svn_auth_ssl_server_trust_prompt_func_t",
            "svn_auth_ssl_client_cert_prompt_func_t",
            "svn_error_malfunction_handler_t",
            "svn_read_fn_t",
            "svn_write_fn_t",
            "svn_close_fn_t",
            "svn_stream_mark_fn_t",
            "svn_stream_seek_fn_t",
            "svn_wc_notify_func_t",
            "svn_client_get_commit_log_t",
            "svn_cancel_func_t",
            "svn_wc_notify_func2_t",
            "svn_client_get_commit_log2_t",
            "svn_ra_progress_notify_func_t",
            "svn_client_get_commit_log3_t",
            "svn_wc_conflict_resolver_func2_t",
            "svn_ra_check_tunnel_func_t",
            "svn_ra_open_tunnel_func_t",
            "svn_ra_close_tunnel_func_t",
            "svn_commit_callback2_t",
            "svn_commit_callback2_t",
            "svn_client_status_func_t",
            "svn_log_entry_receiver_t",
            "svn_commit_callback2_t",
            "svn_client_list_func2_t",
            "svn_client_info_receiver2_t",
        };
    }
}
