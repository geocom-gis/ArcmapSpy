// Copyright © 2018 geocom.ch
// This program is subject to the terms of the GPL-3.0 License.
// You should have received a copy of the GNU General Public License
// along with this program. If not, see http://www.gnu.org/licenses/.

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace ArcmapSpy.Utils
{
    /// <summary>
    /// Implements a pick tool, that can be used inside the ArcMap environment,
    /// to easily select a feature with the mouse.
    /// </summary>
    internal class ArcmapPick
    {
        private const int popupShiftH = 20;
        private const int popupShiftV = 8;

        private readonly List<IFeatureLayer> _pickLayers;
        private readonly int _snapTolerance;
        private ArcmapPickCandidate _selectedCandidate;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArcmapPick"/> class.
        /// </summary>
        /// <param name="pickLayers">A list of layers, which contains features that
        /// can be snapped to. These features are accepted as result of the pick.</param>
        /// <param name="focusMap">Esri map with the focus.</param>
        /// <param name="snapTolerance">Tolerance in pixels for snapping.</param>
        public ArcmapPick(
            List<IFeatureLayer> pickLayers,
            IMap focusMap,
            int snapTolerance)
        {
            _pickLayers = pickLayers;
            FocusMap = focusMap;
            _snapTolerance = snapTolerance;
        }

        /// <summary>
        /// Gets the focus map of ArcMap
        /// </summary>
        public IMap FocusMap { get; private set; }

        /// <summary>
        /// Can be called from the mouse up event of the ArcMap tool.
        /// </summary>
        /// <param name="x">Clicked x coordinate.</param>
        /// <param name="y">Clicked y coordinate.</param>
        /// <returns>The picked feature candidate or null if no feature was picked.</returns>
        public ArcmapPickCandidate OnMouseUp(int x, int y)
        {
            ArcmapPickCandidate result = null;
            IScreenDisplay screenDisplay = ArcmapUtils.FocusMapToScreenDisplay(FocusMap);
            IPoint mapPoint = TransformMousePositionToMapPoint(x, y, screenDisplay);
            double searchTolerance = ArcmapUtils.PixelsToDistance(_snapTolerance, screenDisplay);
            double mapScale = GetCurrentMapScale(screenDisplay);

            List<ArcmapPickCandidate> pickCandidates = FindPickCandidates(mapPoint, _pickLayers, mapScale, searchTolerance);
            SortPickCandidates(pickCandidates);

            if (pickCandidates.Count == 1)
            {
                result = pickCandidates[0];
            }
            else if (pickCandidates.Count > 1)
            {
                System.Drawing.Point position = new System.Drawing.Point(x + popupShiftH, y + popupShiftV);
                ContextMenuStrip menu = BuildContextMenu(pickCandidates);
                menu.Opacity = 0.9;
                _selectedCandidate = null;
                ContextMenuUtils.ShowContextMenu(menu, position, (IntPtr)ArcmapUtils.FocusMapToScreenDisplay(FocusMap).hWnd);
                if (_selectedCandidate != null)
                {
                    result = _selectedCandidate;
                }
            }
            return result;
        }

        private static List<ArcmapPickCandidate> FindPickCandidates(
            IPoint mapPoint, List<IFeatureLayer> pickLayers, double mapScale, double searchTolerance)
        {
            List<ArcmapPickCandidate> result = new List<ArcmapPickCandidate>();

            if (pickLayers.Count > 0)
            {
                IEnvelope searchEnvelope = mapPoint.Envelope;
                searchEnvelope.Expand(searchTolerance, searchTolerance, false);

                foreach (IFeatureLayer pickLayer in pickLayers)
                {
                    if (ArcmapLayerUtils.LayerIsVisible(pickLayer, mapScale))
                    {
                        IIdentify identifier = pickLayer as IIdentify;
                        IArray elements = identifier.Identify(searchEnvelope);
                        foreach (object element in elements.Enumerate())
                        {
                            IFeature feature = (element as IRowIdentifyObject).Row as IFeature;
                            double distance;
                            IPoint footPoint;
                            CalculateNearestDistanceToFeature(mapPoint, feature, out distance, out footPoint);

                            ArcmapPickCandidate ranking = new ArcmapPickCandidate(element as IIdentifyObj, distance);
                            result.Add(ranking);
                        }
                    }
                }
            }
            return result;
        }

        private static void CalculateNearestDistanceToFeature(
            IPoint point, IFeature feature, out double distance, out IPoint footPoint)
        {
            IGeometry geometry = feature.Shape;
            IProximityOperator proximity = geometry as IProximityOperator;
            distance = proximity.ReturnDistance(point);
            footPoint = proximity.ReturnNearestPoint(point, esriSegmentExtension.esriNoExtension);
        }

        private static void SortPickCandidates(List<ArcmapPickCandidate> candidates)
        {
            candidates.Sort((item1, item2) => item1.CompareTo(item2));
        }

        /// <summary>
        /// Transformates the position of the cursor (screen coordinates) to a point
        /// on the map (map coordinates).
        /// </summary>
        /// <param name="x">East coordinate of the mouse.</param>
        /// <param name="y">North coordinate of the mouse.</param>
        /// <param name="display">Map object, we want the coordinate for.</param>
        /// <returns>Point with map coordinates.</returns>
        private static IPoint TransformMousePositionToMapPoint(int x, int y, IScreenDisplay display)
        {
            return display.DisplayTransformation.ToMapPoint(x, y);
        }

        private static double GetCurrentMapScale(IScreenDisplay display)
        {
            return display.DisplayTransformation.ScaleRatio;
        }

        private ContextMenuStrip BuildContextMenu(IEnumerable<ArcmapPickCandidate> snapCandidates)
        {
            ContextMenuStrip mnuContextMenu = new ContextMenuStrip();
            foreach (ArcmapPickCandidate candidate in snapCandidates)
            {
                string title = string.Format(
                    "{0} ({1})",
                    candidate.IdentifyObj.Layer.Name,
                    candidate.Feature.OID);
                ToolStripItem mnuItem = mnuContextMenu.Items.Add(title);
                mnuItem.Tag = candidate;
                mnuItem.Click += contextMenuItem_Click;
                mnuItem.MouseEnter += contextMenuItem_Select;
            }
            return mnuContextMenu;
        }

        private void contextMenuItem_Click(object sender, EventArgs e)
        {
            object menuTag = ((ToolStripItem) sender).Tag;
            _selectedCandidate = (ArcmapPickCandidate) menuTag;
        }

        private void contextMenuItem_Select(object sender, EventArgs e)
        {
            object menuTag = ((ToolStripItem) sender).Tag;
            ArcmapPickCandidate candidate = (ArcmapPickCandidate) menuTag;
            ArcmapUtils.FlashFeature(candidate.Feature, ArcmapUtils.FocusMapToScreenDisplay(FocusMap));
        }
    }
}
