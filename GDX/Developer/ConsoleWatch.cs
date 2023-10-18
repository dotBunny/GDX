// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Globalization;

namespace GDX.Developer
{
#if UNITY_2022_2_OR_NEWER
    public class ConsoleWatch
    {
        public int WatchID;
        public string DisplayText;
        public string DisplayValue;

        public void Update(float value)
        {
            DisplayValue = value.ToString(CultureInfo.InvariantCulture);
        }
        public void Update(int value)
        {
            DisplayValue = value.ToString(CultureInfo.InvariantCulture);
        }
        public void Update(string value)
        {
            DisplayValue = value;
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}