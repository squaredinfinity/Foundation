using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Foundation.Settings
{
    /// <summary>
    /// Contains values of valid setting scopes.
    /// Note that valiues [0-99] are reserved.
    /// Custom implementations of Settings Service that include additional scopes should start from 100.
    /// </summary>
    public class SettingScope
    {
        /// <summary>
        /// All scopes
        /// </summary>
        public static readonly int All = 0;

        /// <summary>
        /// Global for all users
        /// </summary>
        public static readonly int Global = 1;

        /// <summary>
        /// Global for current user
        /// </summary>
        public static readonly int User = 2;

        /// <summary>
        /// Machine for all users
        /// </summary>
        public static readonly int Machine = 3;

        /// <summary>
        /// Machine for current user
        /// </summary>
        public static readonly int UserMachine = 4;
    }
}
