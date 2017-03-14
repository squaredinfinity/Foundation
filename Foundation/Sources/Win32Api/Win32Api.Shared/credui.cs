using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Interop;

namespace SquaredInfinity.Win32Api
{
    public partial class credui
    {
        public static bool PromptForCredentials(
            string caption,
            string message,
            string targetName,
            Bitmap bannerBitmap,
            out string userName,
            out string domainName,
            out string password)
        {
            var success = false;

            var ci = new credui.CREDUI_INFO();
            ci.cbSize = Marshal.SizeOf(ci);
            ci.pszCaptionText = caption;
            ci.pszMessageText = message;

            try
            {
                var curApp = System.Windows.Application.Current;

                if (curApp != null)
                {
                    var mainWindow = curApp.MainWindow;

                    if (mainWindow != null)
                    {
                        ci.hwndParent = new WindowInteropHelper(mainWindow).Handle;
                    }
                }
            }
            catch {/* ignore, worst case scenario it won't be a modal window */}


            if (bannerBitmap != null)
                ci.hbmBanner = bannerBitmap.GetHbitmap();

            bool save = false;

            var userFullNameSB = new StringBuilder(255);
            var passwordSB = new StringBuilder(255);

            var flags =
                credui.CREDUI_FLAGS.GENERIC_CREDENTIALS
                | credui.CREDUI_FLAGS.SHOW_SAVE_CHECK_BOX
                | credui.CREDUI_FLAGS.ALWAYS_SHOW_UI
                | credui.CREDUI_FLAGS.EXPECT_CONFIRMATION
                | credui.CREDUI_FLAGS.REQUEST_ADMINISTRATOR;

            var promptResult =
                credui.CredUIPromptForCredentialsW(
                ref ci,
                targetName,
                IntPtr.Zero,
                0,
                userFullNameSB,
                255,
                passwordSB,
                255,
                ref save,
                flags);

            var userNameSb = new StringBuilder(255);
            var userDomainSb = new StringBuilder(255);

            if (promptResult == credui.CRED_UI_RETURN_CODES.NO_ERROR)
            {
                var parseResult = credui.CredUIParseUserName(userFullNameSB.ToString(), userNameSb, 255, userDomainSb, 255);

                // try to access resource, if worked then confirm credentials (if save == true)
                if (save)
                {
                    var confirmResult = credui.CredUIConfirmCredentials(targetName, true);
                }
            }

            userName = userNameSb.ToString();
            domainName = userDomainSb.ToString();
            password = passwordSB.ToString();

            success = true;

            return success;
        }
    }
}
