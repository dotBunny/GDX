﻿// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using GDX.Developer;
using UnityEditor;
using Console = System.Console;

namespace GDX.Editor.ConsoleCommands
{
#if UNITY_2022_2_OR_NEWER
    public class ClearConsoleCommand : ConsoleCommandBase
    {
        public override bool Evaluate(float deltaTime)
        {
            Assembly assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
            Type type = assembly.GetType("UnityEditor.LogEntries");
            MethodInfo method = type.GetMethod("Clear");
            if (method != null)
            {
                method.Invoke(new object(), null);
            }

            GDX.Developer.Console.Log.Clear();

            return true;
        }

        public override string GetKeyword()
        {
            return "clear";
        }

        public override string GetHelpMessage()
        {
            return "Clears the editor console.";
        }

        public override bool IsEditorOnly()
        {
            return true;
        }

        [InitializeOnLoadMethod]
        static void RegisterCommand()
        {
            Developer.Console.RegisterCommand(new ClearConsoleCommand());
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}