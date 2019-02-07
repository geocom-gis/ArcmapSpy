// Copyright © 2018 geocom.ch
// This program is subject to the terms of the GPL-3.0 License.
// You should have received a copy of the GNU General Public License
// along with this program. If not, see http://www.gnu.org/licenses/.

using System;
using System.Collections;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace ArcmapSpy.ViewModels.TreeView
{
    /// <summary>
    /// Implementation of the <see cref="IComObjectDescriptionGenerator"/> interface,
    /// which can handle the Esri IPropertySet interface.
    /// </summary>
    class ComObjectDescriptionGeneratorIPropertySet : IComObjectDescriptionGenerator
    {
        public string GererateDescription(object obj, Type typ)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("*** IPropertySet ***");
            sb.AppendLine();

            object names;
            object values;
            IPropertySet propertySet = obj as IPropertySet;

            propertySet.GetAllProperties(out names, out values);
            IEnumerable enumNames = names as IEnumerable;
            if (enumNames != null)
            {
                foreach (var name in enumNames)
                {
                    object value = propertySet.GetProperty(name.ToString());
                    sb.AppendFormat("• name: '{0}'  value: {1}", name, value);
                    sb.AppendLine();
                }
            }
            sb.AppendLine();
            return sb.ToString();
        }
    }
}
