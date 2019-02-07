// Copyright © 2018 geocom.ch
// This program is subject to the terms of the GPL-3.0 License.
// You should have received a copy of the GNU General Public License
// along with this program. If not, see http://www.gnu.org/licenses/.

using System;
using System.Collections.Generic;
using System.Linq;
using ArcmapSpy.Utils;
using ArcmapSpy.ViewModels;
using ArcmapSpy.Views;
using ESRI.ArcGIS.Carto;

namespace ArcmapSpy
{
    /// <summary>
    /// ArcMap tool to explore a single picked feature.
    /// </summary>
    public class GeometrySpyTool : ESRI.ArcGIS.Desktop.AddIns.Tool
    {
        private List<IFeatureLayer> _pickLayers;
        private ArcmapPick _pick;

        /// <summary>
        /// Initailizes a new instance of the <see cref="GeometrySpyTool"/> class.
        /// </summary>
        public GeometrySpyTool()
        {
            AppStartup.OnStartup();
        }

        /// <summary>
        /// This event is called, when the ArcMap tool is activated.
        /// </summary>
        protected override void OnActivate()
        {
            try
            {
                _pickLayers = ArcmapLayerUtils.EnumerateLayers(ArcmapUtils.GetFocusMap(), false).ToList();
            }
            catch (Exception)
            {
                // Don't let the exeption escape to arcgis.
            }
        }

        /// <summary>
        /// This event is called, when the ArcMap tool is deactivated.
        /// </summary>
        /// <returns>Return true if the tool can be deactivated.</returns>
        protected override bool OnDeactivate()
        {
            try
            {
                ResetPickTool();
            }
            catch (Exception)
            {
                // Don't let the exeption escape to arcgis.
            }
            return true;
        }

        /// <summary>
        /// This event is called, when the user released the mouse button in ArcMap.
        /// </summary>
        /// <param name="arg">Event arguments</param>
        protected override void OnMouseUp(MouseEventArgs arg)
        {
            try
            {
                ArcmapPick pick = LazyLoadPickTool();
                ArcmapPickCandidate pickedCandidate = pick.OnMouseUp(arg.X, arg.Y);
                if (pickedCandidate != null)
                {
                    ArcmapUtils.FlashFeature(pickedCandidate.Feature, ArcmapUtils.FocusMapToScreenDisplay(pick.FocusMap));
                    GeometrySpyViewModel viewModel = new GeometrySpyViewModel(pickedCandidate.Feature);
                    GeometrySpyView form = new GeometrySpyView();
                    form.SetDataContext(viewModel);
                    WindowsUtils.MakeModelessDialogParentOf(form, new IntPtr(ArcMap.Application.hWnd));
                    form.Show();
                }
            }
            catch (Exception)
            {
                // Don't let the exeption escape to arcgis.
                ResetPickTool();
            }
            ArcmapUtils.RefreshMap(ArcmapUtils.GetFocusMap());
        }

        private ArcmapPick LazyLoadPickTool()
        {
            if (_pick == null)
            {
                _pick = new ArcmapPick(_pickLayers, ArcmapUtils.GetFocusMap(), 10);
            }
            return _pick;
        }

        private void ResetPickTool()
        {
            _pick = null;
        }
    }
}
