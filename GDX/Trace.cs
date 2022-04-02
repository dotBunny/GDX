// Copyright (c) 2020-2022 dotBunny Inc.
// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using Debug = System.Diagnostics.Debug;
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
            if (!GDXConfig.TraceDevelopmentLevels.HasFlags(level))
            {
                return;
            }
#elif DEBUG
            if (!GDXConfig.traceDebugLevels.HasFlags(level))
            {
                return;
            }
#else
            if (!GDXConfig.TraceReleaseLevels.HasFlags(level))
            {
                return;
            }
#endif

            // Build output content
            string outputContent;
            if (traceObject is Exception traceException)
            {
                outputContent = $"{traceException.Message}\n{traceException.StackTrace}\n{memberName}\n{sourceFilePath}:{sourceLineNumber.ToString()}";
            }
            else
            {
                outputContent = $"{traceObject}\n{memberName}\n{sourceFilePath}:{sourceLineNumber.ToString()}";
            }

            // This will output to anything internally registered for tracing (IDE consoles for example)
            Debug.WriteLine(outputContent);

            // Is outputting to the Unity console enabled?
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (!GDXConfig.TraceDevelopmentOutputToUnityConsole)
            {
                return;
            }
#elif DEBUG
            if (!GDXConfig.traceDebugOutputToUnityConsole)
            {
                return;
            }
#else
            // NO RELEASE CONSOLE OUTPUT
            return;
#endif


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

                default:
                    UnityEngine.Debug.Log(traceObject, contextObject);
                    break;

            }
#endif
        }
    }
}
