// Copyright © 2018 geocom.ch
// This program is subject to the terms of the GPL-3.0 License.
// You should have received a copy of the GNU General Public License
// along with this program. If not, see http://www.gnu.org/licenses/.

using System;
using System.Collections.Generic;
using System.Linq;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.CartoUI;
using ESRI.ArcGIS.CatalogUI;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace ArcmapSpy.Utils
{
    /// <summary>
    /// Helper functions to work with a running ArcMap instance.
    /// </summary>
    internal static class ArcmapUtils
    {
        /// <summary>
        /// Gets the focus map from an ArcMap application object.
        /// </summary>
        /// <returns>Focusmap of the application.</returns>
        public static IMap GetFocusMap()
        {
            IApplication application = GetArcmapApplication();
            return (application.Document as IMxDocument)?.FocusMap;
        }

        /// <summary>
        /// Gets the IScreenDisplay interface from the ArcMap focusmap.
        /// </summary>
        /// <param name="focusMap">Esri focus map.</param>
        /// <returns>The screen display.</returns>
        public static IScreenDisplay FocusMapToScreenDisplay(IMap focusMap)
        {
            IScreenDisplay result = null;
            if (focusMap != null)
                result = (focusMap as IActiveView)?.ScreenDisplay;
            return result;
        }

        /// <summary>
        /// Flashes a single feature.
        /// </summary>
        /// <param name="feature">Feature to flash in the map.</param>
        /// <param name="display">Use this map to flash.</param>
        public static void FlashFeature(IFeature feature, IScreenDisplay display)
        {
            IFeatureIdentifyObj featureIdentifyObj = new FeatureIdentifyObjectClass();
            featureIdentifyObj.Feature = feature;
            (featureIdentifyObj as IIdentifyObj).Flash(display);
        }

        /// <summary>
        /// Refreshes the whole map, including all temporary graphics. This is a relatively slow
        /// operation, consider to use <see cref="InvalidateMap"/> for repeated calls.
        /// </summary>
        /// <param name="focusMap">Map to refresh.</param>
        public static void RefreshMap(IMap focusMap)
        {
            (focusMap as IActiveView)?.Refresh();
        }

        /// <summary>
        /// Instructs ArcMap to refresh the whole map, as soon as there is time for drawing.
        /// </summary>
        /// <param name="focusMap">Map to refresh.</param>
        public static void InvalidateMap(IMap focusMap)
        {
            (focusMap as IActiveView)?.ScreenDisplay.Invalidate(null, true, (short)esriScreenCache.esriAllScreenCaches);
        }

        /// <summary>
        /// Converts a screen pixel distance to the real distance.
        /// </summary>
        /// <param name="pixels">Number of pixels on the screen.</param>
        /// <param name="display">Esri display.</param>
        /// <returns>Real world distance.</returns>
        public static double PixelsToDistance(int pixels, IScreenDisplay display)
        {
            IDisplayTransformation transformation = display.DisplayTransformation;

            // get number of pixels in x axis
            tagRECT deviceRect = transformation.get_DeviceFrame();
            int pixelsOfMapWidth = deviceRect.right - deviceRect.left;

            // get extent of visible area in map
            IEnvelope mapExtent = transformation.VisibleBounds;
            double distOfMapWidth = mapExtent.Width;

            // calculate size of one pixel
            double sizeOfPixel = distOfMapWidth / pixelsOfMapWidth;
            return sizeOfPixel * pixels;
        }

        /// <summary>
        /// Enumerates all workspaces in ArcMap.
        /// </summary>
        /// <param name="esriApplication">The Esri application istance.</param>
        /// <returns>Enumeration of workspaces.</returns>
        public static IEnumerable<IWorkspace> EnumerateAllWorkspaces(IApplication esriApplication)
        {
            HashSet<IWorkspace> duplicateFinder = new HashSet<IWorkspace>();
            foreach (IDataset dataset in ArcmapUtils.EnumerateAllDatasets(esriApplication))
            {
                IWorkspace workspace = dataset.Workspace;
                if ((workspace != null) && !duplicateFinder.Contains(workspace))
                {
                    duplicateFinder.Add(workspace);
                    yield return workspace;
                }
            }
        }

        /// <summary>
        /// Enumerates all datasets in ArcMap.
        /// </summary>
        /// <param name="esriApplication">The Esri application istance.</param>
        /// <returns>Enumeration of datasets.</returns>
        public static IEnumerable<IDataset> EnumerateAllDatasets(IApplication esriApplication)
        {
            esriDatasetType[] acceptedDatasetTypes = new esriDatasetType[] { esriDatasetType.esriDTGeo, esriDatasetType.esriDTFeatureDataset, esriDatasetType.esriDTFeatureClass, esriDatasetType.esriDTText, esriDatasetType.esriDTTable };

            IDocumentDatasets documentDatasets = esriApplication.Document as IDocumentDatasets;
            IEnumDataset datasetEnumeration = documentDatasets.Datasets;
            datasetEnumeration.Reset();
            IDataset dataset = datasetEnumeration.Next();
            while (dataset != null)
            {
                if (acceptedDatasetTypes.Contains(dataset.Type))
                    yield return dataset;
                dataset = datasetEnumeration.Next();
            }
        }

        public static IEnumerable<IDatasetName> EnumerateDatasetNames(IEnumDatasetName enumDatasetNames, bool recursive)
        {
            if (enumDatasetNames != null)
            {
                enumDatasetNames.Reset();
                IDatasetName datasetName = enumDatasetNames.Next();
                while (datasetName != null)
                {
                    yield return datasetName;
                    if (recursive && (datasetName.Type == esriDatasetType.esriDTFeatureDataset))
                    {
                        foreach (var subDatasetName in EnumerateDatasetNames(datasetName.SubsetNames, recursive))
                            yield return subDatasetName;
                    }
                    datasetName = enumDatasetNames.Next();
                }
            }
        }

        /// <summary>
        /// Determines the base geometry type of an esri geometry object.
        /// </summary>
        /// <param name="esriGeometry">Esri geometry object.</param>
        /// <returns>Base geometry type of the geometry object.</returns>
        public static GeometryBaseType DetectGeometryBaseType(IGeometry esriGeometry)
        {
            return DetectGeometryBaseType(esriGeometry.GeometryType);
        }

        /// <summary>
        /// Determines the base geometry type of an esri geometry type.
        /// </summary>
        /// <param name="esriGeometryType">Esri geometry type</param>
        /// <returns>Base geometry type of the geometry object.</returns>
        public static GeometryBaseType DetectGeometryBaseType(esriGeometryType esriGeometryType)
        {
            switch (esriGeometryType)
            {
                case esriGeometryType.esriGeometryPoint:
                case esriGeometryType.esriGeometryMultipoint:
                    return GeometryBaseType.Point;
                case esriGeometryType.esriGeometryLine:
                case esriGeometryType.esriGeometryCircularArc:
                case esriGeometryType.esriGeometryEllipticArc:
                case esriGeometryType.esriGeometryBezier3Curve:
                case esriGeometryType.esriGeometryPath:
                case esriGeometryType.esriGeometryPolyline:
                    return GeometryBaseType.Line;
                case esriGeometryType.esriGeometryRing:
                case esriGeometryType.esriGeometryPolygon:
                    return GeometryBaseType.Area;
                default:
                    return GeometryBaseType.Unknown;
            }
        }

        private static IApplication GetArcmapApplication()
        {
            Type appRefType = Type.GetTypeFromCLSID(typeof(AppRefClass).GUID);
            return Activator.CreateInstance(appRefType) as IApplication;
        }

        /// <summary>
        /// Gets the reference scale of the focus map.
        /// </summary>
        /// <param name="defaultScale">This value is returned, if the special value 0.0 is set as
        /// reference scale in ArcMap, meaning no scaling of styles.</param>
        /// <returns>Reference scale.</returns>
        public static double GetReferenceScale(double defaultScale = 0.0)
        {
            double result = GetFocusMap().ReferenceScale;
            if (result == 0.0)
                result = defaultScale;
            return result;
        }
    }
}
