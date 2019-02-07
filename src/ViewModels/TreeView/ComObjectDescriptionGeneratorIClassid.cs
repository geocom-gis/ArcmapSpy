// Copyright © 2018 geocom.ch
// This program is subject to the terms of the GPL-3.0 License.
// You should have received a copy of the GNU General Public License
// along with this program. If not, see http://www.gnu.org/licenses/.

using System;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace ArcmapSpy.ViewModels.TreeView
{
    /// <summary>
    /// Implementation of the <see cref="IComObjectDescriptionGenerator"/> interface,
    /// which can handle the Esri IClassId interface.
    /// </summary>
    class ComObjectDescriptionGeneratorIClassid : IComObjectDescriptionGenerator
    {
        public string GererateDescription(object obj, Type typ)
        {
            IClassID classId = obj as IClassID;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("*** IClassID ***");
            sb.AppendLine();

            try
            {
                Guid id = classId.GetCLSID();
                sb.AppendFormat("[{0}] {1}()", id, "GetCLSID");
            }
            catch (Exception e)
            {
                sb.AppendFormat("GetCLSID() exception->'{0}'", e.Message);
            }
            sb.AppendLine();

            try
            {
                string prog = classId.GetProgID();
                sb.AppendFormat("[{0}] {1}()", prog, "GetProgID");
            }
            catch (Exception e)
            {
                sb.AppendFormat("GetProgID() exception->'{0}'", e.Message);
            }
            sb.AppendLine();
            return sb.ToString();
        }
    }
}
