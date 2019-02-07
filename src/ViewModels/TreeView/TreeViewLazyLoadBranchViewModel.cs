// Copyright © 2018 geocom.ch
// This program is subject to the terms of the GPL-3.0 License.
// You should have received a copy of the GNU General Public License
// along with this program. If not, see http://www.gnu.org/licenses/.

using System.ComponentModel;

namespace ArcmapSpy.ViewModels.TreeView
{
    /// <summary>
    /// Base class for viewmodels representing a branch node with childrens in a treeview.
    /// The children are lazy loaded in overriding the <see cref="LazyLoadChildren"/> method.
    /// </summary>
    public class TreeViewLazyLoadBranchViewModel : TreeviewBranchViewModel
    {
        private bool _hasDummyChildNode;

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeViewLazyLoadBranchViewModel"/> class.
        /// </summary>
        /// <param name="parent">Parent item in the tree view.</param>
        public TreeViewLazyLoadBranchViewModel(TreeviewBaseViewModel parent)
            : base(parent)
        {
            _hasDummyChildNode = true;
            Children.Add(new TreeviewLeafViewModel(this));
            PropertyChanged += IsExpandedChangedEventHandler;
        }

        private void IsExpandedChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {
            if (_hasDummyChildNode && (e.PropertyName == "IsExpanded"))
            {
                _hasDummyChildNode = false;
                Children.Clear();
                LazyLoadChildren();
            }
        }

        /// <summary>
        /// Invoked when the child items need to be loaded on demand.
        /// Subclasses can override this to populate the Children collection.
        /// </summary>
        protected virtual void LazyLoadChildren()
        {
        }
    }
}
