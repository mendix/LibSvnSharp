using System;
using LibSvnSharp.Implementation;

namespace LibSvnSharp
{
    public sealed class SvnClientConfiguration : SvnBase
    {
        readonly SvnClientContext _client;
               
        internal SvnClientConfiguration(SvnClientContext client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        /*
        /// <summary>Gets or sets a boolean indicating whether commits will fail if no log message is provided</summary>
        /// <remarks>The default value of this property is true</remarks>
        [System.ComponentModel.DefaultValue((System.Boolean)true)]
        public bool LogMessageRequired
        {
            bool get();
            void set(bool value);
        }

        /// <summary>Gets or sets a boolean indicating whether to load the svn mimetypes file when calling Add or Import the first time</summary>
        /// <remarks>The default value of this property is true; this matches the behaviour of the svn commandline client</remarks>
        /// <value>true if loading the mimetypes file on the initial import, otherwise false</value>
        [System.ComponentModel.DefaultValue((System.Boolean)true)]
        public bool LoadSvnMimeTypes
        {
            bool get();
            void set(bool value);
        }

        //public SvnSshOverride SshOverride
        //{
        //    SvnSshOverride get();
        //    void set(SvnSshOverride value);
        //}

        /// <summary>Gets or sets a boolean indicating whether to always use the subversion integrated diff library
        /// instead of the user configured diff tools</summary>
        /// <remarks>The default value of this property is true; to allow parsing the output of the diff library</remarks>
        /// <value>true if subversions internal diff must be used, otherwise false</value>
        [System.ComponentModel.DefaultValue((System.Boolean)true)]
        public bool UseSubversionDiff
        {
            bool get();
            void set(bool value);
        }

        /// <summary>Gets or sets a value indicating whether the 'preserved-conflict-file-exts' should be forced to '*'</summary>
        [System.ComponentModel.DefaultValue(SvnOverride.Never)]
        public SvnOverride KeepAllExtensionsOnConflict
        {
            SvnOverride get();
            void set(SvnOverride value);
        }

        /// <summary>Gets the subversion global ignore pattern as specified in the configuration</summary>
        public IEnumerable<String> GlobalIgnorePattern
        {
            IEnumerable<String> get();
        }
        */

        /// <summary>While the configuration isn't used yet, allows overriding specific configuration options</summary>
        public void SetOption(string file, string section, string option, string value)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));
            if (section == null)
                throw new ArgumentNullException(nameof(section));
            if (option == null)
                throw new ArgumentNullException(nameof(option));
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            _client.SetConfigurationOption(file, section, option, value);
        }
    }
}
