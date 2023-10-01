// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine;

namespace GDX.RuntimeContent
{
    public static class ResourceProvider
    {
        public static FontContent Fonts;
        public static StyleSheetContent StyleSheets;
        public static UIElementsContent UIElements;
        public static ShaderContent Shaders;
        
        [RuntimeInitializeOnLoadMethod]
        static void Init()
        {
            Fonts = Resources.Load<FontContent>("GDX.Fonts");
            StyleSheets = Resources.Load<StyleSheetContent>("GDX.StyleSheets");
            UIElements = Resources.Load<UIElementsContent>("GDX.UIElements");
            Shaders = Resources.Load<ShaderContent>("GDX.Shaders");
        }
    }
}