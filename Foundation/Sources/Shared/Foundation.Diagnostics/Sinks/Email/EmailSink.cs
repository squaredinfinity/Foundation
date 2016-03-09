using SquaredInfinity.Foundation.Diagnostics.Sinks;
using System;
using System.Collections.Generic;
using System.Text;
using SquaredInfinity.Foundation.Diagnostics;
using SquaredInfinity.Foundation.Diagnostics.Formatters;
using System.Net.Mail;

namespace Foundation.Diagnostics.Sinks.Email
{
    public class EmailSink : Sink
    {
        public string To { get; set; }
        public string From { get; set; }

        public IFormatter BodyFormatter { get; set; }

        public IFormatter SubjectFormatter { get; set; }

        public string LogsToAttach { get; set; }

        SmtpClient _smtpClient;
        SmtpClient SmtpClient
        {
            get
            {
                if (_smtpClient == null)
                    _smtpClient = new SmtpClient();

                return _smtpClient;
            }
        }

        public override ISinkLocation SinkLocation
        {
            get
            {
                return new SinkLocation("email", To);
            }
        }

        public override void Write(IDiagnosticEvent de)
        {
            throw new NotImplementedException();
        }

        public override void OnApplicationExit()
        {
            base.OnApplicationExit();

            // todo: bundle and handling of application exit should be moved lower in the hierarchy
            //if (Bundle != null && Bundle.ReleaseOnAppExit)
            //{
            //    Bundle.ReleaseAllBundles();
            //}
        }
    }
}
