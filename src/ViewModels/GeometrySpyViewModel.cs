// Copyright © 2018 geocom.ch
// This program is subject to the terms of the GPL-3.0 License.
// You should have received a copy of the GNU General Public License
// along with this program. If not, see http://www.gnu.org/licenses/.

using System.ComponentModel;
using ArcmapSpy.ViewModels.TreeView;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace ArcmapSpy.ViewModels
{
    public class GeometrySpyViewModel : BaseViewModel
    {
        private readonly IGeometry _geometry;
        private string _selectedNodeInfo;
        private string _hierarchyPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeometrySpyViewModel"/>.
        /// </summary>
        public GeometrySpyViewModel()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeometrySpyViewModel"/>.
        /// <param name="feature">The feature to represent.</param>
        /// </summary>
        public GeometrySpyViewModel(IFeature feature)
        {
            _geometry = feature.Shape;
            RootNode = CreateTree();
            RootNode.ChildPropertyChanged += ChildPropertyChangedEventHandler;
        }

        private void ChildPropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {
            if ((e.PropertyName == "IsSelected") && ((TreeviewBaseViewModel)sender).IsSelected)
            {
                if (sender is ComObjectViewModel)
                {
                    ComObjectViewModel comObjectViewModel = (ComObjectViewModel)sender;
                    SelectedNodeInfo = comObjectViewModel.Description;
                    HierarchyPath = comObjectViewModel.GetHierachyPath();
                }
                else
                {
                    SelectedNodeInfo = string.Empty;
                    HierarchyPath = string.Empty;
                }
            }
        }

        /// <summary>
        /// Gets the viewmodel of the root node for the treeview.
        /// </summary>
        public TreeviewRootViewModel RootNode { get; private set; }

        /// <summary>
        /// Gets or sets the text, which is displayed as information about the selected node.
        /// </summary>
        public string SelectedNodeInfo
        {
            get { return _selectedNodeInfo; }
            set { ChangeProperty(ref _selectedNodeInfo, value); }
        }

        /// <summary>
        /// Gets or sets the path, which represents the hierarchy to get to the selected COM object.
        /// </summary>
        public string HierarchyPath
        {
            get { return _hierarchyPath; }
            set { ChangeProperty(ref _hierarchyPath, value); }
        }

        private TreeviewRootViewModel CreateTree()
        {
            TreeviewRootViewModel result = new TreeviewRootViewModel();
            if (_geometry != null)
            {
                result.Children.Add(new ComObjectViewModel(result, _geometry, typeof(IGeometry), "IGeometry"));
            }
            return result;
        }
    }
}
