// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using GDX.Tables;
using UnityEditor;

namespace GDX.Editor
{
    public static class TableCache
    {
        public static AssetDatabaseReference[] FindTables()
        {
            List<AssetDatabaseReference> returnList = new List<AssetDatabaseReference>(5);

            // Get derived types
            TypeCache.TypeCollection tableTypeCollection = TypeCache.GetTypesDerivedFrom<ITable>();
            int count = tableTypeCollection.Count;
            for (int i = 0; i < count; i++)
            {

                string typeDef = tableTypeCollection[i].ToString();
                string[] paths = AssetDatabase.FindAssets($"t:{typeDef}");
                int pathCount = paths.Length;
                for (int j = 0; j < pathCount; j++)
                {
                    returnList.Add(new AssetDatabaseReference(paths[j], tableTypeCollection[i].UnderlyingSystemType));
                }
            }
            return returnList.ToArray();
        }

        static int s_TableTicketHead;
        static readonly Dictionary<int, ITable> k_TicketToTable = new Dictionary<int, ITable>(5);
        static readonly Dictionary<ITable, int> k_TableToTicket = new Dictionary<ITable, int>(5);

        public static ITable GetTable(int ticket)
        {
            return k_TicketToTable.TryGetValue(ticket, out ITable table) ? table : null;
        }

        public static int GetTicket(ITable table)
        {
            return k_TableToTicket.TryGetValue(table, out int ticket) ? ticket : -1;
        }

        public static void UnregisterTable(ITable table)
        {
            int ticket = GetTicket(table);
            k_TableToTicket.Remove(table);
            k_TicketToTable.Remove(ticket);
        }

        public static int RegisterTable(ITable table)
        {
            int ticket = GetTicket(table);
            if (ticket != -1)
            {
                return ticket;
            }

            // Register table
            int head = s_TableTicketHead;
            k_TicketToTable.Add(head, table);
            k_TableToTicket.Add(table, head);

            // Increment our next head
            s_TableTicketHead++;
            return head;
        }
    }
}