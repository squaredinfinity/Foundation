using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Foundation.Settings
{
    public enum SettingScope
    {
        /// <summary>
        /// All scopes
        /// </summary>
        All,

        /// <summary>
        /// Global for all users
        /// </summary>
        Global,

        /// <summary>
        /// Global for current user
        /// </summary>
        User,

        /// <summary>
        /// Machine for all users
        /// </summary>
        Machine,

        /// <summary>
        /// Machine for current user
        /// </summary>
        UserMachine,
    }
}
