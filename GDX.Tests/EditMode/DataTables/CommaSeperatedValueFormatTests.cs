// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.IO;
using GDX.DataTables.DataBinding;
using GDX.DataTables.DataBinding.Formats;
using NUnit.Framework;

namespace GDX.DataTables
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="CommaSeperatedValueFormat" />.
    /// </summary>
    public class CommaSeperatedValueFormatTests
    {
        static SerializableTable MakeTable(string lastValue)
        {
            string stringType = Serializable.SerializableTypes.String.GetLabel();
            SerializableTable table = new SerializableTable
            {
                DataVersion = 3,
                StructureVersion = 1,
                Headers = new[] { "First", "Second" },
                Types = new[] { stringType, stringType },
                Rows = new[]
                {
                    new SerializableRow(2) { Identifier = 0, Name = "Alpha", Data = new[] { "a", "b" } },
                    new SerializableRow(2) { Identifier = 1, Name = "Beta", Data = new[] { "c", lastValue } }
                }
            };
            return table;
        }

        static SerializableTable RoundTrip(SerializableTable table)
        {
            CommaSeperatedValueFormat format = new CommaSeperatedValueFormat();
            string path = Path.Combine(Path.GetTempPath(), $"gdx-csv-{System.Guid.NewGuid():N}.csv");
            try
            {
                format.Push(path, table);
                return format.Pull(path, 0, 0);
            }
            finally
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }

        [Test]
        [Category(Core.TestCategory)]
        public void RoundTrip_LastValueEmpty_LastRowKeepsEveryColumn()
        {
            SerializableTable result = RoundTrip(MakeTable(string.Empty));

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Rows.Length);
            Assert.AreEqual(2, result.Rows[1].Data.Length);
            Assert.AreEqual("c", result.Rows[1].Data[0]);
            Assert.AreEqual(string.Empty, result.Rows[1].Data[1]);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void RoundTrip_LastValueSet_ValuesKept()
        {
            SerializableTable result = RoundTrip(MakeTable("d"));

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Rows[1].Data.Length);
            Assert.AreEqual("d", result.Rows[1].Data[1]);
            Assert.AreEqual("Beta", result.Rows[1].Name);
        }
    }
}
