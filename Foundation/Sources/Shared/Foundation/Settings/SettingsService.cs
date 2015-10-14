using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Foundation.Settings
{
    public abstract class SettingsService : ISettingsService
    {
        ISerialzier _defaultSerializer;
        protected ISerialzier DefaultSerializer { get; private set; }
        protected string DefaultApplication { get; private set; }
        protected string DefaultContainer { get; private set; }

        protected IReadOnlyList<SettingScope> ScopesByPriority { get; private set; }

        public SettingsService(ISerialzier defaultSerializer)
            : this("", "", defaultSerializer)
        { }

        public SettingsService(string defaultApplication, string defaultContainer, ISerialzier defaultSerializer)
        {
            this.DefaultApplication = defaultApplication;
            this.DefaultContainer = defaultContainer;

            this.DefaultSerializer = defaultSerializer;
            this.ScopesByPriority = GetScopesByPriority();
        }

        #region Get Current Machine / User / Scopes

        protected virtual IReadOnlyList<SettingScope> GetScopesByPriority()
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

        public T GetSetting<T>(string key, ISerialzier serializer, Func<T> defaultValue)
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

        public T GetSetting<T>(string key, SettingScope scope, string machineName, string userName, Func<T> defaultValue)
        {
            return GetSetting<T>(key, scope, machineName, userName, defaultValue);
        }

        public T GetSetting<T>(string key, SettingScope scope, Func<T> defaultValue)
        {
            return GetSetting<T>(key, scope, GetCurrentMachineName(), GetCurrentUserName(), DefaultSerializer, defaultValue);
        }

        public T GetSetting<T>(string key, SettingScope scope, string machineName, string userName, ISerialzier serializer, Func<T> defaultValue)
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

        public bool TryGetSetting<T>(string key, SettingScope scope, string machineName, string userName, out T value)
        {
            return TryGetSetting<T>(key, scope, machineName, userName, DefaultSerializer, out value);
        }

        public bool TryGetSetting<T>(string key, ISerialzier serializer, out T value)
        {
            return TryGetSetting<T>(key, SettingScope.All, GetCurrentMachineName(), GetCurrentUserName(), DefaultSerializer, out value);
        }

        public bool TryGetSetting<T>(string key, SettingScope scope, string machineName, string userName, ISerialzier serializer, out T value)
        {
            return TryGetSetting<T>(DefaultApplication, DefaultContainer, key, scope, machineName, userName, serializer, out value);
        }
        public bool TryGetSetting<T>(string application, string container, string key, ISerialzier serializer, out T value)
        {
            return TryGetSetting<T>(application, container, key, SettingScope.All, GetCurrentMachineName(), GetCurrentUserName(), serializer, out value);
        }

        public bool TryGetSetting<T>(string application, string container, string key, SettingScope scope, string machineName, string userName, out T value)
        {
            return TryGetSetting<T>(application, container, key, scope, machineName, userName, DefaultSerializer, out value);
        }

        public bool TryGetSetting<T>(string application, string container, string key, SettingScope scope, string machineName, string userName, ISerialzier serializer, out T value)
        {
            value = default(T);
            var data = (byte[])null;
            var data_encoding = (Encoding)null;

            if (scope == SettingScope.All)
            {
                foreach (var _scope in ScopesByPriority)
                {
                    if (scope.HasFlag(_scope))
                    {
                        if (DoTryGetSetting(application, container, key, _scope, machineName, userName, out data_encoding, out data))
                            break;
                    }
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

        public void SetSetting<T>(string key, SettingScope scope, ISerialzier serializer, T value)
        {
            SetSetting<T>(key, scope, GetCurrentMachineName(), GetCurrentUserName(), serializer, value);
        }

        public void SetSetting<T>(string key, SettingScope scope, T value)
        {
            SetSetting<T>(key, scope, GetCurrentMachineName(), GetCurrentUserName(), DefaultSerializer, value);
        }

        public void SetSetting<T>(string application, string container, string key, SettingScope scope, string machineName, string userName, T value)
        {
            SetSetting<T>(key, scope, machineName, userName, DefaultSerializer, value);
        }

        public void SetSetting<T>(string key, SettingScope scope, string machineName, string userName, ISerialzier serializer, T value)
        {
            SetSetting<T>(DefaultApplication, DefaultContainer, key, scope, machineName, userName, serializer, value);
        }

        public void SetSetting<T>(string key, SettingScope scope, string machineName, string userName, T value)
        {
            SetSetting<T>(DefaultApplication, DefaultContainer, key, scope, machineName, userName, DefaultSerializer, value);
        }

        public void SetSetting<T>(string application, string container, string key, SettingScope scope, string machineName, string userName, ISerialzier serializer, T value)
        {
            if (scope == SettingScope.All)
                throw new ArgumentException("scope must be a specific value, not All");

            var serialized_data = serializer.Serialize<T>(value);

            DoSetSetting<T>(application, container, key, scope, machineName, userName, serialized_data.Encoding, serialized_data.Bytes);
        }

        #endregion


        public abstract void DoSetSetting<T>(
            string application,
            string container,
            string key,
            SettingScope scope,
            string machineName,
            string userName,
            Encoding valueEncoding,
            byte[] value);

        public abstract bool DoTryGetSetting(
            string application,
            string container,
            string key,
            SettingScope scope,
            string machineName,
            string userName,
            out Encoding valueEncoding,
            out byte[] value);
    }
}
