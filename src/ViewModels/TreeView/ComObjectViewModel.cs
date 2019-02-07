// Copyright © 2018 geocom.ch
// This program is subject to the terms of the GPL-3.0 License.
// You should have received a copy of the GNU General Public License
// along with this program. If not, see http://www.gnu.org/licenses/.

using System;
using System.Collections.Generic;

namespace ArcmapSpy.ViewModels.TreeView
{
    public class ComObjectViewModel : TreeViewLazyLoadBranchViewModel
    {
        private readonly object _comObj;
        private readonly Type _type;
        private string _description;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComObjectViewModel"/> class.
        /// </summary>
        /// <param name="parent">The parent viewmodel.</param>
        /// <param name="comObj">Com object which is represented in this node.</param>
        /// <param name="title">Title of the node, or null if the title should be generated automatically.</param>
        public ComObjectViewModel(TreeviewBaseViewModel parent, object comObj, Type typ, string title)
            : base(parent)
        {
            _comObj = comObj;
            _type = typ;
            Title = title;
            _description = null;
        }

        protected override void LazyLoadChildren()
        {
            AddPropertyChildNodes();
            Children.Sort((a, b) => string.Compare(a.Title, b.Title, StringComparison.InvariantCultureIgnoreCase));
            AddQueryInterfaceChildNode(_comObj);
        }

        public string Description
        {
            get
            {
                if (_description == null)
                {
                    IComObjectDescriptionGenerator generator;
                    string typeName = _type.ToString();
                    if (Ioc.IsRegisteredWithKey<IComObjectDescriptionGenerator>(typeName))
                        generator = Ioc.GetOrCreateWithKey<IComObjectDescriptionGenerator>(typeName);
                    else
                        generator = new ComObjectDescriptionGeneratorDefault();
                    _description = generator.GererateDescription(_comObj, _type);
                }
                return _description;
            }
        }

        /// <summary>
        /// Adds a child node for each property of the current COM object, which is another COM object.
        /// </summary>
        private void AddPropertyChildNodes()
        {
            IComObjectNodeExpander nodeExpander;
            string typeName = _type.ToString();
            if (Ioc.IsRegisteredWithKey<IComObjectNodeExpander>(typeName))
                nodeExpander = Ioc.GetOrCreateWithKey<IComObjectNodeExpander>(typeName);
            else
                nodeExpander = new ComObjectNodeExpanderDefault();
            nodeExpander.CreatePropertyChildNodes(this, _comObj, _type);
        }

        /// <summary>
        /// Adds a group node, which can cast the current node to other interfaces.
        /// </summary>
        /// <param name="comObj">COM object of the current node.</param>
        private void AddQueryInterfaceChildNode(object comObj)
        {
            ComQueryInterfaceViewModel child = new ComQueryInterfaceViewModel(this, comObj);
            Children.Insert(0, child);
        }

        public string GetHierachyPath()
        {
            List<string> parts = GetParentTitles();
            parts.Add(Title);

            string prefix = string.Empty;
            for (int index = 0; index < parts.Count; index++)
            {
                if (parts[index].StartsWith("as "))
                {
                    prefix += "(";
                    parts[index] = " " + parts[index] + ")";
                }
            }
            return prefix + string.Join("", parts);
        }

        private List<string> GetParentTitles()
        {
            List<string> titles = new List<string>();

            List<TreeviewBaseViewModel> parents = BuildParentList();
            foreach (TreeviewBaseViewModel viewModel in parents)
            {
                if (viewModel is ComObjectViewModel)
                    titles.Add((viewModel as ComObjectViewModel).Title);
            }

            return titles;
        }

        private List<TreeviewBaseViewModel> BuildParentList()
        {
            List<TreeviewBaseViewModel> result = new List<TreeviewBaseViewModel>();

            TreeviewBaseViewModel parent = Parent;
            while (parent != null)
            {
                result.Insert(0, parent);
                parent = parent.Parent;
            }
            return result;
        }
    }
}
