// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace GDX
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class VisualScriptingNodeAttribute : Attribute
    {
        public enum Category
        {
            Extensions,
            Types
        }

        public readonly Category _type;

        public VisualScriptingNodeAttribute(Category type) {
            _type = type;
        }
    }
}