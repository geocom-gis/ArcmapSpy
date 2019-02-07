// Copyright © 2018 geocom.ch
// This program is subject to the terms of the GPL-3.0 License.
// You should have received a copy of the GNU General Public License
// along with this program. If not, see http://www.gnu.org/licenses/.

using System.Collections.Generic;

namespace ArcmapSpy.ViewModels.TreeView
{
    /// <summary>
    /// Base class for viewmodels representing a branch node with childrens in a treeview.
    /// </summary>
    public class TreeviewBranchViewModel : TreeviewBaseViewModel
    {
        private List<TreeviewBaseViewModel> _children;

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeviewBranchViewModel"/> class.
        /// </summary>
        /// <param name="parent">Parent item in the tree view.</param>
        public TreeviewBranchViewModel(TreeviewBaseViewModel parent)
            : base(parent)
        {
        }

        /// <summary>
        /// Gets a list of tree view child nodes.
        /// </summary>
        public List<TreeviewBaseViewModel> Children
        {
            get
            {
                if (_children == null)
                    _children = new List<TreeviewBaseViewModel>();
                return _children;
            }
        }
    }
}
