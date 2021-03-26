// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace GDX
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class VisualScriptingAttribute : Attribute
    {
        public enum Category
        {
            Extensions,
            Types
        }

        public Category _category;

        public VisualScriptingAttribute(Category category) {
            _category = category;
        }
    }
}