# Serializable Dictionary

## A New Beginning

A typical piece of feedback we've heard repeatedly is that Unity needs to have a Dictionary collection capable of being serialized. With the introduction of template-based property drawers in `Unity 2020.1`, the final piece to solving that puzzle landed. While the core functionality backing the SerializableDictionary is compatible across all supported versions of Unity. The real power comes from the freshly acquired property drawer features.

> [!WARNING]
> Unity's internal serializer cannot serialize `System.Object`, objects must inherit from `UnityEngine.Object` instead.

## Familiar Workflow

![Property Drawer](/images/manual/features/serializable-dictionary/string-prefab.png)

One of the pillars of design for the `SerializableDictionary` was making sure that its workflow in editor kept in line with Unity's `OrderedList` inspector, with some obvious contextual tweaks. The left side (**A**) contains the entry's key, with its value (**B**) being listed directly beside it. Clicking on that row selects it, making the subtraction (**-**) button on the right available.  

> [!NOTE] 
> The addition button (**+**) only becomes available when a valid and unique key is provided.

The ability to edit the number of elements in the collection (**C**) is disabled due to the difficulty of predetermining valid keys for various types effectively.

## Code Example

```csharp
using GDX.Collections.Generic;

public class SerializableDictionaryExamples
{
    // An integer keyed collection of strings
    public SerializableDictionary<int, string> IntegerStringKVP = new SerializableDictionary<int, string>();

    // A string keyed collection of GameObjects (works with Prefabs!)
    public SerializableDictionary<string, GameObject> StringGameObjectKVP = new SerializableDictionary<string, GameObject>();
}
```
