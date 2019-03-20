// Copyright © 2018 geocom.ch
// This program is subject to the terms of the GPL-3.0 License.
// You should have received a copy of the GNU General Public License
// along with this program. If not, see http://www.gnu.org/licenses/.

using System.Collections.Generic;
using System.Text;
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

        public bool IdentifierCaseSensitive { get; set; }
        public bool QuotedIdentifierCaseSensitive { get; set; }
        public bool StringComparisonCaseSensitive { get; set; }

        public string InvalidCharactersForIdentifiers { get; set; }
        public KeyValueList<string, string> SqlFunctions { get; private set; }
        public KeyValueList<string, string> SpecialCharacters { get; private set; }
        public List<string> ReservedWords { get; private set; }

        public string CaseSensitive
        {
            get
            {
                string startQuote;
                string endQuote;
                GetQuotes(out startQuote, out endQuote);

                StringBuilder result = new StringBuilder();
                result.AppendLine(string.Format("Identifier:  {0}", IdentifierCaseSensitive));
                result.AppendLine(string.Format("{0}Quoted identifier{1}:  {2}", startQuote, endQuote, QuotedIdentifierCaseSensitive));
                result.AppendLine(string.Format("String comparison:  {0}",  StringComparisonCaseSensitive));
                return result.ToString();
            }
        }

        private void GetQuotes(out string startQuote, out string endQuote)
        {
            SpecialCharacters.TryGetValue("esriSQL_DelimitedIdentifierPrefix", out startQuote);
            SpecialCharacters.TryGetValue("esriSQL_DelimitedIdentifierSuffix", out endQuote);
        }
    }
}
