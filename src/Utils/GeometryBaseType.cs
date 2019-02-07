// Copyright © 2018 geocom.ch
// This program is subject to the terms of the GPL-3.0 License.
// You should have received a copy of the GNU General Public License
// along with this program. If not, see http://www.gnu.org/licenses/.

using System;

namespace ArcmapSpy.Utils
{
    /// <summary>
    /// Enumeration of possible geometry base types.
    /// </summary>
    public enum GeometryBaseType
    {
        /// <summary>Unknown type</summary>
        Unknown = 0,

        /// <summary>All kinds of point types, including collections of points.</summary>
        Point = 1,

        /// <summary>All kinds of line types, including poly lines.</summary>
        Line = 2,

        /// <summary>All kinds of surface types.</summary>
        Area = 3
    }
}
