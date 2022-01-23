﻿// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine.UIElements;

namespace GDX.Editor
{
    public interface IConfigSection
    {
        bool GetDefaultVisibility();
        string GetSectionID();
        string GetSectionHeaderLabel();
        string GetSectionHelpLink();
        bool GetToggleSupport();

        void DrawSectionHeader(GDXConfig config);
        void DrawSectionContent(GDXConfig config);
    }
}