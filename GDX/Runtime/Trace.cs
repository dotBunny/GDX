// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Diagnostics;
using Object = UnityEngine.Object;

namespace GDX
{
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

            Assertion = 50,

            /// <summary>
            ///     A fatal error has occured which needs to be logged, and the program will subsequently crash.
            /// </summary>
            Fatal = 100
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Assertion(object message, Object context = null)
        {
            Output(TraceLevel.Assertion, message, context);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Error(object message, Object context = null)
        {
            Output(TraceLevel.Error, message, context);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Exception(Exception ex, Object context = null)
        {
            Output(TraceLevel.Exception, ex, context);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Fatal(object message, Object context = null)
        {
            Output(TraceLevel.Fatal, message, context);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Info(object message, Object context = null)
        {
            Output(TraceLevel.Info, message, context);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Log(object message, Object context = null)
        {
            Output(TraceLevel.Log, message, context);
        }

        /// <summary>
        /// </summary>
        /// <param name="level"></param>
        /// <param name="traceObject"></param>
        /// <param name="contextObject"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Output(TraceLevel level, object traceObject, Object contextObject = null)
        {
            // Get a reference to the config
            // TODO: Would it be better to cache this?
            GDXConfig config = GDXConfig.Get();

#if UNITY_EDITOR
            if (level < config.traceEditorMinimumLevel)
            {
                return;
            }
#else
            if (level < config.traceBuildMinimumLevel)
            {
                return;
            }
#endif


            // This will output to anything internally registered for tracing (IDE consoles for example)
            Debug.WriteLine(traceObject);

            // Is outputting to the Unity console enabled?
            if (!config.traceOutputToUnityConsole)
            {
                return;
            }

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

        public static void OutputFormat(TraceLevel level, Object contextObject, string format, params object[] args)
        {
            Output(level, string.Format(format, args), contextObject);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Warning(object message, Object context = null)
        {
            Output(TraceLevel.Warning, message, context);
        }
    }
}