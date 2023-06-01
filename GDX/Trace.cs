// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using Object = UnityEngine.Object;

namespace GDX
{
    /// <summary>
    ///     Trace logging functionality.
    /// </summary>
    [VisualScriptingCompatible(8)]
    public static class Trace
    {
        // TODO: Create trace levels output methods which can be compiled out?

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
        ///     Log a trace message to the appropriate subscribers and the Unity console where applicable with additional context on invoke location.
        /// </summary>
        /// <param name="level">The <see cref="TraceLevel" /> of the particular message.</param>
        /// <param name="traceObject">An <see cref="object" /> representing the message to be recorded.</param>
        /// <param name="contextObject">An <see cref="UnityEngine.Object" /> indicating context for the given message.</param>
        /// <param name="memberName">Automatically filled out member name which invoked this method.</param>
        /// <param name="sourceFilePath">Automatically filled out source code path of the invoking method.</param>
        /// <param name="sourceLineNumber">Automatically filled out line number of the invoking method.</param>
        public static void Output(TraceLevel level, object traceObject,
            Object contextObject = null,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (!Config.TraceDevelopmentLevels.HasFlags(level))
            {
                return;
            }
#elif DEBUG
            if (!Config.traceDebugLevels.HasFlags(level))
            {
                return;
            }
#else
            if (!Config.TraceReleaseLevels.HasFlags(level))
            {
                return;
            }
#endif // UNITY_EDITOR || DEVELOPMENT_BUILD

            // Build output content
            if (traceObject is Exception traceException)
            {
                Console.WriteLine(
                    $"{traceException.Message}\n\t@ {sourceFilePath}:{sourceLineNumber.ToString()}\n\t[EXCEPTION] Stacktrace:\n{traceException.StackTrace}\n");
            }
            else
            {
                Console.WriteLine($"{traceObject}\n\t@ {sourceFilePath}:{sourceLineNumber.ToString()}");
            }

            // Is outputting to the Unity console enabled?
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (!Config.TraceDevelopmentOutputToUnityConsole)
            {
                return;
            }
#elif DEBUG
            if (!Config.traceDebugOutputToUnityConsole)
            {
                return;
            }
#endif // UNITY_EDITOR || DEVELOPMENT_BUILD

#if UNITY_EDITOR || DEVELOPMENT_BUILD || DEBUG
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
                    UnityEngine.Diagnostics.Utils.ForceCrash(UnityEngine.Diagnostics.ForcedCrashCategory.FatalError);
                    break;

                case TraceLevel.Info:
                case TraceLevel.Log:
                default:
                    UnityEngine.Debug.Log(traceObject, contextObject);
                    break;

            }
#endif // UNITY_EDITOR || DEVELOPMENT_BUILD || DEBUG
        }
    }
}
