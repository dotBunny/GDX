// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine;

namespace GDX.RuntimeContent
{
    public static class ResourceProvider
    {
        static FontContent s_Fonts;
        static StyleSheetContent s_StyleSheets;
        static UIElementsContent s_UIElements;
        static ShaderContent s_Shaders;


        public static FontContent GetFonts()
        {
            if (s_Fonts == null)
            {
                s_Fonts = Resources.Load<FontContent>("GDX.Fonts");
            }

            return s_Fonts;
        }


        public static StyleSheetContent GetStyleSheets()
        {
            if (s_StyleSheets == null)
            {
                s_StyleSheets = Resources.Load<StyleSheetContent>("GDX.StyleSheets");
            }

            return s_StyleSheets;
        }

        public static UIElementsContent GetUIElements()
        {
            if (s_UIElements == null)
            {
                s_UIElements = Resources.Load<UIElementsContent>("GDX.UIElements");
            }

            return s_UIElements;
        }

        public static ShaderContent GetShaders()
        {
            if (s_Shaders == null)
            {
                s_Shaders = Resources.Load<ShaderContent>("GDX.Shaders");
            }

            return s_Shaders;
        }
    }
}