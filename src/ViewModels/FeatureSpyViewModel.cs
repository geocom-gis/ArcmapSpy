// Copyright © 2018 geocom.ch
// This program is subject to the terms of the GPL-3.0 License.
// You should have received a copy of the GNU General Public License
// along with this program. If not, see http://www.gnu.org/licenses/.

using System.Collections.Generic;
using System.Windows.Input;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using GalaSoft.MvvmLight.Command;
using ArcmapSpy.Utils;

namespace ArcmapSpy.ViewModels
{
    public class FeatureSpyViewModel : BaseViewModel
    {
        private readonly IFeature _feature;
        private readonly ILayer _layer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureSpyViewModel"/>.
        /// </summary>
        public FeatureSpyViewModel()
            : this(null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureSpyViewModel"/>.
        /// <param name="feature">The feature to represent.</param>
        /// <param name="layer">The layer from which the feature originated.</param>
        /// </summary>
        public FeatureSpyViewModel(IFeature feature, ILayer layer)
        {
            _feature = feature;
            _layer = layer;
            ColumnValues = new List<ColumnInfoViewModel>();
            JumpToLayerCommand = new RelayCommand(JumpToLayer);
            SelectFeatureCommand = new RelayCommand(SelectFeature);

            // Analyse the feature
            if (feature != null)
            {
                WorkspaceName = ArcobjWorkspaceUtils.GetWorkspaceName(_feature);
                TableName = ArcobjWorkspaceUtils.GetTableName(_feature.Table);
                ObjectId = _feature.OID;
                object globalId = ArcobjWorkspaceUtils.GetIdFromRow(_feature, true);
                if (globalId is string)
                    GlobalId = globalId.ToString();
                else
                    GlobalId = string.Empty;
                FillColumnValues(ColumnValues, _feature);
            }
            if (_layer != null)
            {
                LayerName = _layer.Name;
            }
        }

        /// <summary>
        /// Gets the name of the layer the feature belongs to.
        /// </summary>
        public string LayerName { get; private set; }

        /// <summary>
        /// Gets the name of the workspace the feature belongs to.
        /// </summary>
        public string WorkspaceName { get; private set; }

        /// <summary>
        /// Gets the name of the table the feature belongs to.
        /// </summary>
        public string TableName { get; private set; }

        /// <summary>
        /// Gets the objectid of the feature.
        /// </summary>
        public int ObjectId { get; private set; }

        /// <summary>
        /// Gets the GlobalId of the feature, or an empty guid if no such column exists.
        /// </summary>
        public string GlobalId { get; private set; }

        /// <summary>
        /// Gets a list of column-value pairs.
        /// </summary>
        public List<ColumnInfoViewModel> ColumnValues { get; }

        /// <summary>
        /// Gets the command which can show the matching layer in ArcMap.
        /// </summary>
        public ICommand JumpToLayerCommand { get; private set; }

        /// <summary>
        /// Expands the table of contents in ArcMap to the matching layer and selects it.
        /// </summary>
        private void JumpToLayer()
        {
            ArcmapLayerUtils.ExpandParentLayers(_layer, ArcmapUtils.GetFocusMap());
            ArcmapLayerUtils.SelectLayer(_layer, ArcMap.Application);
        }

        /// <summary>
        /// Gets the command which can show the matching layer in ArcMap.
        /// </summary>
        public ICommand SelectFeatureCommand { get; private set; }

        /// <summary>
        /// Expands the table of contents in ArcMap to the matching layer and selects it.
        /// </summary>
        private void SelectFeature()
        {
            IFeatureSelection featureSelection;
            IMap map = ArcmapUtils.GetFocusMap();
            foreach (IFeatureLayer featureLayer in ArcmapLayerUtils.EnumerateLayers(map, false))
            {
                featureSelection = featureLayer as IFeatureSelection;
                featureSelection?.Clear();
            }

            featureSelection = _layer as IFeatureSelection;
            featureSelection?.Add(_feature);

            ArcmapUtils.RefreshMap(map);
        }

        private void FillColumnValues(List<ColumnInfoViewModel> columnValues, IFeature feature)
        {
            int subtypeFieldIndex = -1;
            ISubtypes subtypes = feature.Class as ISubtypes;
            if (subtypes != null)
                subtypeFieldIndex = subtypes.SubtypeFieldIndex;
            KeyValueList<int, string> subtypeValues = ReadSubtypeDomainValues(feature);

            for (int columnIndex = 0; columnIndex < feature.Fields.FieldCount; columnIndex++)
            {
                IField field = feature.Fields.Field[columnIndex];
                object value = feature.Value[columnIndex];
                bool isSubtype = columnIndex == subtypeFieldIndex;
                KeyValueList<object, string> domainValues = ReadColumnDomainValues(field);

                ColumnInfoViewModel columnInfo = new ColumnInfoViewModel
                {
                    ColumnName = field.Name,
                    Alias = field.AliasName,
                    FieldType = field.Type.ToString(),
                    Required = field.Required,
                    Editable = field.Editable,
                    Nullable = field.IsNullable,
                    Length = field.Length,
                    Precision = field.Precision,
                    Value = value.ToString(),
                    IsSubtype = isSubtype,
                    SubtypeValues = isSubtype ? subtypeValues : null,
                    DomainValues = domainValues,
                };

                if (columnInfo.Required)
                    columnInfo.ColumnName += " *";
                columnValues.Add(columnInfo);
            }
        }

        private static KeyValueList<int, string> ReadSubtypeDomainValues(IFeature feature)
        {
            ISubtypes subtypes = feature.Class as ISubtypes;
            IEnumSubtype enumSubtype = subtypes?.Subtypes;
            if (enumSubtype == null)
                return null;

            var result = new KeyValueList<int, string>();
            enumSubtype.Reset();
            int subtypeCode;

            string description = enumSubtype.Next(out subtypeCode);
            while (description != null)
            {
                result.AddOrReplace(subtypeCode, description);
                description = enumSubtype.Next(out subtypeCode);
            }
            return result;
        }


        private KeyValueList<object, string> ReadColumnDomainValues(IField field)
        {
            ICodedValueDomain codedValueDomain = field?.Domain as ICodedValueDomain;
            if (codedValueDomain == null)
                return null;

            var result = new KeyValueList<object, string>();
            for (var index = 0; index < codedValueDomain.CodeCount; index++)
            {
                result.AddOrReplace(codedValueDomain.Value[index], codedValueDomain.Name[index]);
            }
            return result;
        }
    }
}
