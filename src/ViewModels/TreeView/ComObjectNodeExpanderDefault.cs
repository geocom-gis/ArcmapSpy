// Copyright © 2018 geocom.ch
// This program is subject to the terms of the GPL-3.0 License.
// You should have received a copy of the GNU General Public License
// along with this program. If not, see http://www.gnu.org/licenses/.

using System;
using System.Reflection;

namespace ArcmapSpy.ViewModels.TreeView
{
    public class ComObjectNodeExpanderDefault : IComObjectNodeExpander
    {
        public void CreatePropertyChildNodes(TreeviewBranchViewModel parentNode, object comObj, Type typ)
        {
            PropertyInfo[] propertyInfos = typ.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo propInfo in propertyInfos)
            {
                try
                {
                    if (!propInfo.CanRead || !propInfo.PropertyType.IsInterface)
                        continue;

                    if (!IsIndexedProperty(propInfo))
                        AddPropertyChildNode(parentNode, comObj, propInfo);
                    else
                        AddIndexedPropertyChildNodes(parentNode, comObj, propInfo);
                }
                catch (Exception ex)
                {
                    // do not add to list, continue
                    string error = ex.Message;
                }
            }
        }

        private static void AddPropertyChildNode(TreeviewBranchViewModel parentNode, object comObj, PropertyInfo propInfo)
        {
            object value = propInfo.GetValue(comObj, null);
            if ((value != null) && value.GetType().IsCOMObject)
            {
                ComObjectViewModel child = new ComObjectViewModel(parentNode, value, propInfo.PropertyType, "." + propInfo.Name);
                parentNode.Children.Add(child);
            }
        }

        private static void AddIndexedPropertyChildNodes(TreeviewBranchViewModel parentNode, object comObj,
            PropertyInfo propInfo)
        {
            ParameterInfo[] parameters = propInfo.GetIndexParameters();
            if ((parameters.Length == 1) && (parameters[0].ParameterType == typeof (int)))
            {
                bool stop = false;
                for (int index = 0; !stop; index++)
                {
                    try
                    {
                        object value = propInfo.GetValue(comObj, new object[] {index});
                        stop = value == null;
                        if ((value != null) && value.GetType().IsCOMObject)
                        {
                            ComObjectViewModel child = new ComObjectViewModel(
                                parentNode, value, propInfo.PropertyType, string.Format(".{0}[{1:000}]", propInfo.Name, index));
                            parentNode.Children.Add(child);
                        }
                    }
                    catch (Exception)
                    {
                        stop = true;
                    }
                }
            }
        }

        private static bool IsIndexedProperty(PropertyInfo propInfo)
        {
            ParameterInfo[] indexers = propInfo.GetIndexParameters();
            return (indexers.Length > 0);
        }
    }
}
