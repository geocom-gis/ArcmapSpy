// Copyright © 2018 geocom.ch
// This program is subject to the terms of the GPL-3.0 License.
// You should have received a copy of the GNU General Public License
// along with this program. If not, see http://www.gnu.org/licenses/.

using ArcmapSpy.Views;

namespace ArcmapSpy
{
    /// <summary>
    /// ArcMap command which allows to explore the internals of the ArcMap workspaces.
    /// </summary>
    public class WorkspaceSpyCommand : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkspaceSpyCommand"/> class.
        /// </summary>
        public WorkspaceSpyCommand()
        {
            AppStartup.OnStartup();
        }

        protected override void OnClick()
        {
            using (WorkspaceSpyView form = new WorkspaceSpyView())
            {
                form.ShowDialog();
            }
        }

        protected override void OnUpdate()
        {
        }
    }
}
