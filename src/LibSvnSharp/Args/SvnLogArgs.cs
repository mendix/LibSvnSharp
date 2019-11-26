using System;
using System.Collections.ObjectModel;

namespace LibSvnSharp
{
    /// <summary>Extended Parameter container of SvnClient.Log</summary>
    /// <threadsafety static="true" instance="false"/>
    public class SvnLogArgs : SvnClientArgs
    {
        SvnRevision _pegRevision;
        SvnRevision _start;
        SvnRevision _end;
        int _limit;
        bool _noLogChangedPaths;

        SvnRevisionPropertyNameCollection _retrieveProperties;
        Collection<SvnRevisionRange> _ranges;
 
        internal int _mergeLogLevel; // Used by log handler to provide mergeLogLevel
        internal Uri _searchRoot;

        public event EventHandler<SvnLogEventArgs> Log;

        protected internal virtual void OnLog(SvnLogEventArgs e)
        {
            Log?.Invoke(this, e);
        }
 
        /// <summary>Initializes a new <see cref="SvnLogArgs" /> instance with default properties</summary>
        public SvnLogArgs()
        {
            _start = SvnRevision.None;
            _end = SvnRevision.None;
            _pegRevision = SvnRevision.None;
            //_limit = 0;
            //_noLogChangedPaths = false;
            //_strictHistory = false;
        }

        /// <summary>Initializes a new <see cref="SvnLogArgs" /> instance with the specified range</summary>
        public SvnLogArgs(SvnRevisionRange range)
        {
            Range = range ?? throw new ArgumentNullException(nameof(range));
            _pegRevision = SvnRevision.None;
        }

        public override sealed SvnCommandType CommandType => SvnCommandType.Log;

        /// <summary>Gets the revision in which the Url's are evaluated (Aka peg revision)</summary>
        public SvnRevision OperationalRevision
        {
            get => _pegRevision;
            set => _pegRevision = value ?? SvnRevision.None;
        }

        [Obsolete("Use .OperationalRevision")]
        public SvnRevision OriginRevision
        {
            get => OperationalRevision;
            set => OperationalRevision = value;
        }

        /// <summary>Gets or sets the range specified by <see cref="Start" />-<see cref="End" /> wrapped as a <see cref="SvnRevisionRange" /></summary>
        /// <remarks>New code should either use <see cref="Start" /> and <see cref="End" /> or <see cref="Ranges" /></remarks>
        public SvnRevisionRange Range
        {
            get => new SvnRevisionRange(Start, End);
            set
            {
                if (value == null)
                {
                    Start = null;
                    End = null;
                }
                else
                {
                    Start = value.StartRevision;
                    End = value.EndRevision;
                }
            }
        }

        public SvnRevision Start
        {
            get
            {
                if (_ranges != null)
                    return (_ranges.Count == 1) ? _ranges[0].StartRevision : SvnRevision.None;

                return _start ?? SvnRevision.None;
            }
            set
            {
                _start = value;
 
                if (_ranges != null)
                {
                    _end = End;
                    _ranges.Clear();
                    _ranges.Add(new SvnRevisionRange(Start, End));
                }
            }
        }
 
        public SvnRevision End
        {
            get
            {
                if (_ranges != null)
                    return (_ranges.Count == 1) ? _ranges[0].EndRevision : SvnRevision.None;

                return _end ?? SvnRevision.None;
            }
            set
            {
                _end = value;
 
                if (_ranges != null)
                {
                    _start = Start;
                    _ranges.Clear();
                    _ranges.Add(new SvnRevisionRange(Start, End));
                }
            }
        }
 
        public Collection<SvnRevisionRange> Ranges
        {
            get
            {
                if (_ranges == null)
                {
                    SvnRevision start = Start;
                    SvnRevision end = End;
 
                    _ranges = new Collection<SvnRevisionRange>();
 
                    if (start != SvnRevision.None || end != SvnRevision.None)
                        _ranges.Add(new SvnRevisionRange(start, end));
                }
 
                return _ranges;
            }
        }

        /// <summary>Gets or sets the limit on the number of log items fetched</summary>
        public int Limit
        {
            get => _limit;
            set => _limit = (value >= 0) ? value : 0;
        }

        /// <summary>Gets or sets a boolean indicating whether the paths changed in the revision should be provided</summary>
        public bool RetrieveChangedPaths
        {
            get => !_noLogChangedPaths;
            set => _noLogChangedPaths = !value;
        }

        /// <summary>Gets or sets a boolean indicating whether only the history of this exact node should be fetched (Aka stop on copying)</summary>
        /// <remarks>If @a StrictNodeHistory is set, copy history (if any exists) will
        /// not be traversed while harvesting revision logs for each target. </remarks>
        public bool StrictNodeHistory { get; set; }

        /// <summary>Gets or sets a boolean indicating whether the merged revisions should be fetched instead of the node changes</summary>
        public bool RetrieveMergedRevisions { get; set; }

        /// <summary>Gets or sets the base uri to which relative Uri's are relative</summary>
        public Uri BaseUri { get; set; }

        /// <summary>Gets the list of properties to retrieve. Only SVN 1.5+ repositories allow adding custom properties to this list</summary>
        /// <remarks>This list defaults to the author, date, logmessage properties. Clear the list to retrieve no properties,
        /// or set RetrieveAllProperties to true to retrieve all properties.
        /// </remarks>
        public SvnRevisionPropertyNameCollection RetrieveProperties =>
            _retrieveProperties ?? (_retrieveProperties = new SvnRevisionPropertyNameCollection(false));

        /// <summary>Gets or sets a boolean indication whether to retrieve all revision properties</summary>
        /// <remarks>Default value: false</remarks>
        public bool RetrieveAllProperties { get; set; }

        internal bool RetrievePropertiesUsed => _retrieveProperties != null;

        internal bool RangesUsed => _ranges != null;
    }
}
