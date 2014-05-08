using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.Diagnostics
{
    /// <summary>
    /// Represents information about the diagnostic event.
    /// This information will be passed to Filters and Sinks.
    /// </summary>
    public partial class DiagnosticEvent : IDiagnosticEvent
    {
        public static readonly IComparer DateTimeLocalComparer = new DateTimeLocalComparerImplementation();
        public static readonly IComparer DateTimeLocalMostRecentFirstComparer = new DateTimeLocalMostRecentFirstComparerImplementation();

        ILogger Diagnostics = new InternalDiagnosticLogger(typeof(DiagnosticEvent).FullName);

        internal Func<string> GetMessage { get; set; }
        
        string VerbatimMessage { get; set; }

        public string Message
        {
            get
            {
                EvaluateMessageIfNeeded();
                return VerbatimMessage;
            }
            set
            {
                VerbatimMessage = value;
            }
        }

        public bool HasMessage
        {
            get
            {
                return VerbatimMessage != null || GetMessage != null;
            }
        }

        public Exception ExceptionObject { get; set; }

        public string Category { get; set; }

        public bool HasCategory
        {
            get
            {
                return !Category.IsNullOrEmpty();
            }
        }

        public bool HasException
        {
            get
            {
                return ExceptionObject != null;
            }
        }

        public SeverityLevel Severity { get; set; }
        public bool HasRawMessage { get; set; }

        public IDiagnosticEventPropertyCollection Properties { get; private set; }

        public bool HasAdditionalContextData
        {
            get
            {
                return AdditionalContextData != null && AdditionalContextData.Any();
            }
        }

        public IDiagnosticEventPropertyCollection AdditionalContextData { get; set; }

        /// <summary>
        /// Collection of additional objects which will have their representation sent to sinks.
        /// </summary>
        public IAttachedObjectCollection AttachedObjects { get; set; }

        public bool HasAttachedObjects
        {
            get { return AttachedObjects != null && AttachedObjects.Count > 0; }
        }

        public string LoggerName { get; set; }

        public DateTime? DateTimeLocal
        {
            get
            {
                var p = (object) null;

                if (!Properties.TryGetValue("DateTime.Local", out p))
                    return null;

                return p as DateTime?;
            }
        }

        public DateTime? DateTimeUtc
        {
            get
            {
                var p = (object)null;

                if (!Properties.TryGetValue("DateTime.Utc", out p))
                    return null;

                return p as DateTime?;
            }
        }

        public DiagnosticEvent()
        {
            Properties = new DiagnosticEventPropertyCollection(capacity: 27);

            Properties.AddOrUpdate("Event.HasCallerInfo", false);

            LoggerName = "";

            AttachedObjects = new AttachedObjectCollection();
        }

        public void PinAdditionalContextDataIfNeeded()
        {
            Properties.AddOrUpdate("Event.HasAdditionalContextData", HasAdditionalContextData);
            Properties.AddOrUpdate("Event.AdditionalContextData", AdditionalContextData);
        }

        public void EvaluateAndPinAllDataIfNeeded()
        {
            EvaluateMessageIfNeeded();

            Properties.AddOrUpdate("Event.Level", Severity);

            Properties.AddOrUpdate("Event.HasLoggerName", !LoggerName.IsNullOrEmpty());

            Properties.AddOrUpdate("Event.LoggerName", LoggerName);

            Properties.AddOrUpdate("Event.HasRawMessage", HasRawMessage);

            Properties.AddOrUpdate("Event.HasCategory", HasCategory);

            Properties.AddOrUpdate("Event.Category", Category.ToString(valueWhenNull: "", valueWhenEmpty: ""));

            Properties.AddOrUpdate("Event.HasMessage", HasMessage);

            PinAdditionalContextDataIfNeeded();

            if (HasMessage)
                Properties.AddOrUpdate("Event.Message", Message);

            Properties.AddOrUpdate("Event.HasException", HasException);

            if (HasException)
                Properties.AddOrUpdate("Event.Exception", ExceptionObject);

            Properties.AddOrUpdate("Event.HasAttachedObjects", HasAttachedObjects);

            if (HasAttachedObjects)
                Properties.AddOrUpdate("Event.AttachedObjects", AttachedObjects);

            Properties.AddOrUpdate("DateTime.Local", DateTime.Now);
            Properties.AddOrUpdate("DateTime.Utc", DateTime.UtcNow);
        }

        void EvaluateMessageIfNeeded()
        {
            if (VerbatimMessage != null)
                return;

            if (GetMessage != null)
            {
                try
                {
                    VerbatimMessage = GetMessage();
                    return;
                }
                catch (Exception ex)
                {
                    Diagnostics.Error(ex, "Unable to load error message");

                    //! if failed to load error message, use information about this error as error message so it's not reevaluated in the future and gives feedback to users)
                    VerbatimMessage =
                        "LOGGER: Unable to load error messge: "
                        + Environment.NewLine
                        + ex.ToString();

                    return;
                }
            }
        }

        public static DiagnosticEvent PrepareDiagnosticEvent(
            SeverityLevel logLevel,
            Func<string> getMessage,
            Exception exception = null,
            string category = null,
            AttachedObject[] attachedObjects = null)
        {
            var de = new DiagnosticEvent();

            de.Severity = logLevel;

            de.Category = category;

            de.ExceptionObject = exception;

            de.GetMessage = getMessage;

            de.AttachedObjects.AddRange(attachedObjects);

            return de;
        }

        public static DiagnosticEvent PrepareDiagnosticEvent(
            SeverityLevel eventSeverity,
            string message = null,
            object[] messageArgs = null,
            Exception exception = null,
            string category = null,
            AttachedObject[] attachedObjects = null)
        {
            var de = new DiagnosticEvent();

            de.Severity = eventSeverity;

            de.Category = category;

            de.ExceptionObject = exception;

            if (message != null)
            {
                if (messageArgs != null)
                    de.GetMessage = () => string.Format(message, messageArgs);
                else
                    de.VerbatimMessage = message;
            }

            de.AttachedObjects.AddRange(attachedObjects);

            return de;
        }

        public object this[string propertyName]
        {
            get
            {
                var prop = (object) null;

                if (!Properties.TryGetValue(propertyName, out prop))
                    return null;

                return prop;
            }
        }
    }
}
