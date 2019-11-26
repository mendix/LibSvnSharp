using System;
using System.Text.RegularExpressions;

namespace LibSvnSharp.Security
{
    public abstract class SvnAuthenticationEventArgs : SvnEventArgs
    {
        bool _save;
        Uri _realmUri;

        internal static readonly Regex _reRealmUri = new Regex(
            "^\\<(?<server>[-+a-z]+://[^ >]+)\\>( (?<realm>.*))?$",
            RegexOptions.ExplicitCapture | RegexOptions.Singleline);

        protected SvnAuthenticationEventArgs(string realm, bool maySave)
        {
            Realm = realm;
            MaySave = maySave;
        }

        /// <summary>If MaySave is false, the auth system does not allow the credentials
        /// to be saved (to disk). A prompt function shall not ask the user if the
        /// credentials shall be saved if may_save is FALSE. For example, a GUI client
        /// with a remember password checkbox would grey out the checkbox if may_save
        /// is false</summary>
        public bool MaySave { get; }

        /// <summary>If realm is non-null, maybe use it in the prompt string</summary>
        public string Realm { get; }

        public Uri RealmUri
        {
            get
            {
                if (_realmUri != null || Realm == null)
                    return _realmUri;

                var m = _reRealmUri.Match(Realm);

                if (m.Success)
                {
                    var uriValue = m.Groups[1].Value;

                    if (uriValue != null && !uriValue.EndsWith("/", StringComparison.Ordinal))
                        uriValue += "/";

                    if (Uri.TryCreate(uriValue, UriKind.Absolute, out var uri))
                        _realmUri = uri;
                }

                return _realmUri;
            }
        }

        public bool Save
        {
            get => _save;
            set => _save = value && MaySave;
        }

        public bool Cancel { get; set; }

        public bool Break { get; set; }

        protected internal virtual void Clear()
        {
            Break = false;
            Cancel = false;
            _save = false;
        }
    }
}
