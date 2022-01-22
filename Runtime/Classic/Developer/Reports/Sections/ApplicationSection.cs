// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GDX.Classic.Developer.Reports.Sections
{
    public readonly struct ApplicationSection
    {
        /// <summary>
        ///     The name of the scene that was known to the <see cref="UnityEngine.SceneManagement" /> as being the active scene
        ///     when this <see cref="ApplicationSection" /> was created.
        /// </summary>
        public readonly string ActiveScene;

        /// <summary>
        ///     The time of creation for the <see cref="ApplicationSection" />.
        /// </summary>
        public readonly DateTime Created;

        /// <summary>
        ///     The platform that the <see cref="ApplicationSection" /> was created on.
        /// </summary>
        public readonly RuntimePlatform Platform;

        public ApplicationSection(string activeScene, DateTime created, RuntimePlatform platform)
        {
            ActiveScene = activeScene;
            Created = created;
            Platform = platform;
        }

        public static ApplicationSection Get()
        {
            return new ApplicationSection(
                SceneManager.GetActiveScene().name,
                DateTime.Now,
                Application.platform);
        }

        public void Output(ReportContext context, StringBuilder builder)
        {
            builder.AppendLine(context.CreateKVP("Active Scene", ActiveScene));
            builder.AppendLine(context.CreateKVP("Platform", Platform.ToString()));
            builder.AppendLine(context.CreateKVP("Created", Created.ToString(Localization.LocalTimestampFormat)));
        }
    }
}