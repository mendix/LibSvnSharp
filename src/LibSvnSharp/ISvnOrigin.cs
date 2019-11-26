using System;

namespace LibSvnSharp
{
    /// <summary>Generic encapsulation of a specific revision of a node in subversion</summary>
    public interface ISvnOrigin
    {
        /// <summary>(Required) The Uri of <see cref="Target" />. Only differs from target if <see cref="Target" />
        /// specifies a <see cref="SvnPathTarget" /></summary>
        Uri Uri { get; }

        /// <summary>The target specified</summary>
        SvnTarget Target { get; }

        /// <summary>(Required) The repository root of <see cref="Target" /></summary>
        Uri RepositoryRoot { get; }

        /// <summary>(Optional) The node kind of <see cref="Target" /></summary>
        SvnNodeKind NodeKind { get; }
    }
}
