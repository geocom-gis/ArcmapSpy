// Copyright © 2018 geocom.ch
// This program is subject to the terms of the GPL-3.0 License.
// You should have received a copy of the GNU General Public License
// along with this program. If not, see http://www.gnu.org/licenses/.

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ArcmapSpy.Utils
{
    /// <summary>
    /// Helper functions to work with window related Windows API functions.
    /// </summary>
    internal abstract class WindowsUtils
    {
        public const int GWL_HWNDPARENT = -8;

        // Microsoft.Win32.UnsafeNativeMethods
        [DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "SetWindowLong")]
        private static extern IntPtr SetWindowLongPtr32(HandleRef hWnd, int nIndex, HandleRef dwNewLong);

        // Microsoft.Win32.UnsafeNativeMethods
        [DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(HandleRef hWnd, int nIndex, HandleRef dwNewLong);

        /// <summary>
        /// Makes a modeless dialog stay above a parent window (ArcMap window handle), without making it modal.
        /// </summary>
        /// <param name="modelessDialog">Form to keep on top.</param>
        /// <param name="parentWindowsHandle">Handle of the parent window.</param>
        public static void MakeModelessDialogParentOf(Form modelessDialog, IntPtr parentWindowsHandle)
        {
            HandleRef dialogHandle = new HandleRef(modelessDialog, modelessDialog.Handle);
            HandleRef parentHandle = new HandleRef(null, parentWindowsHandle);
            if (IntPtr.Size == 4)
                SetWindowLongPtr32(dialogHandle, GWL_HWNDPARENT, parentHandle);
            else
                SetWindowLongPtr64(dialogHandle, GWL_HWNDPARENT, parentHandle);
        }
    }
}
