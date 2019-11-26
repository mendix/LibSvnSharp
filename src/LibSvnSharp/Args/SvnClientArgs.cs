using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using LibSvnSharp.Implementation;
using LibSvnSharp.Interop.Svn;

namespace LibSvnSharp
{
    /// <summary>Base class of all <see cref="SvnClient" /> arguments</summary>
    /// <threadsafety static="true" instance="false"/>
    public abstract class SvnClientArgs
    {
        static SvnClientArgs()
        {
            SvnBase.EnsureLoaded();
        }

        bool _noThrowOnError;
        bool _noThrowOnCancel;

        // On +- 90% of the SvnClientArgs instances none of these is used
        SvnException[] _warnings;
        SvnErrorCode[] _expectedErrors;
        SvnErrorCode[] _expectedCauses;
        SvnErrorCategory[] _expectedErrorCategories;

        /// <summary>Raised to allow canceling an in-progress command</summary>
        public event EventHandler<SvnCancelEventArgs> Cancel;

        /// <summary>Raised to notify of progress by an in-progress command</summary>
        public event EventHandler<SvnProgressEventArgs> Progress;

        /// <summary>Raised to notify changes by an in-progress command</summary>
        public event EventHandler<SvnNotifyEventArgs> Notify;

        /// <summary>Raised to notify errors from an command</summary>
        public event EventHandler<SvnErrorEventArgs> SvnError;

        /// <summary>Raises the <see cref="Cancel" /> event</summary>
        protected virtual void OnCancel(SvnCancelEventArgs e)
        {
            Cancel?.Invoke(this, e);
        }

        /// <summary>Raises the <see cref="Progress" /> event</summary>
        protected virtual void OnProgress(SvnProgressEventArgs e)
        {
            Progress?.Invoke(this, e);
        }

        /// <summary>Raises the <see cref="Notify" /> event</summary>
        protected virtual void OnNotify(SvnNotifyEventArgs e)
        {
            Notify?.Invoke(this, e);
        }

        /// <summary>Called when handling a <see cref="SvnException" /></summary>
        protected virtual void OnSvnError(SvnErrorEventArgs e)
        {
            SvnError?.Invoke(this, e);
        }

        internal bool _hooked;

        internal virtual void RaiseOnNotify(SvnNotifyEventArgs e)
        {
            switch (e.Action)
            {
                case SvnNotifyAction.LockFailedLock:
                case SvnNotifyAction.LockFailedUnlock:
                case SvnNotifyAction.PropertyDeletedNonExistent:
                case SvnNotifyAction.ExternalFailed:
                    if (e.Error != null)
                    {
                        _warnings = SvnBase.ExtendArray(_warnings, e.Error);
                    }
                    break;
            }
            OnNotify(e);
        }

        internal void RaiseOnCancel(SvnCancelEventArgs e)
        {
            OnCancel(e);
        }

        internal void RaiseOnProgress(SvnProgressEventArgs e)
        {
            OnProgress(e);
        }

        internal void RaiseOnSvnError(SvnErrorEventArgs e)
        {
            OnSvnError(e);
        }

        /// <summary>Gets the <see cref="SvnCommandType" /> of the command</summary>
        public abstract SvnCommandType CommandType { get; }

        /// <summary>
        /// Gets or sets a boolean indicating whether the call must throw an error if an error occurs.
        /// If an exception would occur, the method returns false and the <see cref="LastException" /> property
        /// is set to the exception which would have been throw. Defaults to true.
        /// </summary>
        public bool ThrowOnError
        {
            get => !_noThrowOnError;
            set => _noThrowOnError = !value;
        }

        /// <summary>
        /// Gets or sets a boolean indicating whether the call must throw an error if a non fatal error occurs.
        /// (E.g. locking or updating an external failed). Defaults to false
        /// </summary>
        public bool ThrowOnWarning { get; set; }

        /// <summary>
        /// Gets or sets a boolean indicating whether the call must throw an error if the operation is cancelled
        /// <see cref="IsLastInvocationCanceled" /> is true and the returnvalue <c>false</c> if the operation was canceled.
        /// (The <see cref="LastException" /> property is set to the cancel exception). Defaults to true
        /// </summary>
        public bool ThrowOnCancel
        {
            get => !_noThrowOnCancel;
            set => _noThrowOnCancel = !value;
        }

        /// <summary>
        /// Gets the last exception thrown by a Subversion api command to which this argument was provided
        /// </summary>
        public SvnException LastException { get; internal set; }

        /// <summary>Gets a collection of warnings issued by the last command invocation</summary>
        public ICollection<SvnException> Warnings
        {
            get
            {
                ReadOnlyCollection<SvnException> rc = null;

                if (_warnings != null && _warnings.Length != 0)
                {
                    IList<SvnException> warnings = _warnings;
                    rc = new ReadOnlyCollection<SvnException>(warnings);
                }

                return rc;
            }
        }

        public bool IsLastInvocationCanceled => LastException is SvnOperationCompletedException;

        /// <summary>Adds an error code to the list of errors not to throw exceptions for</summary>
        public void AddExpectedError(SvnErrorCode errorCode)
        {
            if (errorCode == SvnErrorCode.None)
                throw new ArgumentOutOfRangeException(nameof(errorCode));

            _expectedErrors = SvnBase.ExtendArray(_expectedErrors, errorCode);
        }

        /*
        /// <summary>Adds error codes to the list of errors not to throw exceptions for</summary>
        public void AddExpectedError(params SvnErrorCode[] errorCodes)
        {
            if (errorCodes == null || errorCodes.Length == 0)
                return;

            _expectedErrors = SvnBase.ExtendArray(_expectedErrors, errorCodes);
        }
        */

        /// <summary>Adds an error category to the list of errors not to throw exceptions for</summary>
        public void AddExpectedError(SvnErrorCategory errorCategory)
        {
            if (errorCategory == SvnErrorCategory.None)
                throw new ArgumentOutOfRangeException(nameof(errorCategory));

            _expectedErrorCategories = SvnBase.ExtendArray(_expectedErrorCategories, errorCategory);
        }

        /*
        /// <summary>Adds error categories to the list of errors not to throw exceptions for</summary>
        public void AddExpectedError(params SvnErrorCategory[] errorCategories)
        {
            if (errorCategories == null || errorCategories.Length == 0)
                return;

            _expectedErrorCategories = SvnBase.ExtendArray(_expectedErrorCategories, errorCategories);
        }
        */

        /// <summary>Adds an error code to the list of errors not to throw exceptions for</summary>
        public void AddExpectedCause(SvnErrorCode errorCode)
        {
            if (errorCode == SvnErrorCode.None)
                throw new ArgumentOutOfRangeException(nameof(errorCode));

            _expectedCauses = SvnBase.ExtendArray(_expectedErrors, errorCode);
        }

        /*
        /// <summary>Adds error codes to the list of errors not to throw exceptions for</summary>
        public void AddExpectedCause(... array<SvnErrorCode>^ errorCodes)
        {
            if (errorCodes == null || errorCodes.Length == 0)
                return;

            _expectedCauses = SvnBase.ExtendArray(_expectedErrors, errorCodes);
        }
         */

        internal bool HandleResult(SvnClientContext client, svn_error_t error)
        {
            return HandleResult(client, error, null);
        }

        internal bool HandleResult(SvnClientContext client, SvnException exception)
        {
            return HandleResult(client, exception, null);
        }

        internal bool HandleResult(SvnClientContext client, svn_error_t error, object targets)
        {
            // Note: client can be null if called from a not SvnClient command
            if (error == null)
            {
                LastException = null;
                return true;
            }

            var err = error.apr_err;

            LastException = SvnException.Create(error); // Releases error

            if (err == (int) SvnErrorCode.SVN_ERR_CEASE_INVOCATION)
                return false;

            return HandleResult(client, LastException, targets);
        }

        internal bool HandleResult(SvnClientContext client, SvnException exception, object targets)
        {
            LastException = exception;

            if (LastException == null)
                return true;

            if (LastException.SubversionErrorCode == (int) SvnErrorCode.SVN_ERR_CEASE_INVOCATION)
                return false;

            LastException.Targets = targets;

            var ea = new SvnErrorEventArgs(LastException);

            if (client is SvnClient svnClient)
                svnClient.HandleClientError(ea);
            else
                OnSvnError(ea);

            if (ea.Cancel)
                return false;

            if (_expectedErrors != null && 0 <= Array.IndexOf(_expectedErrors, LastException.SvnErrorCode))
                return false;

            if (_expectedErrorCategories != null && 0 <= Array.IndexOf(_expectedErrorCategories, LastException.SvnErrorCategory))
                return false;

            if (_expectedCauses != null && LastException.ContainsError(_expectedCauses))
                return false;

            if (!ThrowOnCancel && LastException.SubversionErrorCode == (int) SvnErrorCode.SVN_ERR_CANCELLED)
                return false;

            if (ThrowOnError)
                throw LastException;

            return false;
        }

        internal void Prepare()
        {
            LastException = null;
            _hooked = true;
            _warnings = null;
        }
    }
}
