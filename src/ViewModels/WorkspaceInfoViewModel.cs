// Copyright © 2018 geocom.ch
// This program is subject to the terms of the GPL-3.0 License.
// You should have received a copy of the GNU General Public License
// along with this program. If not, see http://www.gnu.org/licenses/.

using System.Collections.Generic;
using ArcmapSpy.Utils;

namespace ArcmapSpy.ViewModels
{
    /// <summary>
    /// Holds information about an Esri workspace.
    /// </summary>
    public class WorkspaceInfoViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkspaceInfoViewModel"/> class.
        /// </summary>
        public WorkspaceInfoViewModel()
        {
            SqlFunctions = new KeyValueList<string, string>();
            SpecialCharacters = new KeyValueList<string, string>();
            ReservedWords = new List<string>();
        }

        public string Name { get; set; }
        public string Path { get; set; }
        public bool StringComparisonCaseSensitive { get; set; }
        public string InvalidCharactersForIdentifiers { get; set; }

        public KeyValueList<string, string> SqlFunctions { get; private set; }
        public KeyValueList<string, string> SpecialCharacters { get; private set; }
        public List<string> ReservedWords { get; private set; }
    }
}
