// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEditor;

namespace GDX.Editor
{
    public static class EditorTaskDirector
    {
        // TODO: maybe we have a gdx config for this too?
        // TODO: Disable when we go into playmode?
        // TODO: what does playmode ticking look like?

        static double s_NextUpdate = 0f;
        static double s_TickRate = 0.5f; // TODO: Make GDX config?

        static int s_ProgressID = -1;

        [InitializeOnLoadMethod]
        static void Initialize()
        {
            // Always occurs on the main thread
            EditorApplication.update += EditorUpdate;
        }

        static void EditorUpdate()
        {
            if (EditorApplication.timeSinceStartup < s_NextUpdate)
            {
                return;
            }
            s_NextUpdate = EditorApplication.timeSinceStartup + s_TickRate;
            Threading.TaskDirector.Tick();

            // Update background worker
            int busyCount = Threading.TaskDirector.GetBusyCount();
            int waitingCount = Threading.TaskDirector.GetBusyCount();

            if (busyCount > 0 || waitingCount > 0)
            {
                // Create our progress indicator
                if (s_ProgressID == -1)
                {
                    s_ProgressID = Progress.Start("GDX Task Director", "Executing Tasks ...");
                }

                Progress.SetDescription(s_ProgressID, $"{busyCount} Working / {waitingCount} Waiting");
            }
            else if (s_ProgressID != -1)
            {
                Progress.Finish(s_ProgressID);
                s_ProgressID = -1;
            }
        }
    }
}