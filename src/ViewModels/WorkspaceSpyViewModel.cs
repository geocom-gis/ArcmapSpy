// Copyright © 2018 geocom.ch
// This program is subject to the terms of the GPL-3.0 License.
// You should have received a copy of the GNU General Public License
// along with this program. If not, see http://www.gnu.org/licenses/.

using System;
using System.Collections.Generic;
using System.IO;
using ArcmapSpy.Utils;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;

namespace ArcmapSpy.ViewModels
{
    /// <summary>
    /// ViewModel of the <see cref="WorkspaceSpyCommand"/>.
    /// </summary>
    public class WorkspaceSpyViewModel : BaseViewModel
    {
        private const string NotAvailable = "---";
        private WorkspaceInfoViewModel _selectedWorkspace;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkspaceSpyViewModel"/> class.
        /// </summary>
        public WorkspaceSpyViewModel()
        {
            if (Ioc.IsRegistered<IApplication>())
                Workspaces = LoadWorkspaceInfosFromArcMap();
            else
                Workspaces = new List<WorkspaceInfoViewModel>();
            if (Workspaces.Count > 0)
                SelectedWorkspace = Workspaces[0];
        }

        /// <summary>
        /// Gets a list of currently open workspaces
        /// </summary>
        public List<WorkspaceInfoViewModel> Workspaces { get; private set; }

        /// <summary>
        /// Gets or sets the currently selected workspace from the <see cref="Workspaces"/> list.
        /// </summary>
        public WorkspaceInfoViewModel SelectedWorkspace
        {
            get { return _selectedWorkspace; }
            set { ChangeProperty(ref _selectedWorkspace, value); }
        }

        private List<WorkspaceInfoViewModel> LoadWorkspaceInfosFromArcMap()
        {
            var result = new List<WorkspaceInfoViewModel>();

            var workspaces = ArcmapUtils.EnumerateAllWorkspaces(Ioc.GetOrCreate<IApplication>());
            foreach (IWorkspace workspace in workspaces)
            {
                WorkspaceInfoViewModel info = new WorkspaceInfoViewModel();
                info.Path = workspace.PathName;
                info.Name = Path.GetFileNameWithoutExtension(info.Path);

                ISQLSyntax sqlSyntax = workspace as ISQLSyntax;
                if (sqlSyntax != null)
                {
                    info.StringComparisonCaseSensitive = sqlSyntax.GetStringComparisonCase();
                    info.InvalidCharactersForIdentifiers = sqlSyntax.GetInvalidCharacters();

                    foreach (var enumName in Enum.GetValues(typeof(esriSQLFunctionName)))
                    {
                        string enumValue = sqlSyntax.GetFunctionName((esriSQLFunctionName)enumName);
                        if (string.IsNullOrEmpty(enumValue))
                            enumValue = NotAvailable;
                        info.SqlFunctions.AddOrReplace(enumName.ToString(), enumValue);
                    }
                    info.SqlFunctions.Sort((x, y) => string.Compare(x.Key, y.Key, StringComparison.InvariantCultureIgnoreCase));

                    foreach (var enumName in Enum.GetValues(typeof(esriSQLSpecialCharacters)))
                    {
                        string enumValue = sqlSyntax.GetSpecialCharacter((esriSQLSpecialCharacters)enumName);
                        if (string.IsNullOrEmpty(enumValue))
                            enumValue = string.Empty;
                        info.SpecialCharacters.AddOrReplace(enumName.ToString(), enumValue);
                    }
                    info.SpecialCharacters.Sort((x, y) => string.Compare(x.Key, y.Key, StringComparison.InvariantCultureIgnoreCase));

                    foreach (string keyword in sqlSyntax.GetKeywords().Enumerate())
                    {
                        info.ReservedWords.Add(keyword);
                    }
                    info.ReservedWords.Sort(StringComparer.InvariantCultureIgnoreCase);
                }

                result.Add(info);
            }
            return result;
        }
    }
}
