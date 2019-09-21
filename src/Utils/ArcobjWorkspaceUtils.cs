// Copyright © 2018 geocom.ch
// This program is subject to the terms of the GPL-3.0 License.
// You should have received a copy of the GNU General Public License
// along with this program. If not, see http://www.gnu.org/licenses/.

using System;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Location;

namespace ArcmapSpy.Utils
{
    /// <summary>
    /// Helper functions to work with ArcObjects workspaces.
    /// </summary>
    public static class ArcobjWorkspaceUtils
    {
        /// <summary>
        /// Gets the name of the workspace, as it is used in the Geonis desktop environment.
        /// </summary>
        /// <param name="esriWorkspace">Esri workspace to get the name from.</param>
        /// <returns>The name/path of the workspace.</returns>
        public static string GetWorkspaceName(IWorkspace esriWorkspace)
        {
            return esriWorkspace.PathName;
        }

        /// <summary>
        /// Gets the name of the workspace, as it is used in the Geonis desktop environment.
        /// </summary>
        /// <param name="esriDataset">Esri dataset to get the workspace name from.</param>
        /// <returns>The name/path of the workspace.</returns>
        public static string GetWorkspaceName(IDataset esriDataset)
        {
            IDataset baseDataset = DigForBaseDataset(esriDataset);
            return GetWorkspaceName(baseDataset.Workspace);
        }

        /// <summary>
        /// Gets the name of the workspace, as it is used in the Geonis desktop environment.
        /// </summary>
        /// <param name="esriRow">Esri row to get the workspace name from.</param>
        /// <returns>The name/path of the workspace.</returns>
        public static string GetWorkspaceName(IRow esriRow)
        {
            return GetWorkspaceName(esriRow.Table as IDataset);
        }

        /// <summary>
        /// Gets the name of the given Esri table.
        /// </summary>
        /// <param name="esriTable">Esri table to get the name from.</param>
        /// <returns>The name of the table, possibly containing a qualifier.</returns>
        public static string GetTableName(ITable esriTable)
        {
            return GetTableName(esriTable as IDataset);
        }

        /// <summary>
        /// Gets the name of the given Esri dataset. If the dataset contains nested datasets
        /// (e.g. routeevent) the function digs for the base table.
        /// </summary>
        /// <param name="esriDataset">Esri dataset to get the name from.</param>
        /// <returns>The name of the dataset, possibly containing a qualifier.</returns>
        public static string GetTableName(IDataset esriDataset)
        {
            IDataset baseDataset = DigForBaseDataset(esriDataset);
            return baseDataset.Name;
        }

        /// <summary>
        /// Extracts the table name from a qualified table name (e.g. qualifier.tablename).
        /// If the the table name does not contain a qualifier, the table name itself is returned.
        /// </summary>
        /// <param name="tableName">Table name, which may include a qualifier.</param>
        /// <returns>Table name without qualifier.</returns>
        public static string UnqualifyTableName(string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
                return tableName;

            int pos = tableName.LastIndexOf('.');
            return (pos >= 0)
                ? tableName.Remove(0, pos + 1)
                : tableName;
        }

        /// <summary>
        /// Gets the name of the GlobalId field if available, otherwise null.
        /// </summary>
        /// <param name="esriTable">The table to get the field name from.</param>
        /// <returns>Name of the GlobalId field, or null if no such field exists.</returns>
        public static string GetGlobalIdFieldName(ITable esriTable)
        {
            if ((esriTable is IClassEx classEx) && classEx.HasGlobalID)
                return classEx.GlobalIDFieldName;
            return null;
        }

        /// <summary>
        /// Determines whether an Esri table has a GlobalId field or not.
        /// </summary>
        /// <param name="esriTable">The Esri table object to check.</param>
        /// <returns>Returns true if a GlobalId field is available, otherwise false.</returns>
        public static bool TableHasGlobalId(ITable esriTable)
        {
            if (esriTable is IClassEx classEx)
                return classEx.HasGlobalID;
            return false;
        }

        /// <summary>
        /// Gets the id value from a given Esri row.
        /// </summary>
        /// <param name="esriRow">The Esri row to get the id from.</param>
        /// <param name="preferGlobalId">If this parameter is true and the row contains a
        /// GlobalId field, the function returns the GlobalId, otherwise the ObjectId.</param>
        /// <returns>Returns the id of the row.</returns>
        public static object GetIdFromRow(IRow esriRow, bool preferGlobalId)
        {
            ITable esriTable = esriRow.Table;
            bool returnGlobalId = preferGlobalId && TableHasGlobalId(esriTable);
            if (returnGlobalId)
            {
                string fieldName = (esriTable as IClassEx).GlobalIDFieldName;
                int fieldIndex = esriRow.Fields.FindField(fieldName);
                return esriRow.Value[fieldIndex];
            }

            if (esriRow.HasOID)
                return esriRow.OID;

            throw new Exception(string.Format("The id of the row of table {0} could not be determined.", GetTableName(esriTable)));
        }

        /// <summary>
        /// If the dataset contains nested datasets (e.g. routeevent) the function recursively
        /// digs for the base dataset, otherwise the dataset itself is returned.
        /// </summary>
        /// <param name="esriDataset">Esri dataset to find the base dataset for.</param>
        /// <returns>Base dataset.</returns>
        private static IDataset DigForBaseDataset(IDataset esriDataset)
        {
            if (esriDataset is IVirtualTable)
            {
                ITable nestedTable = null;
                if (esriDataset is IRouteEventSource)
                    nestedTable = (esriDataset as IRouteEventSource).EventTable;
                else if (esriDataset is IXYEventSource)
                    nestedTable = (esriDataset as IXYEventSource).EventTable;
                else if (esriDataset is IRelQueryTable)
                    nestedTable = (esriDataset as IRelQueryTable).SourceTable;

                if (nestedTable != null)
                    return DigForBaseDataset(nestedTable as IDataset); // recursive call
            }
            return esriDataset;
        }
    }
}
