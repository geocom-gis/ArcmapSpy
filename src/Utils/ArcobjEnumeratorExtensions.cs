// Copyright © 2018 geocom.ch
// This program is subject to the terms of the GPL-3.0 License.
// You should have received a copy of the GNU General Public License
// along with this program. If not, see http://www.gnu.org/licenses/.

using System.Collections.Generic;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace ArcmapSpy.Utils
{
    /// <summary>
    /// This static class can convert Esri enumerations to DotNet enumerations,
    /// so they can be used in foreach statements or in Linq expressions.
    /// </summary>
    internal static class ArcobjEnumeratorExtensions
    {
        /// <summary>
        /// Converts an Esri enumerable interface to a DotNet IEnumerable.
        /// </summary>
        /// <param name="esriEnum">An enumerable Esri interface.</param>
        /// <returns>The adapted dotnet enumeration.</returns>
        public static IEnumerable<IRow> Enumerate(this ICursor esriEnum)
        {
            for (var item = esriEnum.NextRow(); item != null; item = esriEnum.NextRow())
            {
                yield return item;
            }
        }

        /// <summary>
        /// Converts an Esri enumerable interface to a DotNet IEnumerable.
        /// </summary>
        /// <param name="esriEnum">An enumerable Esri interface.</param>
        /// <returns>The adapted dotnet enumeration.</returns>
        public static IEnumerable<IFeature> Enumerate(this IFeatureCursor esriEnum)
        {
            for (var item = esriEnum.NextFeature(); item != null; item = esriEnum.NextFeature())
            {
                yield return item;
            }
        }

        /// <summary>
        /// Converts an Esri enumerable interface to a DotNet IEnumerable.
        /// </summary>
        /// <param name="enumDataset">An enumerable Esri interface.</param>
        /// <returns>The adapted dotnet enumeration.</returns>
        public static IEnumerable<IDataset> Enumerate(this IEnumDataset enumDataset)
        {
            if (enumDataset != null)
            {
                enumDataset.Reset();
                IDataset dataset = enumDataset.Next();
                while (dataset != null)
                {
                    yield return dataset;
                    dataset = enumDataset.Next();
                }
            }
        }

        /// <summary>
        /// Converts an Esri enumerable interface to a DotNet IEnumerable.
        /// </summary>
        /// <param name="esriSet">An enumerable Esri interface.</param>
        /// <returns>The adapted dotnet enumeration.</returns>
        public static IEnumerable<object> Enumerate(this ISet esriSet)
        {
            esriSet.Reset();
            for (object item = esriSet.Next(); item != null; item = esriSet.Next())
            {
                yield return item;
            }
        }

        /// <summary>
        /// Converts an Esri enumerable interface to a DotNet IEnumerable.
        /// </summary>
        /// <param name="esriArray">Array to loop through.</param>
        /// <returns>A dotnet enumeration of the array.</returns>
        public static IEnumerable<object> Enumerate(this IArray esriArray)
        {
            if (esriArray != null)
            {
                int count = esriArray.Count;
                for (int index = 0; index < count; index++)
                {
                    object item = esriArray.Element[index];
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Converts an Esri enumerable interface to a DotNet IEnumerable.
        /// </summary>
        /// <param name="esriEnum">An enumerable Esri interface.</param>
        /// <returns>The adapted dotnet enumeration.</returns>
        public static IEnumerable<INetWeightAssociation> Enumerate(this IEnumNetWeightAssociation esriEnum)
        {
            esriEnum.Reset();
            for (var item = esriEnum.Next(); item != null; item = esriEnum.Next())
            {
                yield return item;
            }
        }

        /// <summary>
        /// Converts an Esri enumerable interface to a DotNet IEnumerable.
        /// </summary>
        /// <param name="esriEnum">An enumerable Esri interface.</param>
        /// <returns>The adapted dotnet enumeration.</returns>
        public static IEnumerable<int> Enumerate(this IEnumNetEID esriEnum)
        {
            esriEnum.Reset();
            for (int item = esriEnum.Next(); item > 0; item = esriEnum.Next())
            {
                yield return item;
            }
        }

        /// <summary>
        /// Converts an Esri enumerable interface to a DotNet IEnumerable.
        /// </summary>
        /// <param name="esriEnum">An enumerable Esri interface.</param>
        /// <returns>The adapted dotnet enumeration.</returns>
        public static IEnumerable<IPoint> Enumerate(this IEnumVertex esriEnum)
        {
            IPoint point;
            int partIndex;
            int vertexIndex;

            esriEnum.Reset();
            esriEnum.Next(out point, out partIndex, out vertexIndex);
            while (point != null)
            {
                yield return point;
                esriEnum.Next(out point, out partIndex, out vertexIndex);
            }
        }

        /// <summary>
        /// Converts an Esri enumerable interface to a DotNet IEnumerable.
        /// </summary>
        /// <param name="esriEnum">An enumerable Esri interface.</param>
        /// <returns>The adapted dotnet enumeration.</returns>
        public static IEnumerable<ICurve> Enumerate(this IEnumSegment esriEnum)
        {
            ISegment segment;
            var partIndex = 0;
            var segmentIndex = 0;

            esriEnum.Reset();
            esriEnum.Next(out segment, ref partIndex, ref segmentIndex);
            while (segment != null)
            {
                yield return segment;
                esriEnum.Next(out segment, ref partIndex, ref segmentIndex);
            }
        }

        /// <summary>
        /// Converts an Esri enumerable interface to a DotNet IEnumerable.
        /// </summary>
        /// <param name="esriEnum">An enumerable Esri interface.</param>
        /// <returns>The adapted dotnet enumeration.</returns>
        public static IEnumerable<IGeometry> Enumerate(this IGeometryCollection esriEnum)
        {
            var count = esriEnum.GeometryCount;
            for (var index = 0; index < count; index++)
            {
                var geometry = esriEnum.Geometry[index];
                yield return geometry;
            }
        }

        /// <summary>
        /// Converts an Esri enumerable interface to a DotNet IEnumerable.
        /// </summary>
        /// <param name="esriEnum">An enumerable Esri interface.</param>
        /// <returns>The adapted dotnet enumeration.</returns>
        public static IEnumerable<IGeometry> Enumerate(this IEnumGeometry esriEnum)
        {
            if (esriEnum != null)
            {
                esriEnum.Reset();
                IGeometry esriEnumItem = esriEnum.Next();
                while (esriEnumItem != null)
                {
                    yield return esriEnumItem;
                    esriEnumItem = esriEnum.Next();
                }
            }
        }

        /// <summary>
        /// Converts an Esri enumerable interface to a DotNet IEnumerable.
        /// </summary>
        /// <param name="esriEnum">An enumerable Esri interface.</param>
        /// <returns>The adapted dotnet enumeration.</returns>
        public static IEnumerable<IPoint> Enumerate(this IMultipoint esriEnum)
        {
            return (esriEnum as IPointCollection).EnumVertices.Enumerate();
        }

        /// <summary>
        /// Converts an Esri enumerable interface to a DotNet IEnumerable.
        /// </summary>
        /// <param name="esriEnum">An enumerable Esri interface, e.g.
        /// map.FeatureSelection as IEnumFeature .</param>
        /// <returns>The adapted dotnet enumeration.</returns>
        public static IEnumerable<IFeature> Enumerate(this IEnumFeature esriEnum)
        {
            // Return all columns of the feature
            if (esriEnum is IEnumFeatureSetup)
            {
                ((IEnumFeatureSetup) esriEnum).AllFields = true;
            }

            IFeature feature;
            esriEnum.Reset();
            while ((feature = esriEnum.Next()) != null)
            {
                yield return feature;
            }
        }

        /// <summary>
        /// Converts an Esri enumerable interface to a DotNet IEnumerable.
        /// </summary>
        /// <param name="esriEnum">An enumerable Esri interface.</param>
        /// <returns>The adapted dotnet enumeration.</returns>
        public static IEnumerable<IName> Enumerate(this IEnumName enumEnum)
        {
            if (enumEnum != null)
            {
                enumEnum.Reset();
                IName datasetName = enumEnum.Next();
                while (datasetName != null)
                {
                    yield return datasetName;
                    datasetName = enumEnum.Next();
                }
            }
        }

        /// <summary>
        /// Converts an Esri enumerable interface to a DotNet IEnumerable.
        /// </summary>
        /// <param name="esriEnum">An enumerable Esri interface.</param>
        /// <returns>The adapted dotnet enumeration.</returns>
        public static IEnumerable<string> Enumerate(this IEnumBSTR esriEnum)
        {
            if (esriEnum != null)
            {
                esriEnum.Reset();
                string esriEnumItem = esriEnum.Next();
                while (esriEnumItem != null)
                {
                    yield return esriEnumItem;
                    esriEnumItem = esriEnum.Next();
                }
            }
        }
    }
}
