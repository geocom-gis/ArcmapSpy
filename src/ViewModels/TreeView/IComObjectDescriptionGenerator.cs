// Copyright © 2018 geocom.ch
// This program is subject to the terms of the GPL-3.0 License.
// You should have received a copy of the GNU General Public License
// along with this program. If not, see http://www.gnu.org/licenses/.

using System;

namespace ArcmapSpy.ViewModels.TreeView
{
    interface IComObjectDescriptionGenerator
    {
        string GererateDescription(object obj, Type typ);
    }
}
