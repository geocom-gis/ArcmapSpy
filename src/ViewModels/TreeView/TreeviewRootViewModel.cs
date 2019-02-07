// Copyright © 2018 geocom.ch
// This program is subject to the terms of the GPL-3.0 License.
// You should have received a copy of the GNU General Public License
// along with this program. If not, see http://www.gnu.org/licenses/.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ArcmapSpy.ViewModels.TreeView
{
    public class TreeviewRootViewModel : TreeviewBaseViewModel
    {
        private ObservableCollection<TreeviewBaseViewModel> _children;

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeviewRootViewModel"/> class.
        /// </summary>
        public TreeviewRootViewModel()
            : base(null)
        {
        }

        /// <summary>
        /// Gets a list of tree view child nodes.
        /// </summary>
        public ObservableCollection<TreeviewBaseViewModel> Children
        {
            get
            {
                if (_children == null)
                    _children = new ObservableCollection<TreeviewBaseViewModel>();
                return _children;
            }
        }

        /// <summary>
        /// This event will be raised for certain property changes of a child node.
        /// Tis way listeners have a central point to attach to the event.
        /// </summary>
        public event PropertyChangedEventHandler ChildPropertyChanged;

        /// <summary>
        /// The root node consumes the bubble up and raises the <see cref="ChildPropertyChanged"/>
        /// event.
        /// </summary>
        /// <param name="sender">TreeViewItem doing the change.</param>
        /// <param name="propertyName">Name of the property which has changed.</param>
        public override void BubbleUpOnChildPropertyChanged(TreeviewBaseViewModel sender, string propertyName)
        {
            if (ChildPropertyChanged != null)
                ChildPropertyChanged(sender, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Returns an enumeration of all leaf tree items of the tree.
        /// </summary>
        /// <returns>Enumeration of all leafes.</returns>
        public IEnumerable<TreeviewLeafViewModel> EnumLeafes()
        {
            List<TreeviewLeafViewModel> result = new List<TreeviewLeafViewModel>();
            VisitLeafesRecursively(this, result);
            return result;
        }

        private void VisitLeafesRecursively(TreeviewBaseViewModel node, List<TreeviewLeafViewModel> result)
        {
            if (node is TreeviewRootViewModel)
            {
                var children = (node as TreeviewRootViewModel).Children;
                foreach (TreeviewBaseViewModel child in children)
                    VisitLeafesRecursively(child, result);
            }
            else if (node is TreeviewBranchViewModel)
            {
                var children = (node as TreeviewBranchViewModel).Children;
                foreach (TreeviewBaseViewModel child in children)
                    VisitLeafesRecursively(child, result);
            }
            else if (node is TreeviewLeafViewModel)
            {
                result.Add(node as TreeviewLeafViewModel);
            }
        }
    }
}
