// Copyright © 2018 geocom.ch
// This program is subject to the terms of the GPL-3.0 License.
// You should have received a copy of the GNU General Public License
// along with this program. If not, see http://www.gnu.org/licenses/.

using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using ArcmapSpy.Utils;
using ESRI.ArcGIS.Carto;
using GalaSoft.MvvmLight.Command;

namespace ArcmapSpy.ViewModels
{
    /// <summary>
    /// Holds information about a single layer in the ArcMap TOC
    /// </summary>
    public class LayerInfoViewModel : BaseViewModel
    {
        private List<ILayer> _parentLayers;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayerInfoViewModel"/> class.
        /// </summary>
        public LayerInfoViewModel()
        {
            JumpToLayerCommand = new RelayCommand(JumpToLayer);
            RemoveScaleRangeCommand = new RelayCommand(RemoveScaleRange);
        }

        /// <summary>
        /// Gets the layer the info object is pointing to.
        /// </summary>
        public ILayer Layer { get; set; }

        /// <summary>
        /// Gets or sets the title of the layer displayed in the TOC
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the name of the table, the layers is based on.
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Gets a value indicating, whether the layer and its parent layers are turned on.
        /// It is still possible that they are out of scope and are not displayed.
        /// </summary>
        public bool Visible
        {
            get
            {
                bool result = Layer.Visible;
                foreach (ILayer parentLayer in ParentLayers)
                    result &= parentLayer.Visible;
                return result;
            }
        }

        public string ScaleRange
        {
            get
            {
                string minScale = null;
                if (Layer.MinimumScale != 0.0)
                    minScale = string.Format("1:{0}", Layer.MinimumScale);

                string maxScale = null;
                if (Layer.MaximumScale != 0.0)
                    maxScale = string.Format("1:{0}", Layer.MaximumScale);

                string scale = null;
                if ((minScale != null) || (maxScale != null))
                    scale = string.Format(" ({0}...{1})", minScale, maxScale);

                return scale;
            }
        }

        /// <summary>
        /// Gets the path of all parent group layers as string.
        /// </summary>
        public string Group
        {
            get { return string.Join("/", ParentLayers.Select(parentLayer => parentLayer.Name)); }
        }

        /// <summary>
        /// Gets the current query filter if available.
        /// </summary>
        public string Filter { get; set; }

        /// <summary>
        /// Gets the number of rows of the current layer.
        /// </summary>
        public string RowCount { get; set; }

        /// <summary>
        /// Gets the command which can show the matching layer in ArcMap.
        /// </summary>
        public ICommand JumpToLayerCommand { get; private set; }

        /// <summary>
        /// Expands the table of contents in ArcMap to the matching layer and selects it.
        /// </summary>
        private void JumpToLayer()
        {
            ArcmapLayerUtils.ExpandParentLayers(Layer, ArcmapUtils.GetFocusMap());
            ArcmapLayerUtils.SelectLayer(Layer, ArcMap.Application);
        }

        /// <summary>
        /// Gets the command which can remove the scale range of the layer.
        /// </summary>
        public ICommand RemoveScaleRangeCommand { get; private set; }

        /// <summary>
        /// Removes the scale range of the layer.
        /// </summary>
        private void RemoveScaleRange()
        {
            Layer.MinimumScale = 0;
            Layer.MaximumScale = 0;
            RaisePropertyChanged(nameof(ScaleRange));
            RaisePropertyChanged(nameof(RemoveScaleRangeCommandVisible));
        }

        public bool RemoveScaleRangeCommandVisible
        {
            get { return !string.IsNullOrEmpty(ScaleRange); }
        }

        /// <summary>
        /// Gets a list of parent group layers.
        /// </summary>
        private List<ILayer> ParentLayers
        {
            get
            {
                if (_parentLayers == null)
                    _parentLayers = ArcmapLayerUtils.FindParentLayers(Layer, ArcmapUtils.GetFocusMap());
                return _parentLayers;
            }
        }
    }
}
