// Copyright © 2018 geocom.ch
// This program is subject to the terms of the GPL-3.0 License.
// You should have received a copy of the GNU General Public License
// along with this program. If not, see http://www.gnu.org/licenses/.

namespace ArcmapSpy.ViewModels.TreeView
{
    public class TreeviewLeafViewModel : TreeviewBaseViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TreeviewLeafViewModel"/> class.
        /// </summary>
        /// <param name="parent">Parent item in the tree view.</param>
        public TreeviewLeafViewModel(TreeviewBaseViewModel parent)
            : base(parent)
        {
        }

        /// <summary>
        /// Gets or sets a text describing the node.
        /// </summary>
        public string Text { get; set; }
    }
}
