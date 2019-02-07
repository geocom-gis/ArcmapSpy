// Copyright © 2018 geocom.ch
// This program is subject to the terms of the GPL-3.0 License.
// You should have received a copy of the GNU General Public License
// along with this program. If not, see http://www.gnu.org/licenses/.

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ArcmapSpy.Utils
{
    /// <summary>
    /// Helper functions to work with context menu related Windows API functions.
    /// </summary>
    internal static class ContextMenuUtils
    {
        public static void ShowContextMenu(ContextMenuStrip menu, Point position, IntPtr hWnd)
        {
            Point pointOnScreen = PointToScreen(position, hWnd);
            GMouseCaptureForm form = new GMouseCaptureForm(menu, pointOnScreen);
            form.ShowDialog();
        }

        private static Point PointToScreen(Point position, IntPtr hWnd)
        {
            POINT point = new POINT() {x = position.X, y = position.Y};
            HandleRef windowHandleRef = new HandleRef(null, hWnd);
            HandleRef nullHandleRef = new HandleRef(null, IntPtr.Zero);
            MapWindowPoints(windowHandleRef, nullHandleRef, point, 1);
            return new Point(point.x, point.y);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int MapWindowPoints(
            HandleRef hWndFrom, HandleRef hWndTo, [In] [Out] POINT pt, int cPoints);

        [StructLayout(LayoutKind.Sequential)]
        private class POINT
        {
            public int x;
            public int y;
        }
    }

    internal class GMouseCaptureForm : Form
    {
        private ContextMenuStrip _menu;
        private Point _position;
        private bool _isShown = false;

        private const int WM_NCACTIVATE = 0x86;

        public GMouseCaptureForm(ContextMenuStrip menu, Point position)
        {
            _menu = menu;
            _menu.Closed += OnMenuClosed;
            _position = position;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.Silver;
            this.TransparencyKey = Color.Silver;
            this.SetBounds(0, 0, 1, 1);
            this.Shown += OnFormShown;
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (_isShown && m.Msg == WM_NCACTIVATE)
            {
                _isShown = false;
                Close();
            }
        }

        private void OnFormShown(object sender, EventArgs e)
        {
            _menu.Show(_position);
            _isShown = true;
        }

        private void OnMenuClosed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            Close();
        }
    }
}
