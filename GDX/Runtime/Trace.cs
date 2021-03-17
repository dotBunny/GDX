// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using UnityEngine.Diagnostics;
using Debug = System.Diagnostics.Debug;
using Object = UnityEngine.Object;

namespace GDX
{
    /// <summary>
    ///     Trace logging functionality.
    /// </summary>
    public static class Trace
    {
        /// <summary>
        /// Trace Levels
        /// </summary>
        [Flags]
        public enum TraceLevel : ushort
        {
            /// <summary>
            ///     A trivial informational entry.
            /// </summary>
            Info = 0,

            /// <summary>
            ///     An entry indicating something which might be useful to provide context.
            /// </summary>
            Log = 1,

            /// <summary>
            ///     An issue has been found but handled.
            /// </summary>
            Warning = 2,

            /// <summary>
            ///     An error has occurred which may be recoverable, but notification is required.
            /// </summary>
            Error = 4,

            /// <summary>
            ///     An exception has occured and needs to be flagged up for resolution. The should never happen in a release
            ///     environment.
            /// </summary>
            Exception = 8,

            /// <summary>
            ///     An assertion based event has occured and has some sort of messaging to be recorded.
            /// </summary>
            Assertion = 16,

            /// <summary>
            ///     A fatal error has occured which needs to be logged, and the program will subsequently crash.
            /// </summary>
            Fatal = 32
        }

        /// <summary>
        ///     Log a trace message to the appropriate subscribers and the Unity console where applicable.
        /// </summary>
        /// <param name="level">The <see cref="TraceLevel" /> of the particular message.</param>
        /// <param name="traceObject">An <see cref="object" /> representing the message to be recorded.</param>
        /// <param name="contextObject">An <see cref="UnityEngine.Object" /> indicating context for the given message.</param>
        public static void Output(TraceLevel level, object traceObject, Object contextObject = null)
        {

            // Get a reference to the config
            // TODO: Would it be better to cache this?
            GDXConfig config = GDXConfig.Get();

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (!config.traceDevelopmentLevels.HasFlags(level))
            {
                return;
            }
#elif DEBUG
            if (!config.traceDebugLevels.HasFlags(level))
            {
                return;
            }
#else
            if (!config.traceReleaseLevels.HasFlags(level))
            {
                return;
            }
#endif

            // This will output to anything internally registered for tracing (IDE consoles for example)
            Debug.WriteLine(traceObject);

            // Is outputting to the Unity console enabled?
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (!config.traceDevelopmentOutputToUnityConsole)
            {
                return;
            }
#elif DEBUG
            if (!config.traceDebugOutputToUnityConsole)
            {
                return;
            }
#endif

            // Figure out what path to take based on the level
            switch (level)
            {
                case TraceLevel.Assertion:
                    UnityEngine.Debug.LogAssertion(traceObject, contextObject);
                    break;

                case TraceLevel.Warning:
                    UnityEngine.Debug.LogWarning(traceObject, contextObject);
                    break;

                case TraceLevel.Error:
                    UnityEngine.Debug.LogError(traceObject, contextObject);
                    break;

                case TraceLevel.Exception:
                    UnityEngine.Debug.LogException((Exception)traceObject, contextObject);
                    break;

                case TraceLevel.Fatal:
                    UnityEngine.Debug.LogError(traceObject, contextObject);
                    Utils.ForceCrash(ForcedCrashCategory.FatalError);
                    break;

                // ReSharper disable RedundantCaseLabel
                case TraceLevel.Info:
                case TraceLevel.Log:
                default:
                    UnityEngine.Debug.Log(traceObject, contextObject);
                    break;
                // ReSharper restore RedundantCaseLabel
            }
        }
    }
}