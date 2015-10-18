using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Foundation.Settings
{
    public class SettingChangedEventArgs : EventArgs
    {
        public string Application { get; private set; }
        public string Container { get; private set; }
        public string Key { get; private set; }

        public SettingChangedEventArgs(string application, string container, string key)
        {
            this.Application = application;
            this.Container = container;
            this.Key = key;
        }
    }
}
