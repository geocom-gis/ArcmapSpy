// Copyright © 2018 geocom.ch
// This program is subject to the terms of the GPL-3.0 License.
// You should have received a copy of the GNU General Public License
// along with this program. If not, see http://www.gnu.org/licenses/.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ArcmapSpy.Utils;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;

namespace ArcmapSpy.ViewModels
{
    public class LayerSpyViewModel : BaseViewModel
    {
        private readonly List<LayerInfoViewModel> _allLayers;
        private string _filter;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayerSpyViewModel"/> class.
        /// </summary>
        public LayerSpyViewModel()
        {
            if (Ioc.IsRegistered<IApplication>())
                _allLayers = LoadLayerInfosFromArcMap();
            else
                _allLayers = new List<LayerInfoViewModel>();
            FilteredLayers = new ObservableCollection<LayerInfoViewModel>(_allLayers);
        }

        /// <summary>
        /// Gets a collection of infos about the ArcMap table of contents.
        /// </summary>
        public ObservableCollection<LayerInfoViewModel> FilteredLayers { get; }

        /// <summary>
        /// Gets or sets the search filter.
        /// </summary>
        public string Filter
        {
            get { return _filter; }

            set 
            {
                if (ChangeProperty(ref _filter, value))
                    ApplyFilter(_filter);
            }
        }

        private List<LayerInfoViewModel> LoadLayerInfosFromArcMap()
        {
            List<LayerInfoViewModel> result = new List<LayerInfoViewModel>();

            var layers = ArcmapLayerUtils.EnumerateLayers(ArcmapUtils.GetFocusMap(), false);
            foreach (IFeatureLayer layer in layers)
            {
                LayerInfoViewModel layerInfo = LoadLayerInfoFromArcMap(layer);
                result.Add(layerInfo);
            }
            return result;
        }

        private static LayerInfoViewModel LoadLayerInfoFromArcMap(IFeatureLayer layer)
        {
            // tablename
            IDataset dataset = (IDataset)layer.FeatureClass;
            string tableName = ArcobjWorkspaceUtils.GetTableName(dataset);

            IFeatureLayerDefinition layerDefinition = layer as IFeatureLayerDefinition;
            string filter = layerDefinition?.DefinitionExpression;
            int?[] rowCounts =
            {
                ArcmapLayerUtils.GetFilteredLayerRowCount(layer),
                ArcmapLayerUtils.GetLayerRowCount(layer)
            };

            return new LayerInfoViewModel
            {
                Title = layer.Name,
                Layer = layer,
                TableName = tableName,
                Filter = filter,
                RowCount = string.Join(" / ", rowCounts.Where(item => item != null)),
            };
        }

        private void ApplyFilter(string filter)
        {
            FilteredLayers.Clear();
            foreach (LayerInfoViewModel item in _allLayers)
            {
                if (LayerInfoMatchesPattern(item, filter))
                    FilteredLayers.Add(item);
            }
        }

        /// <summary>
        /// Checks whether a note matches with a given search pattern.
        /// </summary>
        /// <param name="item">Note model to test.</param>
        /// <param name="pattern">Filter pattern to search for.</param>
        /// <returns>Returns true if the note matches the pattern, otherwise false.</returns>
        private bool LayerInfoMatchesPattern(LayerInfoViewModel item, string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
                return true;

            // Search in title
            string title = item.Title;
            if (!string.IsNullOrEmpty(title) && (title.IndexOf(pattern, StringComparison.InvariantCultureIgnoreCase) >= 0))
                return true;

            // Search in category
            string category = item.Group;
            if (!string.IsNullOrEmpty(category) && (category.IndexOf(pattern, StringComparison.InvariantCultureIgnoreCase) >= 0))
                return true;

            // Search in table name
            string content = item.TableName;
            if (!string.IsNullOrEmpty(content) && (content.IndexOf(pattern, StringComparison.InvariantCultureIgnoreCase) >= 0))
                return true;

            // Search in query
            string query = item.Filter;
            if (!string.IsNullOrEmpty(query) && (query.IndexOf(pattern, StringComparison.InvariantCultureIgnoreCase) >= 0))
                return true;

            // Search in visibility
            string visibility = item.Visible.ToString();
            if ((pattern.Length > 2) && (visibility.IndexOf(pattern, StringComparison.InvariantCultureIgnoreCase) >= 0))
                return true;

            return false;
        }
    }
}
