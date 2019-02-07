// Copyright © 2018 geocom.ch
// This program is subject to the terms of the GPL-3.0 License.
// You should have received a copy of the GNU General Public License
// along with this program. If not, see http://www.gnu.org/licenses/.

using System;
using ArcmapSpy.Utils;
using ArcmapSpy.Views;

namespace ArcmapSpy
{
    /// <summary>
    /// ArcMap command which allows to explore the table of content (TOC) of the active map.
    /// </summary>
    public class LayerSpyCommand : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LayerSpyCommand"/> class.
        /// </summary>
        public LayerSpyCommand()
        {
            AppStartup.OnStartup();
        }

        protected override void OnClick()
        {
            LayerSpyView form = new LayerSpyView();
            WindowsUtils.MakeModelessDialogParentOf(form, new IntPtr(ArcMap.Application.hWnd));
            form.Show();
        }

        protected override void OnUpdate()
        {
        }
    }
}
