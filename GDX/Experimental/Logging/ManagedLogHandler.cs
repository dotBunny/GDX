// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GDX.Experimental.Logging
{
    public class ManagedLogHandler : ILogHandler
    {
        ILogHandler m_PreviousLogHandler;
        public ManagedLogHandler()
        {
            Start();
        }

        ~ManagedLogHandler()
        {
            Stop();
        }

        public void Start()
        {
            // Cache previous for reset, but only if it isn't our logger
            if (Debug.unityLogger.logHandler.GetType() != typeof(ManagedLogHandler))
            {
                m_PreviousLogHandler = Debug.unityLogger.logHandler;
            }
            Debug.unityLogger.logHandler = this;
            ManagedLog.IsUnityLogHandler = true;
        }

        public void Stop()
        {
            if (m_PreviousLogHandler != null && Debug.unityLogger.logHandler == this)
            {
                Debug.unityLogger.logHandler = m_PreviousLogHandler;
                m_PreviousLogHandler = null;
                ManagedLog.IsUnityLogHandler = false;
            }
        }

        /// <inheritdoc />
        public void LogFormat(LogType logType, Object context, string format, params object[] args)
        {
            if (context == null)
            {
                switch (logType)
                {
                    case LogType.Log:
                        ManagedLog.Info(LogCategory.Unity, string.Format(format, args));
                        break;
                    case LogType.Error:
                        ManagedLog.Error(LogCategory.Unity, string.Format(format, args));
                        break;
                    case LogType.Assert:
                        ManagedLog.Assertion(LogCategory.Unity, string.Format(format, args));
                        break;
                    case LogType.Warning:
                        ManagedLog.Warning(LogCategory.Unity, string.Format(format, args));
                        break;
                }
            }
            else
            {
                switch (logType)
                {
                    case LogType.Log:
                        ManagedLog.InfoWithContext(LogCategory.Unity, string.Format(format, args), context);
                        break;
                    case LogType.Error:
                        ManagedLog.ErrorWithContext(LogCategory.Unity, string.Format(format, args), context);
                        break;
                    case LogType.Assert:
                        ManagedLog.AssertionWithContext(LogCategory.Unity, string.Format(format, args), context);
                        break;
                    case LogType.Warning:
                        ManagedLog.WarningWithContext(LogCategory.Unity, string.Format(format, args), context);
                        break;
                }
            }

#if UNITY_EDITOR
            m_PreviousLogHandler.LogFormat(logType, context, format, args);
#endif
        }

        /// <inheritdoc />
        public void LogException(Exception exception, Object context)
        {
            if (context == null)
            {
                ManagedLog.Exception(LogCategory.Unity, exception);
            }
            else
            {
                ManagedLog.ExceptionWithContext(LogCategory.Unity, exception, context);
            }

#if UNITY_EDITOR
            m_PreviousLogHandler.LogException(exception, context);
#endif
        }
    }
}