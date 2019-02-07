// Copyright © 2018 geocom.ch
// This program is subject to the terms of the GPL-3.0 License.
// You should have received a copy of the GNU General Public License
// along with this program. If not, see http://www.gnu.org/licenses/.

using System.Collections;
using ArcmapSpy.ViewModels.TreeView;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace ArcmapSpy
{
    /// <summary>
    /// Builds the dependencies of the application at startup time.
    /// </summary>
    public static class AppStartup
    {
        public static void OnStartup()
        {
            if (IsFirstTime())
            {
                Ioc.RegisterFactory<IApplication>(() => ArcMap.Application);
                Ioc.RegisterFactoryWithKey<IComObjectDescriptionGenerator>(() => new ComObjectDescriptionGeneratorIClassid(), typeof(IClassID).FullName);
                Ioc.RegisterFactoryWithKey<IComObjectDescriptionGenerator>(() => new ComObjectDescriptionGeneratorIPropertySet(), typeof(IPropertySet).FullName);
                Ioc.RegisterFactoryWithKey<IComObjectNodeExpander>(() => new ComObjectNodeExpanderIEnumerable(), typeof(IEnumerable).FullName);
                Ioc.RegisterFactoryWithKey<IComObjectNodeExpander>(() => new ComObjectNodeExpanderIArray(), typeof(IArray).FullName);
                Ioc.RegisterFactoryWithKey<IComObjectNodeExpander>(() => new ComObjectNodeExpanderISet(), typeof(ISet).FullName);
                Ioc.RegisterFactoryWithKey<IComObjectNodeExpander>(() => new ComObjectNodeExpanderIEnumName(), typeof(IEnumName).FullName);
                Ioc.RegisterFactoryWithKey<IComObjectNodeExpander>(() => new ComObjectNodeExpanderIEnumDatasetName(), typeof(IEnumDatasetName).FullName);
                Ioc.RegisterFactoryWithKey<IComObjectNodeExpander>(() => new ComObjectNodeExpanderIEnumDataset(), typeof(IEnumDataset).FullName);
                Ioc.RegisterFactoryWithKey<IComObjectNodeExpander>(() => new ComObjectNodeExpanderIEnumVertex(), typeof(IEnumVertex).FullName);
                Ioc.RegisterFactoryWithKey<IComObjectNodeExpander>(() => new ComObjectNodeExpanderIEnumSegment(), typeof(IEnumSegment).FullName);
                Ioc.RegisterFactoryWithKey<IComObjectNodeExpander>(() => new ComObjectNodeExpanderIEnumGeometry(), typeof(IEnumGeometry).FullName);
            }
        }

        private static bool IsFirstTime()
        {
            return !Ioc.IsRegistered<IApplication>();
        }
    }
}
