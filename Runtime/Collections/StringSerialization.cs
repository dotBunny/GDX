// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using GDX.Collections.Generic;

public class EditorStrings : ScriptableObject
{
    // BIG TODOS:
    // 1. Make localizations their own ScriptableObjects for flexibility.
    // 2. Revamp runtime data so that IDs and metadata are not in the big char array.
    // 3. Possibly separate runtime tags from literals.
    // 4. Figure out if I'm supporting duplicate tags between categories at runtime, or if it's just a convenience thing for editor like file folders.
    // 5. Runtime int-to-string conversion support to start with, float-to-string (and others) later.
    [HideInInspector] public int StringCount; // Total number of strings ( falls out of sync with NextStringID if some strings are removed in development ).
    [HideInInspector] public int NextStringID; // The next available string ID. Incremented each time a new string is added.

    [HideInInspector] public string[] StringsWithInlineTags; // Strings that contain tags to replace ( i.e. "Stop right there, {PlayerName}!" )
    [HideInInspector] public int[] StringIDs; // The ID of each string. This array is parallel to StringsWithInlineTags.
    [HideInInspector] public string[] StringNames; // The unique name of each string. Parallel to StringsWithInlineTags.
    [HideInInspector] public ulong[] StringVersions; // The version number of each string. Incremented each time the string is updated.

    public string[] StringsIndexedByID; // Contains the same strings as StringsWithInlineTags, with each string at an index equal to its ID.

    public TagIDAuthoringDatabase Tags;

    public void AddString(string str)
    {
        string[] newStringArray = new string[StringCount + 1];
        int[] newIntArray = new int[StringCount + 1];

        StringsWithInlineTags.CopyTo(newStringArray, 0);
        StringIDs.CopyTo(newIntArray, 0);

        StringsWithInlineTags = newStringArray;
        StringIDs = newIntArray;

        StringsWithInlineTags[StringCount] = str;
        StringIDs[StringCount] = NextStringID;

        ++StringCount;
        ++NextStringID;
    }

    public void AddStrings(string[] strings, int count)
    {
        string[] newStringArray = new string[StringCount + count];
        int[] newIntArray = new int[StringCount + count];

        StringsWithInlineTags.CopyTo(newStringArray, 0);
        StringIDs.CopyTo(newIntArray, 0);

        StringsWithInlineTags = newStringArray;
        StringIDs = newIntArray;

        for (int i = 0; i < count; i++)
        {
            StringsWithInlineTags[StringCount] = strings[i];
            StringIDs[StringCount] = NextStringID;
            ++StringCount;
            ++NextStringID;
        }
    }

    public void IndexStrings()
    {
        StringsIndexedByID = new string[NextStringID];
        for (int i = 0; i < StringCount; i++)
        {
            int index = StringIDs[i];
            string str = StringsWithInlineTags[i];
            StringsIndexedByID[index] = str;
        }
    }

    public void StringsToSubstrings<T>(T parser) where T : struct, IStringToSubstringsParser
    {
        int length = NextStringID;
        AuthoringString[] decomposedStrings = new AuthoringString[length];

        for (int i = 0; i < length; i++)
        {
            decomposedStrings[i] = parser.GetSubstringFromString(StringsIndexedByID[i]);
        }
    }

    // TODO: localization
    public void AddOrModifyStrings<T>(string[] strings, string[] stringNames, ulong[] versions)
    {
        int numNewStrings = strings.Length;

        SimpleList<int> actualNewStrings = new SimpleList<int>(numNewStrings);
        SimpleList<int> potentialUpdatedStrings = new SimpleList<int>(numNewStrings);
        HashSet<string> stringSet = new HashSet<string>(StringNames);

        for (int i = 0; i < numNewStrings; i++)
        {
            if (stringSet.Add(stringNames[i]))
            {
                actualNewStrings.AddUnchecked(i);
            }
            else
            {
                potentialUpdatedStrings.AddUnchecked(i);
            }
        }

        Dictionary<string, int> nameToIndexMap = new Dictionary<string, int>(StringNames.Length * 4);
        for (int i = 0; i < StringNames.Length; i++)
        {
            nameToIndexMap.Add(StringNames[i], i);
        }

        int potentialUpdatedCount = potentialUpdatedStrings.Count;
        int[] potentialUpdatedArray = potentialUpdatedStrings.Array;
        for (int i = 0; i < potentialUpdatedCount; i++)
        {
            int index = potentialUpdatedArray[i];
            ulong version = versions[index];

            int indexOfString = nameToIndexMap[stringNames[index]];
            ulong currentVersion = StringVersions[indexOfString];
            StringVersions[indexOfString] = version;

            if (version != currentVersion)
            {
                StringsWithInlineTags[indexOfString] = strings[index];
            }
        }

        int newStringCount = actualNewStrings.Count;
        int[] newStringIndexArray = actualNewStrings.Array;

        int currLength = StringNames.Length;

        string[] newStringsArray = new string[currLength + newStringCount];
        string[] newNamesArray = new string[currLength + newStringCount];
        int[] newIDsArray = new int[currLength + newStringCount];
        ulong[] newVersionsArray = new ulong[currLength + newStringCount];

        System.Array.Copy(StringsWithInlineTags, 0, newStringsArray, 0, currLength);
        System.Array.Copy(StringNames, 0, newNamesArray, 0, currLength);
        System.Array.Copy(StringIDs, 0, newIDsArray, 0, currLength);
        System.Array.Copy(StringVersions, 0, newVersionsArray, 0, currLength);

        StringsWithInlineTags = newStringsArray;
        StringNames = newNamesArray;
        StringIDs = newIDsArray;
        StringVersions = newVersionsArray;

        string[] newStringsIndexedByID = new string[StringsIndexedByID.Length + newStringCount];
        System.Array.Copy(StringsIndexedByID, 0, newStringsIndexedByID, 0, StringsIndexedByID.Length);
        StringsIndexedByID = newStringsIndexedByID;

        int nextStringId = NextStringID;
        for (int i = 0; i < newStringCount; i++, nextStringId++)
        {
            int index = newStringIndexArray[i];
            string name = stringNames[index];
            string str = strings[index];

            newStringsArray[currLength + i] = str;
            newNamesArray[currLength + i] = name;
            newIDsArray[currLength + i] = nextStringId;
        }

        NextStringID = nextStringId;

        StringCount = currLength + newStringCount;
    }

    public int IndexOfTag(string str, int startIndex)
    {
        return 0;
    }
}

public interface IStringToSubstringsParser
{
    AuthoringString GetSubstringFromString(string str);
}

public interface IGetStringsFromFiles
{

}

[System.Serializable]
public struct AuthoringTagEntry : System.IEquatable<AuthoringTagEntry>
{
    public string TagString; // The original string of the tag
    public int TypeID; // What kind of tag is it? Defined by the user, so different types of tags can be resolved to values in different ways.
    public int ID; // The actual ID of the tag.

    public bool Equals(AuthoringTagEntry other)
    {
        return TypeID == other.TypeID && ID == other.ID && TagString.Equals(other.TagString);
    }
}

[System.Serializable]
public struct AuthoringString
{
    public string StringName; // Unique name associated with this string.
    public string OriginalString; // The original string.
    public string[] SubstringLiterals; // All substrings that aren't tags, in read order.
    public string[] Tags; // All tags contained in the original string, in read order.
    public bool StartsWithTag; // Whether or not the string starts with a tag.

    public int IDForRuntime; // The runtime index of this string.
    public ulong Version; // The version of this string.
}

[System.Serializable]
public struct TagIDAuthoringDatabase
{
    public AuthoringTagEntry[] tags;
    public int NextTagID;
}

public struct SimpleString
{
    public char[] chars;
    public int length;
}

public struct RuntimeTagStringStorage
{
    public char[] chars;
    public int index; // Offset into chars where the tag string begins
    public int count; // How many chars are in the tag string
}

public struct Index2D
{
    public int Index0;
    public int Index1;
}

public struct RuntimeStringData
{
    public int substringsIndex; // The index of this string's literal substrings in the packed substring array
    public int metadataInfo; // Contains the metadata index and a bit designating if the string begins with a tag
    public int tagCount; // Also the count of literalsMetaData
    public int substringCount; // How many literal substrings this string contains
}

public struct RuntimeStringDatabase
{
    public RuntimeStringData[] stringLookupTable;
    public int[] stringMetaData;
    public char[] substringLiterals;
}

public struct RuntimeTagDatabase
{
    public RuntimeTagStringStorage[] tagLookup;
}

public struct StringUtil
{
    ///<summary>
    /// The format of the ReadOnlyStrings array works like this:
    /// The first N * 2 chars correspond to all N IDs of ReadOnly strings in the game.
    /// They index into a set of 32 bit ints, the first of which contains the number of ints and a special bit flag.
    /// The following ints alternate between being the index and length of a ReadOnly subsection of the string,
    /// and being the primary and secondary index into an array-of-arrays of char arrays, corresponding to a variable subsection of the string.
    /// The special bit determines whether the string begins with a variable or a ReadOnly subsection.
    /// </summary>

    // Note: this all assumes all chars in the game can fit into one array. C# has limitations.
    public static void GetString(int stringID, char[] readOnlyStrings, SimpleString[][] variableDatabase, char[] outString, out int outStringLength)
    {
        int headerIndex = GetIntFromChars(readOnlyStrings, stringID);
        int headerSize = GetIntFromChars(readOnlyStrings, headerIndex);
        bool startsWithToken = (headerSize & (1 << 31)) != 0;
        headerSize &= int.MaxValue; // mask off final bit which is used as our "starts with token" bool

        int writeBufferIndex = 0;

        if (startsWithToken)
        {
            int variableTypeID = GetIntFromChars(readOnlyStrings, headerIndex + 1);
            int variableID = GetIntFromChars(readOnlyStrings, headerIndex + 2);
            headerIndex += 2;
            headerSize -= 2;

            SimpleString variableString = variableDatabase[variableTypeID][variableID];
            int variableLength = variableString.length;
            char[] variableChars = variableString.chars;

            for (int i = 0; i < variableLength; i++, writeBufferIndex++)
            {
                outString[i] = variableChars[i];
            }
        }

        for (int i = 0; i < headerSize; i += 4)
        {
            int stringIndex = GetIntFromChars(readOnlyStrings, headerIndex + i);
            int stringLength = GetIntFromChars(readOnlyStrings, headerIndex + i + 1);
            int variableTypeID = GetIntFromChars(readOnlyStrings, headerIndex + i + 2);
            int variableID = GetIntFromChars(readOnlyStrings, headerIndex + i + 3);

            for (int j = 0; j < stringLength; j++, writeBufferIndex++)
            {
                outString[writeBufferIndex] = readOnlyStrings[stringIndex + j];
            }

            SimpleString variableString = variableDatabase[variableTypeID][variableID];
            int variableLength = variableString.length;
            char[] variableChars = variableString.chars;

            for (int j = 0; j < variableLength; j++, writeBufferIndex++)
            {
                outString[writeBufferIndex] = variableChars[j];
            }
        }

        bool endsWithReadOnlyString = (headerSize & 2) != 0;

        if (endsWithReadOnlyString)
        {
            int stringIndex = GetIntFromChars(readOnlyStrings, headerIndex + headerSize - 2);
            int stringLength = GetIntFromChars(readOnlyStrings, headerIndex + headerSize - 1);

            for (int j = 0; j < stringLength; j++, writeBufferIndex++)
            {
                outString[writeBufferIndex] = readOnlyStrings[stringIndex + j];
            }
        }

        outStringLength = writeBufferIndex;
    }

    public static int GetString(int stringID, in RuntimeStringDatabase stringDatabase, in RuntimeTagDatabase tagDatabase, char[] outBuffer)
    {
        ref readonly RuntimeStringData stringData = ref stringDatabase.stringLookupTable[stringID];

        int length = 0;


        return length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetIntFromChars(char[] chars, int intIndex)
    {
        int realIndex = intIndex * 2;
        char char0 = chars[realIndex];
        char char1 = chars[realIndex + 1];

        return (char0 << 16) | char1;
    }

    public static void CreateDatabase(AuthoringString[] allStrings, TagIDAuthoringDatabase variableDatabase)
    {
        Dictionary<string, int> sortedStrings = new Dictionary<string, int>();

        for (int i = 0; i < variableDatabase.tags.Length; i++)
        {
            sortedStrings.Add(variableDatabase.tags[i].TagString, i);
        }

        for (int i = 0; i < allStrings.Length; i++)
        {
            AuthoringString stringDecomposition = allStrings[i];

            int charsCount = 0;
            for (int j = 0; j < stringDecomposition.SubstringLiterals.Length; j++)
            {
                charsCount += stringDecomposition.SubstringLiterals[i].Length;
            }

            int stringInfoIntCount = stringDecomposition.SubstringLiterals.Length + stringDecomposition.Tags.Length + 1;
            char[] stringInfo = new char[2 * stringInfoIntCount + charsCount];
            bool startsWithVariable = stringDecomposition.StartsWithTag;
            int headerInfo = stringInfoIntCount | ((startsWithVariable ? 1 : 0) << 31);

            stringInfo[0] = (char)(headerInfo & short.MaxValue);
            stringInfo[1] = (char)((headerInfo >> 16) & short.MaxValue);

            stringInfoIntCount--;

            int stringOffset = 0;

            // when I come back to this code to finish: this loop ain't right
            for (int j = 1; j < stringInfoIntCount; j++)
            {
                string substring = stringDecomposition.SubstringLiterals[j];
                int substringLength = substring.Length;
                int stringIndex = stringOffset;

                stringOffset += substringLength;
            }
        }
    }
}