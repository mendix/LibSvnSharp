using System;

namespace LibSvnSharp.Implementation
{
    [Flags]
    enum SvnExtendedState
    {
        None = 0,
        MimeTypesLoaded = 0x01,
        TortoiseSvnHooksLoaded = 0x02
    }
}
