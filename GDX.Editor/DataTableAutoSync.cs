// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using GDX.DataTables;
using UnityEditor;

namespace GDX.Editor
{
    public class DataTableAutoSync
    {
        public static void Sync(DataTableBase dataTable)
        {
            DataTableMetaData metaData = dataTable.GetMetaData();
            string filePath = metaData.GetAbsoluteSourceOfTruth();
            switch (metaData.SyncFormat)
            {
                case DataTableInterchange.Format.CommaSeperatedValues:
                case DataTableInterchange.Format.JavaScriptObjectNotation:
                    if (!System.IO.File.Exists(filePath))
                    {
                        UnityEngine.Debug.LogError($"The source of truth cannot be found at {filePath}.");
                        return;
                    }

                    DateTime lastWriteTime = System.IO.File.GetLastWriteTimeUtc(filePath);

                    if (lastWriteTime < metaData.SyncTimestamp || metaData.SyncDataVersion < dataTable.GetDataVersion())
                    {
                        // Export
                        DataTableInterchange.Export(dataTable, metaData.SyncFormat, filePath);
                        metaData.SyncTimestamp = System.IO.File.GetLastWriteTimeUtc(filePath);
                        metaData.SyncDataVersion = dataTable.GetDataVersion();
                        EditorUtility.SetDirty(metaData);
                    }

                    if (metaData.SyncTimestamp < lastWriteTime)
                    {
                        // Import
                        DataTableInterchange.Import(dataTable, metaData.SyncFormat, filePath);
                        DataTableTracker.NotifyOfRowChange(DataTableTracker.GetTicket(dataTable), -1);
                        metaData.SyncTimestamp = lastWriteTime;
                        metaData.SyncDataVersion = dataTable.GetDataVersion();
                        EditorUtility.SetDirty(metaData);
                    }

                    break;

            }
        }
    }
}