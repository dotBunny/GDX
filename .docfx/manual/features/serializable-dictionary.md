# Serializable Dictionary

## A New Beginning
A typical piece of feedback we've heard repeatedly is that Unity needs to have a Dictionary collection capable of being serialized. With the introduction of template-based property drawers in `Unity 2020.1`, the final piece to solving that puzzle landed. While the core functionality backing the SerializableDictionary is compatible across all supported versions of Unity. The real power comes from the freshly acquired property drawer features.

> [!IMPORTANT]
> Unity's internal serializer cannot serialize `System.Object`, objects must inherit from `UnityEngine.Object` for it to work.

##

