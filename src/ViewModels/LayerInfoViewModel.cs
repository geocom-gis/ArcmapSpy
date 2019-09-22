// Copyright © 2018 geocom.ch
// This program is subject to the terms of the GPL-3.0 License.
// You should have received a copy of the GNU General Public License
// along with this program. If not, see http://www.gnu.org/licenses/.

using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using ArcmapSpy.Utils;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using GalaSoft.MvvmLight.Command;

namespace ArcmapSpy.ViewModels
{
    /// <summary>
    /// Holds information about a single layer in the ArcMap TOC
    /// </summary>
    public class LayerInfoViewModel : BaseViewModel
    {
        private List<ILayer> _parentLayers;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayerInfoViewModel"/> class.
        /// </summary>
        public LayerInfoViewModel()
        {
            JumpToLayerCommand = new RelayCommand(JumpToLayer);
            RemoveScaleRangeCommand = new RelayCommand(RemoveScaleRange);
            CreateLabelsCommand = new RelayCommand(CreateLabels);
        }

        /// <summary>
        /// Gets the layer the info object is pointing to.
        /// </summary>
        public ILayer Layer { get; set; }

        /// <summary>
        /// Gets or sets the title of the layer displayed in the TOC
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the name of the table, the layers is based on.
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Gets a value indicating, whether the layer and its parent layers are turned on.
        /// It is still possible that they are out of scope and are not displayed.
        /// </summary>
        public bool Visible
        {
            get
            {
                bool result = Layer.Visible;
                foreach (ILayer parentLayer in ParentLayers)
                    result &= parentLayer.Visible;
                return result;
            }
        }

        public string ScaleRange
        {
            get
            {
                string minScale = null;
                if (Layer.MinimumScale != 0.0)
                    minScale = string.Format("1:{0}", Layer.MinimumScale);

                string maxScale = null;
                if (Layer.MaximumScale != 0.0)
                    maxScale = string.Format("1:{0}", Layer.MaximumScale);

                string scale = null;
                if ((minScale != null) || (maxScale != null))
                    scale = string.Format(" ({0}...{1})", minScale, maxScale);

                return scale;
            }
        }

        /// <summary>
        /// Gets the path of all parent group layers as string.
        /// </summary>
        public string Group
        {
            get { return string.Join("/", ParentLayers.Select(parentLayer => parentLayer.Name)); }
        }

        /// <summary>
        /// Gets the current query filter if available.
        /// </summary>
        public string Filter { get; set; }

        /// <summary>
        /// Gets the number of rows of the current layer.
        /// </summary>
        public string RowCount { get; set; }

        /// <summary>
        /// Gets the geometry base type of the layer.
        /// </summary>
        public GeometryBaseType GeometryType
        {
            get { return ArcmapLayerUtils.DetectLayerGeometryType(Layer); }
        }

        /// <summary>
        /// Gets the command which can show the matching layer in ArcMap.
        /// </summary>
        public ICommand JumpToLayerCommand { get; private set; }

        /// <summary>
        /// Expands the table of contents in ArcMap to the matching layer and selects it.
        /// </summary>
        private void JumpToLayer()
        {
            ArcmapLayerUtils.ExpandParentLayers(Layer, ArcmapUtils.GetFocusMap());
            ArcmapLayerUtils.SelectLayer(Layer, ArcMap.Application);
        }

        /// <summary>
        /// Gets the command which can remove the scale range of the layer.
        /// </summary>
        public ICommand RemoveScaleRangeCommand { get; private set; }

        private void RemoveScaleRange()
        {
            if ((Layer.MinimumScale != 0.0) || (Layer.MaximumScale != 0.0))
            {
                // The previous values will still be accessible by ILayerGeneralProperties.LastMinimumScale.
                Layer.MinimumScale = 0;
                Layer.MaximumScale = 0;
            }
            RaisePropertyChanged(nameof(ScaleRange));
            RaisePropertyChanged(nameof(RemoveScaleRangeCommandEnabled));
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="RemoveScaleRangeCommand"/> is enabled or not.
        /// </summary>
        public bool RemoveScaleRangeCommandEnabled
        {
            get { return !string.IsNullOrEmpty(ScaleRange); }
        }

        /// <summary>
        /// Gets the command which can add labels to the features.
        /// </summary>
        public ICommand CreateLabelsCommand { get; private set; }

        private void CreateLabels()
        {
            if (Layer is IGeoFeatureLayer geoFeatureLayer)
            {
                const double TextSizePt = 9.0;
                const double GoodLookingScale = 500.0;
                double referenceScale = ArcmapUtils.GetReferenceScale(GoodLookingScale);
                string tableName = ArcobjWorkspaceUtils.UnqualifyTableName(ArcobjWorkspaceUtils.GetTableName(geoFeatureLayer.FeatureClass as IDataset));
                string oidFieldName = geoFeatureLayer.FeatureClass.OIDFieldName;
                string globalIdFieldName = ArcobjWorkspaceUtils.GetGlobalIdFieldName(geoFeatureLayer.FeatureClass as ITable);
                string expression = FormatLabelExpression(tableName, oidFieldName, globalIdFieldName);

                ITextSymbol textSymbol = new TextSymbolClass();
                textSymbol.Color = ArcmapLayerUtils.DetectLayerMainColor(geoFeatureLayer);
                textSymbol.Size = TextSizePt / referenceScale * GoodLookingScale;

                // Remove existing annotations
                geoFeatureLayer.DisplayAnnotation = false;
                geoFeatureLayer.AnnotationProperties.Clear();

                IAnnotateLayerProperties annotateLayerProperties = new LabelEngineLayerPropertiesClass();
                annotateLayerProperties.UseOutput = true;
                annotateLayerProperties.LabelWhichFeatures = esriLabelWhichFeatures.esriVisibleFeatures;
                annotateLayerProperties.CreateUnplacedElements = true;

                ILabelEngineLayerProperties2 labelEngineProperties = (ILabelEngineLayerProperties2)annotateLayerProperties;
                labelEngineProperties.ExpressionParser = new AnnotationVBScriptEngineClass();
                labelEngineProperties.Expression = expression;
                labelEngineProperties.Symbol = textSymbol;

                IBasicOverposterLayerProperties overposterProperties = labelEngineProperties.BasicOverposterLayerProperties;
                overposterProperties.GenerateUnplacedLabels = true;
                overposterProperties.NumLabelsOption = esriBasicNumLabelsOption.esriOneLabelPerShape;
                overposterProperties.PointPlacementOnTop = true;
                overposterProperties.PointPlacementMethod = esriOverposterPointPlacementMethod.esriSpecifiedAngles;
                overposterProperties.PointPlacementAngles = new[] { 30.0 };
                overposterProperties.LineLabelPosition = CreateLineLabelPosition();

                if (labelEngineProperties.OverposterLayerProperties is IOverposterLayerProperties2 overposterProperties2)
                    overposterProperties2.TagUnplaced = false; // The "place overlapping labels" option

                // Show new annotation in map
                geoFeatureLayer.AnnotationProperties.Add(annotateLayerProperties);
                geoFeatureLayer.DisplayAnnotation = true;
                ArcmapUtils.InvalidateMap(ArcmapUtils.GetFocusMap());
            }
        }

        private static string FormatLabelExpression(string tableName, string oidFieldName, string globalIdFieldName)
        {
            if (!string.IsNullOrEmpty(globalIdFieldName))
                return string.Format(@"""{0} ["" & [{1}] & ""] "" & Left([{2}], 9) & ""…}}""", tableName, oidFieldName, globalIdFieldName);
            else
                return string.Format(@"""{0} ["" & [{1}] & ""]""", tableName, oidFieldName);
        }

        private static ILineLabelPosition CreateLineLabelPosition()
        {
            return new LineLabelPositionClass
            {
                ProduceCurvedLabels = false,
                Above = true,
                Below = false,
                OnTop = false,
                Left = false,
                Right = false,
                InLine = true,
                AtStart = false,
                AtEnd = false,
                Parallel = true,
                Perpendicular = false,
                Horizontal = false,
                Offset = 0.0,
            };
        }

        /// <summary>
        /// Gets a list of parent group layers.
        /// </summary>
        private List<ILayer> ParentLayers
        {
            get { return _parentLayers ?? (_parentLayers = ArcmapLayerUtils.FindParentLayers(Layer, ArcmapUtils.GetFocusMap())); }
        }
    }
}
