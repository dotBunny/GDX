// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GDX.Editor
{
    public class AssetDatabaseReference
    {
        public readonly string Guid;
        public readonly Type Type;

        string m_AssetPath;
        string m_DiskPath;
        string m_FileExtension;
        string m_FileName;
        string m_FileNameWithoutExtension;

        WeakReference<Object> m_LoadedAsset;
        string m_Path;
        string m_PathWithoutExtension;
        string m_TypeString;

        public AssetDatabaseReference(string guid, Type type = null)
        {
            Type = type;
            Guid = guid;
        }

        public Object GetOrLoadAsset()
        {
            m_LoadedAsset ??= new WeakReference<Object>(AssetDatabase.LoadAssetAtPath(GetAssetPath(), Type));
            return m_LoadedAsset.TryGetTarget(out Object target) ? target : null;
        }

        public string GetFileExtension(bool forceUpdate = false)
        {
            if (m_FileExtension == null || forceUpdate)
            {
                m_FileExtension = Path.GetExtension(GetAssetPath(forceUpdate));
            }

            return m_FileExtension;
        }

        public string GetFileName(bool forceUpdate = false)
        {
            if (m_FileName == null || forceUpdate)
            {
                m_FileName = Path.GetFileName(GetAssetPath());
            }

            return m_FileName;
        }

        public string GetFileNameWithoutExtension(bool forceUpdate = false)
        {
            if (m_FileNameWithoutExtension == null || forceUpdate)
            {
                m_FileNameWithoutExtension = Path.GetFileNameWithoutExtension(GetFileName(forceUpdate));
            }

            return m_FileNameWithoutExtension;
        }

        public string GetDiskPath(bool forceUpdate = false)
        {
            if (m_DiskPath == null || forceUpdate)
            {
                m_DiskPath = Path.Combine(Application.dataPath.Substring(0, Application.dataPath.Length - 6),
                    GetAssetPath(forceUpdate));
            }

            return m_DiskPath;
        }

        public string GetPath(bool forceUpdate = false)
        {
            if (m_Path == null || forceUpdate)
            {
                m_Path = GetAssetPath(forceUpdate).Substring(7);
            }

            return m_Path;
        }

        public string GetPathWithoutExtension(bool forceUpdate = false)
        {
            if (m_PathWithoutExtension == null || forceUpdate)
            {
                string path = GetPath(forceUpdate);
                m_PathWithoutExtension = path.Substring(0, path.Length - GetFileExtension().Length);
            }

            return m_PathWithoutExtension;
        }

        public string GetAssetPath(bool forceUpdate = false)
        {
            if (m_AssetPath == null || forceUpdate)
            {
                m_AssetPath = AssetDatabase.GUIDToAssetPath(Guid);
            }

            return m_AssetPath;
        }

        public string GetTypeString(bool forceUpdate = false)
        {
            if (m_TypeString == null || forceUpdate)
            {
                m_TypeString = Type.ToString();
            }

            return m_TypeString;
        }
    }
}