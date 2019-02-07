// Copyright © 2018 geocom.ch
// This program is subject to the terms of the GPL-3.0 License.
// You should have received a copy of the GNU General Public License
// along with this program. If not, see http://www.gnu.org/licenses/.

using System;
using ArcmapSpy.Utils;
using ESRI.ArcGIS.Geometry;

namespace ArcmapSpy.ViewModels.TreeView
{
    public class ComObjectNodeExpanderIEnumVertex : IComObjectNodeExpander
    {
        public void CreatePropertyChildNodes(TreeviewBranchViewModel parentNode, object comObj, Type typ)
        {
            IEnumVertex comEnum = comObj as IEnumVertex;
            foreach (IPoint enumValue in comEnum.Enumerate())
            {
                if ((enumValue != null) && (enumValue.GetType().IsCOMObject))
                {
                    ComObjectViewModel child = new ComObjectViewModel(parentNode, enumValue, typeof(IPoint), ".Next()");
                    parentNode.Children.Add(child);
                }
            }
        }
    }
}
