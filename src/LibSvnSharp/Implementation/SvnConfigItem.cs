using System;

namespace LibSvnSharp.Implementation
{
    sealed class SvnConfigItem
    {
        public SvnConfigItem(string file, string section, string option, string value)
        {
            File = file ?? throw new ArgumentNullException(nameof(file));
            Section = section ?? throw new ArgumentNullException(nameof(section));
            Option = option ?? throw new ArgumentNullException(nameof(option));
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string File { get; }

        public string Section { get; }

        public string Option { get; }

        public string Value { get; }
    }
}
