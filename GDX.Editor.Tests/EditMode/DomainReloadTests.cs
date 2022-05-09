// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.IO;
using System.Text;
using NUnit.Framework;
using Unity.Collections;
using UnityEditor;
using UnityEngine.TestTools;
using Application = UnityEngine.Application;

namespace GDX.Editor
{
    /// <summary>
    ///     A collection of unit tests to validate for issues when compiling or reloading the domain.
    /// </summary>
    public class DomainReloadTests
    {
        const string k_LeakDetectionModeKey = "GDX_DomainReloadTests_LeakDetectionMode";
        static string GetFilePath()
        {
            return Path.Combine(Application.dataPath, "DomainReloadTest.cs");
        }

        static string GetRandomCode()
        {
            StringBuilder randomCode = new StringBuilder(10);

            randomCode.Append(
                Platform.SafeCharacterPool[Core.Random.NextInteger(0, Platform.CharacterPoolLengthExclusive)]);
            randomCode.Append(
                Platform.SafeCharacterPool[Core.Random.NextInteger(0, Platform.CharacterPoolLengthExclusive)]);
            randomCode.Append(
                Platform.SafeCharacterPool[Core.Random.NextInteger(0, Platform.CharacterPoolLengthExclusive)]);
            randomCode.Append(
                Platform.SafeCharacterPool[Core.Random.NextInteger(0, Platform.CharacterPoolLengthExclusive)]);
            randomCode.Append(
                Platform.SafeCharacterPool[Core.Random.NextInteger(0, Platform.CharacterPoolLengthExclusive)]);

            return randomCode.ToString();
        }

        static string GetFileContent()
        {
            string randomCode = GetRandomCode();

            Developer.TextGenerator code = new Developer.TextGenerator("    ", "{", "}");
            code.AppendLine("// Generated domain reload test file.");
            code.AppendLine("#pragma warning disable");
            code.AppendLine("// ReSharper disable All");
            code.AppendLine($"namespace DomainReloadTest_{randomCode}");
            code.PushIndent();
            code.AppendLine($"public class Test_{randomCode}");
            code.PushIndent();
            code.AppendLine("[UnityEngine.Scripting.Preserve]");
            code.AppendLine($"public void Init()");
            code.PushIndent();
            code.AppendLine("UnityEngine.Debug.Log(\"Domain Reload Test\");");

            return code.ToString();
        }

        [UnitySetUp]
        public IEnumerator Setup()
        {
            EditorPrefs.SetInt(k_LeakDetectionModeKey, (int)NativeLeakDetection.Mode);
            string filePath = GetFilePath();
            Platform.ForceDeleteFile(filePath);
            Platform.ForceDeleteFile($"{filePath}.meta");
            yield return null;
        }

        [UnityTest]
        [Category(Core.TestCategory)]
        public IEnumerator Recompile_NoLeakDetection_NoUnexpectedMessages()
        {
            NativeLeakDetection.Mode = NativeLeakDetectionMode.Disabled;
            File.WriteAllText(GetFilePath(),GetFileContent());
            AssetDatabase.ImportAsset(GetFilePath().Replace(Application.dataPath + "\\", "Assets/"));
            yield return new RecompileScripts();

            // Any warning or error will fail this
            LogAssert.NoUnexpectedReceived();
        }

        [UnityTest]
        [Category(Core.TestCategory)]
        public IEnumerator Recompile_JobsLeakDetection_NoUnexpectedMessages()
        {
            NativeLeakDetection.Mode = NativeLeakDetectionMode.Enabled;
            File.WriteAllText(GetFilePath(),GetFileContent());
            AssetDatabase.ImportAsset(GetFilePath().Replace(Application.dataPath + "\\", "Assets/"));
            yield return new RecompileScripts();

            // Any warning or error will fail this
            LogAssert.NoUnexpectedReceived();
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            NativeLeakDetection.Mode = (NativeLeakDetectionMode)EditorPrefs.GetInt(k_LeakDetectionModeKey);
            string filePath = GetFilePath();
            Platform.ForceDeleteFile(filePath);
            Platform.ForceDeleteFile($"{filePath}.meta");
            yield return null;
        }
    }
}