// Copyright © 2018 geocom.ch
// This program is subject to the terms of the GPL-3.0 License.
// You should have received a copy of the GNU General Public License
// along with this program. If not, see http://www.gnu.org/licenses/.

using System;
using ArcmapSpy.Utils;
using ESRI.ArcGIS.esriSystem;

namespace ArcmapSpy.ViewModels.TreeView
{
    public class ComObjectNodeExpanderIEnumName : IComObjectNodeExpander
    {
        public void CreatePropertyChildNodes(TreeviewBranchViewModel parentNode, object comObj, Type typ)
        {
            IEnumName enumName = comObj as IEnumName;
            enumName.Reset();
            foreach (IName value in enumName.Enumerate())
            {
                if ((value != null) && (value.GetType().IsCOMObject))
                {
                    ComObjectViewModel child = new ComObjectViewModel(parentNode, value, typeof(IName), ".Next()");
                    parentNode.Children.Add(child);
                }
            }
        }
    }
}
