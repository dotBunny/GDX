// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace GDX.Developer
{
    public static class ManagedUpdateSystem
    {
        const int k_PreAllocatedSystems = 10;
        static SimpleList<IManagedInitialization> s_ManagedInitialization =
            new SimpleList<IManagedInitialization>(k_PreAllocatedSystems);
        static int s_ManagedInitializationCount;
        static SimpleList<IManagedUpdate> s_ManagedUpdate = new SimpleList<IManagedUpdate>(k_PreAllocatedSystems);
        static int s_ManagedUpdateCount;
        static SimpleList<IManagedPreUpdate> s_ManagedPreUpdate = new SimpleList<IManagedPreUpdate>(k_PreAllocatedSystems);
        static int s_ManagedPreUpdateCount;
        static SimpleList<IManagedFixedUpdate> s_ManagedFixedUpdate =
            new SimpleList<IManagedFixedUpdate>(k_PreAllocatedSystems);
        static int s_ManagedFixedUpdateCount;
        static SimpleList<IManagedEarlyUpdate> s_ManagedEarlyUpdate =
            new SimpleList<IManagedEarlyUpdate>(k_PreAllocatedSystems);
        static int s_ManagedEarlyUpdateCount;
        static SimpleList<IManagedPreLateUpdate> s_ManagedPreLateUpdate =
            new SimpleList<IManagedPreLateUpdate>(k_PreAllocatedSystems);
        static int s_ManagedPreLateUpdateCount;
        static SimpleList<IManagedPostLateUpdate> s_ManagedPostLateUpdate =
            new SimpleList<IManagedPostLateUpdate>(k_PreAllocatedSystems);
        static int s_ManagedPostLateUpdateCount;

        static bool s_AddedToPlayerLoop;

        public static void RegisterManagedInitialization(IManagedInitialization managedInitialization)
        {
            s_ManagedInitialization.AddWithExpandCheck(managedInitialization);
            s_ManagedInitializationCount = s_ManagedInitialization.Count;
        }
        public static void RemoveManagedInitialization(IManagedInitialization managedInitialization)
        {
            s_ManagedInitialization.RemoveFirstItem(managedInitialization);
            s_ManagedInitializationCount = s_ManagedInitialization.Count;
        }
        public static void RegisterManagedUpdate(IManagedUpdate managedUpdate)
        {
            s_ManagedUpdate.AddWithExpandCheck(managedUpdate);
            s_ManagedUpdateCount = s_ManagedUpdate.Count;
        }

        public static void UnregisterManagedUpdate(IManagedUpdate managedUpdate)
        {
            s_ManagedUpdate.RemoveFirstItem(managedUpdate);
            s_ManagedUpdateCount = s_ManagedUpdate.Count;
        }

        public static void RegisterManagedPreUpdate(IManagedPreUpdate managedPreUpdate)
        {
            s_ManagedPreUpdate.AddWithExpandCheck(managedPreUpdate);
            s_ManagedPreUpdateCount = s_ManagedPreUpdate.Count;
        }

        public static void UnregisterManagedPreUpdate(IManagedPreUpdate managedPreUpdate)
        {
            s_ManagedPreUpdate.RemoveFirstItem(managedPreUpdate);
            s_ManagedPreUpdateCount = s_ManagedPreUpdate.Count;
        }

        static void Initialization()
        {
            float deltaTime = Time.deltaTime;
            float unscaledDeltaTime = Time.unscaledDeltaTime;
            for (int i = 0; i < s_ManagedInitializationCount; i++)
            {
                s_ManagedInitialization.Array[i].ManagedInitialization(deltaTime, unscaledDeltaTime);
            }
        }

        static void Update()
        {
            float deltaTime = Time.deltaTime;
            float unscaledDeltaTime = Time.unscaledDeltaTime;
            for (int i = 0; i < s_ManagedUpdateCount; i++)
            {
                s_ManagedUpdate.Array[i].ManagedUpdate(deltaTime, unscaledDeltaTime);
            }
        }

        static void PreUpdate()
        {
            float deltaTime = Time.deltaTime;
            float unscaledDeltaTime = Time.unscaledDeltaTime;
            for (int i = 0; i < s_ManagedPreUpdateCount; i++)
            {
                s_ManagedPreUpdate.Array[i].ManagedPreUpdate(deltaTime, unscaledDeltaTime);
            }
        }

        public static void RegisterManagedFixedUpdate(IManagedFixedUpdate managedFixedUpdate)
        {
            s_ManagedFixedUpdate.AddWithExpandCheck(managedFixedUpdate);
            s_ManagedFixedUpdateCount = s_ManagedFixedUpdate.Count;
        }

        public static void UnregisterManagedFixedUpdate(IManagedFixedUpdate managedFixedUpdate)
        {
            s_ManagedFixedUpdate.RemoveFirstItem(managedFixedUpdate);
            s_ManagedFixedUpdateCount = s_ManagedFixedUpdate.Count;
        }

        static void FixedUpdate()
        {
            float deltaTime = Time.deltaTime;
            float unscaledDeltaTime = Time.unscaledDeltaTime;
            for (int i = 0; i < s_ManagedFixedUpdateCount; i++)
            {
                s_ManagedFixedUpdate.Array[i].ManagedFixedUpdate(deltaTime, unscaledDeltaTime);
            }
        }

        public static void RegisterManagedEarlyUpdate(IManagedEarlyUpdate managedEarlyUpdate)
        {
            s_ManagedEarlyUpdate.AddWithExpandCheck(managedEarlyUpdate);
            s_ManagedEarlyUpdateCount = s_ManagedEarlyUpdate.Count;
        }

        public static void UnregisterManagedEarlyUpdate(IManagedEarlyUpdate managedEarlyUpdate)
        {
            s_ManagedEarlyUpdate.RemoveFirstItem(managedEarlyUpdate);
            s_ManagedEarlyUpdateCount = s_ManagedEarlyUpdate.Count;
        }

        static void EarlyUpdate()
        {
            float deltaTime = Time.deltaTime;
            float unscaledDeltaTime = Time.unscaledDeltaTime;
            for (int i = 0; i < s_ManagedEarlyUpdateCount; i++)
            {
                s_ManagedEarlyUpdate.Array[i].ManagedEarlyUpdate(deltaTime, unscaledDeltaTime);
            }
        }

        public static void RegisterManagedPreLateUpdate(IManagedPreLateUpdate managedPreLateUpdate)
        {
            s_ManagedPreLateUpdate.AddWithExpandCheck(managedPreLateUpdate);
            s_ManagedPreLateUpdateCount = s_ManagedPreLateUpdate.Count;
        }

        public static void UnregisterManagedPreLateUpdate(IManagedPreLateUpdate managedPreLateUpdate)
        {
            s_ManagedPreLateUpdate.RemoveFirstItem(managedPreLateUpdate);
            s_ManagedPreLateUpdateCount = s_ManagedPreLateUpdate.Count;
        }

        static void PreLateUpdate()
        {
            float deltaTime = Time.deltaTime;
            float unscaledDeltaTime = Time.unscaledDeltaTime;
            for (int i = 0; i < s_ManagedPreLateUpdateCount; i++)
            {
                s_ManagedPreLateUpdate.Array[i].ManagedPreLateUpdate(deltaTime, unscaledDeltaTime);
            }
        }

        public static void RegisterManagedPostLateUpdate(IManagedPostLateUpdate managedPostLateUpdate)
        {
            s_ManagedPostLateUpdate.AddWithExpandCheck(managedPostLateUpdate);
            s_ManagedPostLateUpdateCount = s_ManagedPostLateUpdate.Count;
        }

        public static void UnregisterManagedPostLateUpdate(IManagedPostLateUpdate managedPostLateUpdate)
        {
            s_ManagedPostLateUpdate.RemoveFirstItem(managedPostLateUpdate);
            s_ManagedPostLateUpdateCount = s_ManagedPostLateUpdate.Count;
        }

        static void PostLateUpdate()
        {
            float deltaTime = Time.deltaTime;
            float unscaledDeltaTime = Time.unscaledDeltaTime;
            for (int i = 0; i < s_ManagedPostLateUpdateCount; i++)
            {
                s_ManagedPostLateUpdate.Array[i].ManagedPostLateUpdate(deltaTime, unscaledDeltaTime);
            }
        }


        public static void AddToPlayerLoop()
        {
            PlayerLoopSystem systemRoot = PlayerLoop.GetCurrentPlayerLoop();

            systemRoot.AddSubSystemToFirstSubSystemOfType(
                typeof(Initialization),
                typeof(ManagedUpdateSystem), Initialization);
            systemRoot.AddSubSystemToFirstSubSystemOfType(
                typeof(EarlyUpdate),
                typeof(ManagedUpdateSystem), EarlyUpdate);
            systemRoot.AddSubSystemToFirstSubSystemOfType(
                typeof(FixedUpdate),
                typeof(ManagedUpdateSystem), FixedUpdate);

            systemRoot.AddSubSystemToFirstSubSystemOfType(
                typeof(PreUpdate),
                typeof(ManagedUpdateSystem), PreUpdate);

            systemRoot.AddSubSystemToFirstSubSystemOfType(
                typeof(Update),
                typeof(ManagedUpdateSystem), Update);

            systemRoot.AddSubSystemToFirstSubSystemOfType(
                typeof(PreLateUpdate),
                typeof(ManagedUpdateSystem), PreLateUpdate);

            systemRoot.AddSubSystemToFirstSubSystemOfType(
                typeof(PostLateUpdate),
                typeof(ManagedUpdateSystem), PostLateUpdate);

            PlayerLoop.SetPlayerLoop(systemRoot);
            s_AddedToPlayerLoop = true;
        }

        public static void RemoveFromPlayerLoop()
        {
            if (!s_AddedToPlayerLoop) return;

            PlayerLoopSystem systemRoot = PlayerLoop.GetCurrentPlayerLoop();

            systemRoot.RemoveSubSystemsOfTypeFromFirstSubSystemOfType(typeof(Initialization),
                typeof(ManagedUpdateSystem));

            systemRoot.RemoveSubSystemsOfTypeFromFirstSubSystemOfType(typeof(EarlyUpdate),
                typeof(EarlyUpdate));

            systemRoot.RemoveSubSystemsOfTypeFromFirstSubSystemOfType(typeof(FixedUpdate),
                typeof(FixedUpdate));

            systemRoot.RemoveSubSystemsOfTypeFromFirstSubSystemOfType(typeof(PreUpdate),
                typeof(PreUpdate));

            systemRoot.RemoveSubSystemsOfTypeFromFirstSubSystemOfType(typeof(Update),
                typeof(Update));

            systemRoot.RemoveSubSystemsOfTypeFromFirstSubSystemOfType(typeof(PreLateUpdate),
                typeof(PreLateUpdate));

            systemRoot.RemoveSubSystemsOfTypeFromFirstSubSystemOfType(typeof(PostLateUpdate),
                typeof(PostLateUpdate));

            PlayerLoop.SetPlayerLoop(systemRoot);
            s_AddedToPlayerLoop = false;
        }

        public static bool IsRunning()
        {
            return s_AddedToPlayerLoop;
        }

        public interface IManagedInitialization
        {
            void ManagedInitialization(float deltaTime, float unscaledDeltaTime);
        }
        public interface IManagedUpdate
        {
            void ManagedUpdate(float deltaTime, float unscaledDeltaTime);
        }
        public interface IManagedPreUpdate
        {
            void ManagedPreUpdate(float deltaTime, float unscaledDeltaTime);
        }

        public interface IManagedFixedUpdate
        {
            void ManagedFixedUpdate(float deltaTime, float unscaledDeltaTime);
        }


        public interface IManagedEarlyUpdate
        {
            void ManagedEarlyUpdate(float deltaTime, float unscaledDeltaTime);
        }

        public interface IManagedPreLateUpdate
        {
            void ManagedPreLateUpdate(float deltaTime, float unscaledDeltaTime);
        }

        public interface IManagedPostLateUpdate
        {
            void ManagedPostLateUpdate(float deltaTime, float unscaledDeltaTime);
        }
    }
}