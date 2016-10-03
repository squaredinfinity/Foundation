using SquaredInfinity.Foundation.Diagnostics.Sinks;
using System;
using System.Collections.Generic;
using System.Text;
using SquaredInfinity.Foundation.Diagnostics;
using SquaredInfinity.Foundation.Diagnostics.Formatters;
using System.Net.Mail;
using System.Linq;
using System.IO;
using System.IO.Compression;
using SquaredInfinity.Foundation.Extensions;
using SquaredInfinity.Foundation.Diagnostics.ContextDataCollectors;

namespace SquaredInfinity.Foundation.Diagnostics.Sinks.Email
{
    public class EmailSink : Sink
    {
        public string To { get; set; }
        public string From { get; set; }

        public IFormatter Body { get; set; }

        public IFormatter Subject { get; set; }

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
            //if (Bundle != null)
            //{
            //    if (Bundle.TryBundle(de))
            //    {
            //        // event added to the bundle
            //        // do nothing else
            //        return;
            //    }
            //    else
            //    {
            //        // event not added to bundle,
            //        // proceed as usual
            //    }
            //}

            // TODO: allow to specify smtp config in diag config rather than always using one from app.config

            // TODO: this is just a quick mockup to get it working for demo, must be rewriten properly

            PatternFormatter f = new PatternFormatter();

            var msg = new MailMessage();

            var toElements = To.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < toElements.Length; i++)
            {
                f = new PatternFormatter();
                f.Pattern = toElements[i];

                var toformatted = f.Format(de);
                msg.To.Add(toformatted);
            }

            msg.From = new MailAddress(From);

            msg.Subject = Subject.Format(de);

            msg.Body = Body.Format(de);

            if (!LogsToAttach.IsNullOrEmpty())
            {
                // todo: support multiple logs

                throw new NotImplementedException();

                //var config_ref = GlobalDiagnostics.GlobalLogger.Config;

                //var sink =
                //    (from s in config_ref.SinkDefinitions
                //     where s is FileSink
                //     && s.Name == LogsToAttach
                //     select s as FileSink).FirstOrDefault();


                //var attachmentStream = new MemoryStream();

                //using (var l = sink.FileLockingStrategy.AcquireLock(sink.LogFile.FullName))
                //{
                //    using (var fs = new FileStream(sink.SinkLocation.Location, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                //    {
                //        using (var gz = new GZipStream(attachmentStream, CompressionMode.Compress, leaveOpen: true))
                //        {
                //            fs.CopyTo(gz);
                //        }
                //    }
                //}

                //if (attachmentStream != null)
                //{
                //    attachmentStream.Seek(0, SeekOrigin.Begin);

                //    var attachment = new Attachment(attachmentStream, sink.LogFile.Name + ".gz");
                //    msg.Attachments.Add(attachment);
                //}
            }


            SmtpClient.Send(msg);
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

        public override IReadOnlyList<IDataRequest> GetRequestedContextData()
        {
            var l1 = Body.GetRequestedContextData();

            PatternFormatter f = new PatternFormatter();
            f.Pattern = To;

            var l2 = f.GetRequestedContextData();

            var l3 = Subject.GetRequestedContextData();

            return l1.Concat(l2).Concat(l3).ToList();
        }
    }
}
