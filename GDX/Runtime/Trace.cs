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
        public enum TraceLevel
        {
            /// <summary>
            ///     A trivial informational entry.
            /// </summary>
            Info = 0,

            /// <summary>
            ///     An entry indicating something which might be useful to provide context.
            /// </summary>
            Log = 10,

            /// <summary>
            ///     An issue has been found but handled.
            /// </summary>
            Warning = 20,

            /// <summary>
            ///     An error has occurred which may be recoverable, but notification is required.
            /// </summary>
            Error = 30,

            /// <summary>
            ///     An exception has occured and needs to be flagged up for resolution. The should never happen in a release
            ///     environment.
            /// </summary>
            Exception = 40,

            /// <summary>
            ///     An assertion based event has occured and has some sort of messaging to be recorded.
            /// </summary>
            Assertion = 50,

            /// <summary>
            ///     A fatal error has occured which needs to be logged, and the program will subsequently crash.
            /// </summary>
            Fatal = 100
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
            if (level < config.traceDevelopmentMinimumLevel)
            {
                return;
            }
#elif DEBUG
            if (level < config.traceDebugMinimumLevel)
            {
                return;
            }
#else
            if (level < config.traceReleaseMinimumLevel)
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