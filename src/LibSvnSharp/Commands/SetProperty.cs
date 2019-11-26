using System;
using System.Collections.Generic;
using LibSvnSharp.Implementation;
using LibSvnSharp.Interop.Svn;
using LibSvnSharp.Properties;

namespace LibSvnSharp
{
    public partial class SvnClient
    {
        /// <overloads>Set the value of a property on files, dirs (<c>svn propset</c>)</overloads>
        /// <summary>Sets the specified property on the specfied path to value</summary>
        /// <remarks>Use <see cref="DeleteProperty(string,string, SvnSetPropertyArgs)" /> to remove an existing property</remarks>
        public bool SetProperty(string target, string propertyName, string value)
        {
            if (string.IsNullOrEmpty(target))
                throw new ArgumentNullException(nameof(target));
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException(nameof(propertyName));
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (!IsNotUri(target))
                throw new ArgumentException(SharpSvnStrings.ArgumentMustBeAPathNotAUri, nameof(target));

            return SetProperty(target, propertyName, value, new SvnSetPropertyArgs());
        }

        /// <summary>Sets the specified property on the specfied path to value</summary>
        /// <remarks>Use <see cref="DeleteProperty(string,string, SvnSetPropertyArgs)" /> to remove an existing property</remarks>
        public bool SetProperty(string target, string propertyName, ICollection<byte> bytes)
        {
            if (string.IsNullOrEmpty(target))
                throw new ArgumentNullException(nameof(target));
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException(nameof(propertyName));
            if (!IsNotUri(target))
                throw new ArgumentException(SharpSvnStrings.ArgumentMustBeAPathNotAUri, nameof(target));
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            return SetProperty(target, propertyName, bytes, new SvnSetPropertyArgs());
        }

        /// <summary>Sets the specified property on the specfied path to value</summary>
        /// <remarks>Use <see cref="DeleteProperty(string,string, SvnSetPropertyArgs)" /> to remove an existing property</remarks>
        public bool SetProperty(string target, string propertyName, string value, SvnSetPropertyArgs args)
        {
            if (string.IsNullOrEmpty(target))
                throw new ArgumentNullException(nameof(target));
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException(nameof(propertyName));
            if (!IsNotUri(target))
                throw new ArgumentException(SharpSvnStrings.ArgumentMustBeAPathNotAUri, nameof(target));
            if (args == null)
                throw new ArgumentNullException(nameof(args));
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            using var pool = new AprPool(_pool);

            return InternalSetProperty(
                target,
                propertyName,
                pool.AllocPropertyValue(value, propertyName),
                args,
                pool);
        }

        /// <summary>Sets the specified property on the specfied path to value</summary>
        /// <remarks>Use <see cref="DeleteProperty(string,string, SvnSetPropertyArgs)" /> to remove an existing property</remarks>
        public bool SetProperty(string target, string propertyName, ICollection<byte> bytes, SvnSetPropertyArgs args)
        {
            if (string.IsNullOrEmpty(target))
                throw new ArgumentNullException(nameof(target));
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException(nameof(propertyName));
            if (!IsNotUri(target))
                throw new ArgumentException(SharpSvnStrings.ArgumentMustBeAPathNotAUri, nameof(target));
            if (args == null)
                throw new ArgumentNullException(nameof(args));
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            using var pool = new AprPool(_pool);

            var byteArray = bytes as byte[];

            if (byteArray == null)
            {
                byteArray = new byte[bytes.Count];

                bytes.CopyTo(byteArray, 0);
            }

            return InternalSetProperty(target, propertyName, pool.AllocSvnString(byteArray), args, pool);
        }

        unsafe bool InternalSetProperty(string target, string propertyName, svn_string_t value, SvnSetPropertyArgs args, AprPool pool)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException(nameof(propertyName));
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException(nameof(propertyName));

            EnsureState(SvnContextState.ConfigLoaded); // We might need repository access
            using var store = new ArgsStore(this, args, pool);

            sbyte* pcPropertyName = pool.AllocString(propertyName);

            if (!svn_props.svn_prop_name_is_valid(pcPropertyName))
                throw new ArgumentException(SharpSvnStrings.PropertyNameIsNotValid, nameof(propertyName));

            svn_error_t r = svn_client.svn_client_propset_local(
                pcPropertyName,
                value,
                AllocDirentArray(NewSingleItemCollection(SvnTools.GetNormalizedFullPath(target)), pool),
                (svn_depth_t) args.Depth,
                args.SkipChecks,
                CreateChangeListsList(args.ChangeLists, pool), // Intersect ChangeLists
                CtxHandle,
                pool.Handle);

            return args.HandleResult(this, r, target);
        }
    }
}
