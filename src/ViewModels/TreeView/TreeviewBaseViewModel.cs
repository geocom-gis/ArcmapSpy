// Copyright © 2018 geocom.ch
// This program is subject to the terms of the GPL-3.0 License.
// You should have received a copy of the GNU General Public License
// along with this program. If not, see http://www.gnu.org/licenses/.

namespace ArcmapSpy.ViewModels.TreeView
{
    /// <summary>
    /// Base class for viewmodels representing a node in a treeview.
    /// </summary>
    public abstract class TreeviewBaseViewModel : BaseViewModel
    {
        private bool _isSelected;
        private bool _isExpanded;
        private bool _isChecked;

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeviewBaseViewModel"/> class.
        /// </summary>
        /// <param name="parent">Parent item in the tree view, or null for root items.</param>
        protected TreeviewBaseViewModel(TreeviewBaseViewModel parent)
        {
            Parent = parent;
        }

        /// <summary>
        /// Gets or sets the title of the tree node.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the TreeViewItem 
        /// associated with this object is selected.
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }

            set
            {
                ChangeProperty(ref _isSelected, value, nameof(IsSelected));
                BubbleUpOnChildPropertyChanged(this, nameof(IsSelected));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the TreeViewItem 
        /// associated with this object is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get { return _isExpanded; }

            set
            {
                ChangeProperty(ref _isExpanded, value, nameof(IsExpanded));

                // Expand all the way up to the root.
                if (_isExpanded && Parent != null)
                    Parent.IsExpanded = true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the TreeViewItem 
        /// associated with this object is checked.
        /// </summary>
        public bool IsChecked
        {
            get { return _isChecked; }

            set
            {
                ChangeProperty(ref _isChecked, value, nameof(IsChecked));
                BubbleUpOnChildPropertyChanged(this, nameof(IsChecked));
            }
        }

        /// <summary>
        /// Bubbles up a property changed event, so the root node can raise an event.
        /// </summary>
        /// <param name="sender">TreeViewItem doing the change.</param>
        /// <param name="propertyName">Name of the property which has changed.</param>
        public virtual void BubbleUpOnChildPropertyChanged(TreeviewBaseViewModel sender, string propertyName)
        {
            if (Parent != null)
            {
                Parent.BubbleUpOnChildPropertyChanged(sender, propertyName);
            }
        }

        /// <summary>
        /// Gets the viewmodel of the parent node.
        /// </summary>
        public TreeviewBaseViewModel Parent { get; private set; }
    }
}
