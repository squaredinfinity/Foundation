using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Diagnostics
{
    public interface ILogger
    {
        #region Critical

        void Critical(Exception ex, string message = null);
        void Critical(Exception ex, Func<string> getMessage);

        void Critical(string message);
        void Critical(Func<string> getMessage);

        #endregion

        #region Error

        void Error(Exception ex, string message = null);
        void Error(Exception ex, Func<string> getMessage);

        void Error(string message);
        void Error(Func<string> getMessage);

        #endregion

        #region Warning

        void Warning(Exception ex, string message = null);
        void Warning(Exception ex, Func<string> getMessage);

        void Warning(string message);
        void Warning(Func<string> getMessage);

        #endregion

        #region Information

        void Information(Exception ex, string message = null);
        void Information(Exception ex, Func<string> getMessage);

        void Information(string message);
        void Information(Func<string> getMessage);

        #endregion

        #region Verbose

        void Verbose(Exception ex, string message = null);
        void Verbose(Exception ex, Func<string> getMessage);

        void Verbose(string message);
        void Verbose(Func<string> getMessage);

        #endregion
    }
}
