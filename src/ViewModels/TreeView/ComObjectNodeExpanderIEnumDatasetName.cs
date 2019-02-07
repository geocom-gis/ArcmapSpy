// Copyright © 2018 geocom.ch
// This program is subject to the terms of the GPL-3.0 License.
// You should have received a copy of the GNU General Public License
// along with this program. If not, see http://www.gnu.org/licenses/.

using System;
using ArcmapSpy.Utils;
using ESRI.ArcGIS.Geodatabase;

namespace ArcmapSpy.ViewModels.TreeView
{
    public class ComObjectNodeExpanderIEnumDatasetName : IComObjectNodeExpander
    {
        public void CreatePropertyChildNodes(TreeviewBranchViewModel parentNode, object comObj, Type typ)
        {
            IEnumDatasetName enumDatasetName = comObj as IEnumDatasetName;
            foreach (IDatasetName value in ArcmapUtils.EnumerateDatasetNames(enumDatasetName, false))
            {
                if ((value != null) && (value.GetType().IsCOMObject))
                {
                    ComObjectViewModel child = new ComObjectViewModel(parentNode, value, typeof(IDatasetName), ".Next()");
                    parentNode.Children.Add(child);
                }
            }
        }
    }
}
