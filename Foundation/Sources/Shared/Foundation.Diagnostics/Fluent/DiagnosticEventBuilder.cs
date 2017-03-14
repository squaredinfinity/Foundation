using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Extensions;

namespace SquaredInfinity.Diagnostics.Fluent
{
    public class DiagnosticEventBuilder : IHideObjectInheritedMembers
    {
        ILogger Logger { get; set; }

        internal DiagnosticEvent EventToBuild { get; private set; }

        public ApplicationLifecycleEvent ApplicationLifecycleEvent { get; private set; }
        public SecurityEvent SecurityEvent { get; set; }

        public DiagnosticEventBuilder(ILogger logger)
        {
            this.Logger = logger;
            this.EventToBuild = new DiagnosticEvent();

            this.ApplicationLifecycleEvent = new ApplicationLifecycleEvent(this);
            this.SecurityEvent = new SecurityEvent(this);
        }

        public DiagnosticEventBuilder Append(Exception ex)
        {
            if (EventToBuild.ExceptionObject == null)
            {
                EventToBuild.ExceptionObject = ex;
            }
            else
            {
                EventToBuild.AttachedObjects.Add(new AttachedObject("Additional Exception Object", ex));
            }

            return this;
        }

        public DiagnosticEventBuilder AppendProperty(string propertyName, object value)
        {
            EventToBuild.Properties.AddOrUpdate(propertyName, value);

            return this;
        }

        public DiagnosticEventBuilder AppendLink(LinkTypes linkType, string link)
        {
            EventToBuild.Properties.AddOrUpdate("Link." + linkType.ToString(), link);

            return this;
        }

        public DiagnosticEventBuilder AppendLink(LinkTypes linkType, Func<string> getLink)
        {
            var link = string.Empty;

            try
            {
                link = getLink();
            }
            catch (Exception ex)
            {
                link = ex.DumpToString();
            }

            EventToBuild.Properties.AddOrUpdate("Link." + linkType.ToString(), link);

            return this;
        }

        public DiagnosticEventBuilder AppendObject(string name, object value)
        {
            EventToBuild.AttachedObjects.Add(new AttachedObject(name, value));

            return this;
        }

        public void Log()
        {
            Logger.ProcessDiagnosticEvent(EventToBuild);
        }

        public void Log(string message)
        {
            EventToBuild.Message = message;
            Logger.ProcessDiagnosticEvent(EventToBuild);
        }

        public void Log(Func<string> getMessage)
        {
            EventToBuild.GetMessage = getMessage;
            Logger.ProcessDiagnosticEvent(EventToBuild);
        }

        public void LogEvent(string category)
        {
            EventToBuild.Category = category;
            Logger.ProcessDiagnosticEvent(EventToBuild);
        }
    }
}
