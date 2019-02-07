// Copyright © 2018 geocom.ch
// This program is subject to the terms of the GPL-3.0 License.
// You should have received a copy of the GNU General Public License
// along with this program. If not, see http://www.gnu.org/licenses/.

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;

namespace ArcmapSpy.Utils
{
    /// <summary>
    /// Describes one possible candidate of a pick selection.
    /// </summary>
    internal class ArcmapPickCandidate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArcmapPickCandidate"/> class.
        /// </summary>
        /// <param name="identifyOby">Identified object.</param>
        /// <param name="distance">Distance from the identified object to the mouse click.</param>
        public ArcmapPickCandidate(IIdentifyObj identifyOby, double distance)
        {
            IdentifyObj = identifyOby;
            DistanceToClickPoint = distance;
            GeometryType = ArcmapUtils.DetectGeometryBaseType(Feature.Shape);
        }

        /// <summary>
        /// Gets the identified object.
        /// </summary>
        public IIdentifyObj IdentifyObj { get; private set; }

        /// <summary>
        /// Gets the feature of the identified object.
        /// </summary>
        public IFeature Feature
        {
            get { return (IdentifyObj as IRowIdentifyObject)?.Row as IFeature; }
        }

        /// <summary>
        /// Gets the layer of the identified object.
        /// </summary>
        public IFeatureLayer Layer
        {
            get { return IdentifyObj.Layer as IFeatureLayer; }
        }

        public GeometryBaseType GeometryType { get; private set; }

        private double DistanceToClickPoint { get; set; }

        /// <summary>
        /// Compares the candidate with another candidate, to determine the favorited candidate.
        /// </summary>
        /// <param name="other">Other candidate to compare with.</param>
        /// <returns>Returns a value, indicating which one is the better candidate.</returns>
        public int CompareTo(ArcmapPickCandidate other)
        {
            int result = GeometryType.CompareTo(other.GeometryType);
            if (result == 0)
                result = DistanceToClickPoint.CompareTo(other.DistanceToClickPoint);
            return result;
        }
    }
}
