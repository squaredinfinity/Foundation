using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Foundation.Settings
{
    public abstract class SettingsService : ISettingsService
    {
        public event EventHandler<SettingChangedEventArgs> AfterSettingChanged;
        void RaiseAfterSettingChanged(string application, string container, string key)
        {
            if (AfterSettingChanged != null)
                AfterSettingChanged(this, new SettingChangedEventArgs(application, container, key));
        }

        public event EventHandler<EventArgs> AfterSettingsChanged;
        void RaiseAfterSettingsChanged()
        {
            if(AfterSettingsChanged != null)
                AfterSettingsChanged(this, EventArgs.Empty));
        }

        protected ISerializer DefaultSerializer { get; private set; }
        protected string DefaultApplication { get; private set; }
        protected string DefaultContainer { get; private set; }

        protected IReadOnlyList<int> ScopesByPriority { get; private set; }

        public SettingsService(ISerializer defaultSerializer)
            : this("", "", defaultSerializer)
        { }

        public SettingsService(string defaultApplication, string defaultContainer, ISerializer defaultSerializer)
        {
            this.DefaultApplication = defaultApplication;
            this.DefaultContainer = defaultContainer;

            this.DefaultSerializer = defaultSerializer;
            this.ScopesByPriority = GetScopesByPriority();
        }

        #region Get Current Machine / User / Scopes

        protected virtual IReadOnlyList<int> GetScopesByPriority()
        {
            return new[] { SettingScope.UserMachine, SettingScope.User, SettingScope.Machine, SettingScope.Global };
        }

        protected virtual string GetCurrentMachineName()
        {
            return Environment.MachineName;
        }

        protected virtual string GetCurrentUserName()
        {
            return Environment.UserName + "@" + Environment.UserDomainName;
        }

        #endregion

        #region (Try) Get Setting

        public T GetSetting<T>(string key, Func<T> defaultValue)
        {
            return GetSetting<T>(key, DefaultSerializer, defaultValue);
        }

        public T GetSetting<T>(string key, ISerializer serializer, Func<T> defaultValue)
        {
            var value = default(T);

            if (TryGetSetting<T>(key, serializer, out value))
            {
                return value;
            }
            else
            {
                return defaultValue();
            }
        }

        public T GetSetting<T>(string key, int scope, string machineName, string userName, Func<T> defaultValue)
        {
            return GetSetting<T>(key, scope, machineName, userName, defaultValue);
        }

        public T GetSetting<T>(string key, int scope, Func<T> defaultValue)
        {
            return GetSetting<T>(key, scope, GetCurrentMachineName(), GetCurrentUserName(), DefaultSerializer, defaultValue);
        }

        public T GetSetting<T>(string key, int scope, string machineName, string userName, ISerializer serializer, Func<T> defaultValue)
        {
            var value = default(T);

            if (TryGetSetting<T>(key, scope, machineName, userName, serializer, out value))
            {
                return value;
            }
            else
            {
                return defaultValue();
            }
        }

        public bool TryGetSetting<T>(string key, out T value)
        {
            return TryGetSetting<T>(key, DefaultSerializer, out value);
        }

        public bool TryGetSetting<T>(string key, int scope, string machineName, string userName, out T value)
        {
            return TryGetSetting<T>(key, scope, machineName, userName, DefaultSerializer, out value);
        }

        public bool TryGetSetting<T>(string key, int scope, out T value)
        {
            return TryGetSetting<T>(key, scope, GetCurrentMachineName(), GetCurrentUserName(), DefaultSerializer, out value);
        }

        public bool TryGetSetting<T>(string key, ISerializer serializer, out T value)
        {
            return TryGetSetting<T>(key, SettingScope.All, GetCurrentMachineName(), GetCurrentUserName(), DefaultSerializer, out value);
        }

        public bool TryGetSetting<T>(string key, int scope, string machineName, string userName, ISerializer serializer, out T value)
        {
            return TryGetSetting<T>(DefaultApplication, DefaultContainer, key, scope, machineName, userName, serializer, out value);
        }
        public bool TryGetSetting<T>(string application, string container, string key, ISerializer serializer, out T value)
        {
            return TryGetSetting<T>(application, container, key, SettingScope.All, GetCurrentMachineName(), GetCurrentUserName(), serializer, out value);
        }

        public bool TryGetSetting<T>(string application, string container, string key, int scope, string machineName, string userName, out T value)
        {
            return TryGetSetting<T>(application, container, key, scope, machineName, userName, DefaultSerializer, out value);
        }

        public bool TryGetSetting<T>(string application, string container, string key, int scope, string machineName, string userName, ISerializer serializer, out T value)
        {
            value = default(T);
            var data = (byte[])null;
            var data_encoding = (Encoding)null;

            if (scope == SettingScope.All)
            {
                foreach (var _scope in ScopesByPriority)
                {
                    if (DoTryGetSetting(application, container, key, _scope, machineName, userName, out data_encoding, out data))
                        break;
                }

                if (data == null || data_encoding == null)
                    return false;
            }
            else
            {
                if (!DoTryGetSetting(application, container, key, scope, machineName, userName, out data_encoding, out data))
                    return false;
            }

            var sdi = new SerializedDataInfo(data, data_encoding);

            value = serializer.Deserialize<T>(sdi);
            return true;
        }

        #endregion

        #region Set Setting

        public void SetSetting<T>(string key, int scope, ISerializer serializer, T value)
        {
            SetSetting<T>(key, scope, GetCurrentMachineName(), GetCurrentUserName(), serializer, value);
        }

        public void SetSetting<T>(string key, int scope, T value)
        {
            SetSetting<T>(key, scope, GetCurrentMachineName(), GetCurrentUserName(), DefaultSerializer, value);
        }

        public void SetSetting<T>(string application, string container, string key, int scope, string machineName, string userName, T value)
        {
            SetSetting<T>(key, scope, machineName, userName, DefaultSerializer, value);
        }

        public void SetSetting<T>(string key, int scope, string machineName, string userName, ISerializer serializer, T value)
        {
            SetSetting<T>(DefaultApplication, DefaultContainer, key, scope, machineName, userName, serializer, value);
        }

        public void SetSetting<T>(string key, int scope, string machineName, string userName, T value)
        {
            SetSetting<T>(DefaultApplication, DefaultContainer, key, scope, machineName, userName, DefaultSerializer, value);
        }

        public void SetSetting<T>(string application, string container, string key, int scope, string machineName, string userName, ISerializer serializer, T value)
        {
            if (scope == SettingScope.All)
                throw new ArgumentException("scope must be a specific value, not All");

            var serialized_data = serializer.Serialize<T>(value);

            DoSetSetting<T>(application, container, key, scope, machineName, userName, serialized_data.Encoding, serialized_data.Bytes);

            RaiseAfterSettingChanged(application, container, key);
        }

        #endregion


        public abstract void DoSetSetting<T>(
            string application,
            string container,
            string key,
            int scope,
            string machineName,
            string userName,
            Encoding valueEncoding,
            byte[] value);

        public abstract bool DoTryGetSetting(
            string application,
            string container,
            string key,
            int scope,
            string machineName,
            string userName,
            out Encoding valueEncoding,
            out byte[] value);
    }
}
