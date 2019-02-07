// Copyright © 2018 geocom.ch
// This program is subject to the terms of the GPL-3.0 License.
// You should have received a copy of the GNU General Public License
// along with this program. If not, see http://www.gnu.org/licenses/.

using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using ArcmapSpy.ViewModels.TreeView;
using ESRI.ArcGIS.Framework;
using GalaSoft.MvvmLight.Command;

namespace ArcmapSpy.ViewModels
{
    /// <summary>
    /// ViewModel of the <see cref="ApplicationSpyCommand"/>.
    /// </summary>
    public class ApplicationSpyViewModel : BaseViewModel
    {
        private string _selectedNodeInfo;
        private string _hierarchyPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationSpyViewModel"/> class.
        /// </summary>
        public ApplicationSpyViewModel()
        {
            CopyHierarchyPathCommand = new RelayCommand(CopyHierarchyPath);
            RootNode = CreateTree();
            RootNode.ChildPropertyChanged += ChildPropertyChangedEventHandler;
        }

        private void ChildPropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {
            if ((e.PropertyName == "IsSelected") && ((TreeviewBaseViewModel)sender).IsSelected)
            {
                if (sender is ComObjectViewModel)
                {
                    ComObjectViewModel comObjectViewModel = (ComObjectViewModel) sender;
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

        /// <summary>
        /// Gets the command which can copy the current hierarchy path.
        /// </summary>
        public ICommand CopyHierarchyPathCommand { get; private set; }

        /// <summary>
        /// Copies the current hierarchy path to the clipboard.
        /// </summary>
        private void CopyHierarchyPath()
        {
            string hierarchyPath = HierarchyPath;
            if (!string.IsNullOrEmpty(hierarchyPath))
                Clipboard.SetText(hierarchyPath);
        }

        private TreeviewRootViewModel CreateTree()
        {
            TreeviewRootViewModel result = new TreeviewRootViewModel();
            if (Ioc.IsRegistered<IApplication>())
            {
                IApplication esriApplication = ArcMap.Application;
                result.Children.Add(new ComObjectViewModel(result, esriApplication, typeof(IApplication), "ArcMap"));
            }
            return result;
        }
    }
}
