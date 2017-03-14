using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace SquaredInfinity.Win32Api
{
    public partial class credui
    {
        [DllImport("credui", CharSet = CharSet.Unicode)]
        public static extern CRED_UI_RETURN_CODES CredUIPromptForCredentialsW(
            ref CREDUI_INFO creditUR,
            string targetName,
            IntPtr reserved1,
            int iError,
            StringBuilder userName,
            int maxUserName,
            StringBuilder password,
            int maxPassword,
            [MarshalAs(UnmanagedType.Bool)] ref bool pfSave,
            CREDUI_FLAGS flags);

        [DllImport("credui.dll", EntryPoint = "CredUIConfirmCredentialsW", CharSet = CharSet.Unicode)]
        public static extern CRED_UI_RETURN_CODES CredUIConfirmCredentials(
            string targetName,
            [MarshalAs(UnmanagedType.Bool)] bool confirm);

        [DllImport("credui.dll", EntryPoint = "CredUIParseUserNameW", CharSet = CharSet.Unicode)]
        public static extern CRED_UI_RETURN_CODES CredUIParseUserName(
            string userName,
            StringBuilder user,
            int userMaxChars,
            StringBuilder domain,
            int domainMaxChars);
    }
}
