// Copyright © 2018 geocom.ch
// This program is subject to the terms of the GPL-3.0 License.
// You should have received a copy of the GNU General Public License
// along with this program. If not, see http://www.gnu.org/licenses/.

using System;
using System.Collections.Generic;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;

namespace ArcmapSpy.Utils
{
    /// <summary>
    /// Helper functions to work with ArcMap layers.
    /// </summary>
    internal static class ArcmapLayerUtils
    {
        /// <summary>
        /// Enumerates all valid data layers of a given map.
        /// The search is done recursive through the table of contents (TOC).
        /// </summary>
        /// <param name="map">Map to get the layers from.</param>
        /// <param name="onlyVisibleLayers">If true, only visible layers are considered.
        /// To include layers which are invisible, e.g. because they are out of the zoom range,
        /// pass false.</param>
        /// <returns>Enumeration of the layers from the map.</returns>
        public static IEnumerable<IFeatureLayer> EnumerateLayers(IMap map, bool onlyVisibleLayers)
        {
            // Create filter function to accept or reject the layers
            Func<ILayer, bool> acceptLayerFunc;
            if (onlyVisibleLayers)
                acceptLayerFunc = new LayerVisibilityTester(map).IsLayerVisible;
            else
                acceptLayerFunc = layer => true; // no filter, always true

            int layerCount = map.LayerCount;
            for (int index = 0; index < layerCount; index++)
            {
                ILayer layer = map.Layer[index];
                foreach (IFeatureLayer resultLayer in EnumerateLayersRecursive(layer, acceptLayerFunc))
                    yield return resultLayer;
            }
        }

        private static IEnumerable<IFeatureLayer> EnumerateLayersRecursive(ILayer layer, Func<ILayer, bool> acceptLayer)
        {
            if (layer.Valid && acceptLayer(layer))
            {
                if ((layer is IGroupLayer) && (layer is ICompositeLayer))
                {
                    // Recursively call function for childs of this group-layer
                    ICompositeLayer compositeLayer = layer as ICompositeLayer;
                    int layerCount = compositeLayer.Count;
                    for (int index = 0; index < layerCount; index++)
                    {
                        ILayer childLayer = compositeLayer.Layer[index];
                        foreach (IFeatureLayer resultLayer in EnumerateLayersRecursive(childLayer, acceptLayer))
                            yield return resultLayer;
                    }
                }
                else if (layer is IFeatureLayer)
                {
                    yield return layer as IFeatureLayer;
                }
            }
        }

        /// <summary>
        /// Checks if a given layer can actually be seen by the user in the map.
        /// </summary>
        /// <param name="layer">Layer we want to know the visibility from.</param>
        /// <param name="currentMapScale">The current scale of the map.</param>
        /// <returns>Returns true if the layer is visible, otherwise false.</returns>
        public static bool LayerIsVisible(ILayer layer, double currentMapScale)
        {
            bool result = layer.Valid && layer.Visible;

            // test if zoom is inside layer zoom range
            if (result)
            {
                double minScale = layer.MinimumScale;
                double maxScale = layer.MaximumScale;
                result = ((minScale == 0) || (currentMapScale <= minScale)) &&
                         ((maxScale == 0) || (currentMapScale >= maxScale));
            }
            return result;
        }

        /// <summary>
        /// Gets the number of rows of the table, belonging to a given layer.
        /// </summary>
        /// <param name="layer">Esri layer of the TOC</param>
        /// <returns>Number of rows or Null if the layer is invalid.</returns>
        public static int? GetLayerRowCount(IFeatureLayer layer)
        {
            IDisplayTable displayTable = (IDisplayTable)layer;
            if ((layer.Valid) && (displayTable != null))
            {
                IQueryFilter queryFilter = new QueryFilterClass();
                queryFilter.AddField(layer.FeatureClass.OIDFieldName);
                return displayTable.DisplayTable.RowCount(queryFilter);
            }
            return null;
        }

        /// <summary>
        /// Gets the number of rows, belonging to a given layer, filtered by the layer.
        /// </summary>
        /// <param name="layer">Esri layer of the TOC</param>
        /// <returns>Number of rows or Null if the layer is invalid or not filtered.</returns>
        public static int? GetFilteredLayerRowCount(IFeatureLayer layer)
        {
            IDisplayTable displayTable = (IDisplayTable)layer;
            if ((layer.Valid) && (displayTable != null))
            {
                IFeatureLayerDefinition layerDefinition = layer as IFeatureLayerDefinition;
                string filter = layerDefinition?.DefinitionExpression;
                if (!string.IsNullOrEmpty(filter))
                {
                    IQueryFilter queryFilter = new QueryFilterClass();
                    queryFilter.WhereClause = filter;
                    queryFilter.AddField(layer.FeatureClass.OIDFieldName);
                    return displayTable.DisplayTable.RowCount(queryFilter);
                }
            }
            return null;
        }

        /// <summary>
        /// Selects a single layer in the ArcMap TOC. To be sure that the layer is indeed visible,
        /// one should consider to call <see cref="ExpandParentLayers"/> as well.
        /// </summary>
        /// <param name="selectThisLayer">Make this layer the selected one.</param>
        /// <param name="arcMapApplication">The Esri ArcMap application object.</param>
        public static void SelectLayer(ILayer selectThisLayer, IApplication arcMapApplication)
        {
            IMxDocument mxDocument = arcMapApplication.Document as IMxDocument;
            mxDocument.CurrentContentsView.SelectedItem = selectThisLayer;
        }

        /// <summary>
        /// Searches for all parent group layers in the ArcMap TOC, to a given layer.
        /// </summary>
        /// <param name="targetLayer">The layer we need its parents for.</param>
        /// <param name="map">The ArcMap focus map.</param>
        /// <returns>A list of found parent layers. If no parents exists or the layer could not
        /// be found, an empty list is returned.</returns>
        public static List<ILayer> FindParentLayers(ILayer targetLayer, IMap map)
        {
            List<ILayer> parentList = new List<ILayer>();

            int layerCount = map.LayerCount;
            for (int index = 0; index < layerCount; index++)
            {
                ILayer childLayer = map.Layer[index];
                if (FindParentLayersRecursive(targetLayer, childLayer, parentList))
                    return parentList;
            }
            return parentList;
        }

        private static bool FindParentLayersRecursive(ILayer targetLayer, ILayer layer, List<ILayer> parentList)
        {
            if (targetLayer == layer)
            {
                // If layer is found, the recursive search stops
                return true;
            }
            else if ((layer is IGroupLayer) && (layer is ICompositeLayer))
            {
                // Recursively call function for childs of this group-layer
                ICompositeLayer compositeLayer = layer as ICompositeLayer;

                int layerCount = compositeLayer.Count;
                for (int index = 0; index < layerCount; index++)
                {
                    ILayer childLayer = compositeLayer.Layer[index];
                    bool result = FindParentLayersRecursive(targetLayer, childLayer, parentList);

                    // If we are on the correct path, we add it to the result
                    if (result)
                    {
                        parentList.Insert(0, layer);
                        return true;
                    }
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Expands all group layers, which are parents of the given layer <paramref name="expandToThisLayer"/>.
        /// </summary>
        /// <param name="expandToThisLayer">This layer should become visible by expanding the TOC.</param>
        /// <param name="map">Esri map object containing the TOC.</param>
        /// <returns>Returns true if the layer was found, otherwise false.</returns>
        public static void ExpandParentLayers(ILayer expandToThisLayer, IMap map)
        {
            List<ILayer> parentLayers = FindParentLayers(expandToThisLayer, map);
            foreach (ILayer parentLayer in parentLayers)
            {
                IGroupLayer groupLayer = parentLayer as IGroupLayer;
                groupLayer.Expanded = true;
            }
        }

        /// <summary>
        /// Helper class to determine whether a layer is currently visible in a given map.
        /// </summary>
        private class LayerVisibilityTester
        {
            private readonly double _mapScale;

            /// <summary>
            /// Initializes a new instance of the <see cref="LayerVisibilityTester"/> class.
            /// </summary>
            /// <param name="map">The Esri map containing the layers to test.</param>
            public LayerVisibilityTester(IMap map)
            {
                _mapScale = map.MapScale;
            }

            /// <summary>
            /// Checks whether the layer is currently visible in the map.
            /// </summary>
            /// <param name="layer">Layer to check.</param>
            /// <returns>Returns true if the layer is visible, otherwise false.</returns>
            public bool IsLayerVisible(ILayer layer)
            {
                return LayerIsVisible(layer, _mapScale);
            }
        }
    }
}
