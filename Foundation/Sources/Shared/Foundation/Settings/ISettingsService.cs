using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Foundation.Settings
{ 
    public interface ISettingsService
    {
        event EventHandler<SettingChangedEventArgs> AfterSettingChanged;
        event EventHandler<EventArgs> AfterSettingsChanged;

        #region (Try) Get Setting + Scope

        T GetSetting<T>(
            string key,
            int scope,
            string machineName,
            string userName,
            ISerializer serializer,
            Func<T> defaultValue);

        T GetSetting<T>(
            string key,
            int scope,
            Func<T> defaultValue);

        bool TryGetSetting<T>(
            string key,
            int scope,
            string machineName,
            string userName,
            ISerializer serializer,
            out T value);

        T GetSetting<T>(
            string key,
            int scope,
            string machineName,
            string userName,
            Func<T> defaultValue);

        bool TryGetSetting<T>(
            string key,
            int scope,
            string machineName,
            string userName,
            out T value);

        bool TryGetSetting<T>(
            string key,
            int scope,
            out T value);

        bool TryGetSetting<T>(
            string application,
            string container,
            string key,
            int scope,
            string machineName,
            string userName,
            out T value);

        bool TryGetSetting<T>(
            string application,
            string container,
            string key,
            int scope,
            string machineName,
            string userName,
            ISerializer serialzier,
            out T value);

        #endregion

        #region (Try) Get Setting | All Scopes

        /// <summary>
        /// Gets a value of a specified setting.
        /// Checks for setting in order: UserMachine, User, Machine, Global
        /// </summary>
        /// <typeparam name="T">type of setting</typeparam>
        /// <param name="key">name of setting</param>
        /// <param name="defaultValue">constructs default setting value if no actual value could be found</param>
        /// <returns>setting value, result of defaultValue function if no setting could be found on any level</returns>
        T GetSetting<T>(string key, Func<T> defaultValue);

        /// <summary>
        /// Gets a value of a specified setting.
        /// Checks for setting in order: UserMachine, User, Machine, Global
        /// </summary>
        /// <typeparam name="T">type of setting</typeparam>
        /// <param name="key">name of setting</param>
        /// <param name="value">setting value</param>
        /// <returns>true if setting has been found, false otherwise</returns>
        bool TryGetSetting<T>(string key, out T value);

        /// <summary>
        /// Gets a value of a specified setting.
        /// Checks for setting in order: UserMachine, User, Machine, Global
        /// </summary>
        /// <typeparam name="T">type of setting</typeparam>
        /// <param name="key">name of setting</param>
        /// <param name="serializer">serializer for the setting value</param>
        /// <param name="defaultValue">constructs default setting value if no actual value could be found</param>
        /// <returns>setting value, result of defaultValue function if no setting could be found on any level</returns>
        T GetSetting<T>(string key, ISerializer serializer, Func<T> defaultValue);

        /// <summary>
        /// Gets a value of a specified setting.
        /// Checks for setting in order: UserMachine, User, Machine, Global
        /// </summary>
        /// <typeparam name="T">type of setting</typeparam>
        /// <param name="key">name of setting</param>
        /// <param name="serializer">serializer for the setting value</param>
        /// <param name="value">setting value</param>
        /// <returns>true if setting has been found, false otherwise</returns>
        bool TryGetSetting<T>(string key, ISerializer serializer, out T value);

        /// <summary>
        /// Gets a value of a specified setting.
        /// Checks for setting in order: UserMachine, User, Machine, Global
        /// </summary>
        /// <typeparam name="T">type of setting</typeparam>
        /// <param name="application">application</param>
        /// <param name="container">container</param>
        /// <param name="key">name of setting</param>
        /// <param name="serializer">serializer for the setting value</param>
        /// <param name="value">setting value</param>
        /// <returns>true if setting has been found, false otherwise</returns>
        bool TryGetSetting<T>(string application, string container, string key, ISerializer serializer, out T value);

        #endregion

        #region Set Setting

        /// <summary>
        /// Sets a value of a specifed user setting
        /// </summary>
        /// <typeparam name="T">Type of setting value</typeparam>
        /// <param name="key">setting name</param>
        /// <param name="scope">setting scope</param>
        void SetSetting<T>(
            string key,
            int scope,
            T value);

        /// <summary>
        /// Sets a value of a specified user setting
        /// Allows to override user and machine names
        /// </summary>
        /// <typeparam name="T">Type of setting value</typeparam>
        /// <param name="key">setting name</param>
        /// <param name="scope">scope</param>
        /// <param name="machineName">machine name</param>
        /// <param name="userName">user name</param>
        void SetSetting<T>(
            string key,
            int scope,
            string machineName,
            string userName,
            T value);

        /// <summary>
        /// Sets a value of a specified user setting
        /// </summary>
        /// <typeparam name="T">type of setting value</typeparam>
        /// <param name="key">setting name</param>
        /// <param name="scope">scope</param>
        /// <param name="serializer">custom serializer</param>
        void SetSetting<T>(
            string key,
            int scope,
            ISerializer serializer,
            T value);

        /// <summary>
        /// Sets a value of a specified user setting
        /// Allows to override user and machine names
        /// </summary>
        /// <typeparam name="T">type of setting value</typeparam>
        /// <param name="key">setting name</param>
        /// <param name="scope">scope</param>
        /// <param name="machineName">machine name</param>
        /// <param name="userName">user name</param>
        /// <param name="serializer">custom serializer</param>
        void SetSetting<T>(
            string key,
            int scope,
            string machineName,
            string userName,
            ISerializer serializer,
            T value);

        /// <summary>
        /// Sets a value of a specified user setting
        /// Allows to override user and machine names.
        /// </summary>
        /// <typeparam name="T">type of setting value</typeparam>
        /// <param name="application">application scope</param>
        /// <param name="container">container</param>
        /// <param name="key">setting name</param>
        /// <param name="scope">scope</param>
        /// <param name="machineName">machine name</param>
        /// <param name="userName">user name</param>
        /// <param name="serializer">custom serialzier</param>
        /// <param name="value">value</param>
        void SetSetting<T>(
            string application,
            string container,
            string key,
            int scope,
            string machineName,
            string userName,
            ISerializer serializer,
            T value);

        #endregion


        //#region Clear Settings

        //void ClearSettings(string application);
        //void ClearSettings(string application, string container);
        //void ClearSettings(string application, string container, string userName);
        //void ClearSettings(string application, string container, string userName, string machineName);
        //void ClearSettings();

        //#endregion
    }
}
