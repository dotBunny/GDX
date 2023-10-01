using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace GDX.RuntimeContent
{
    public class UIElementsContent : ScriptableObject
    {
        [FormerlySerializedAs("Console")] public VisualTreeAsset RuntimeConsole;
    }
}