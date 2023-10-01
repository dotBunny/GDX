// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

namespace GDX.Developer
{
#if UNITY_2021_3_OR_NEWER
    public abstract class ConsoleCommandBase
    {
        public enum ConsoleCommandLevel
        {
            Anonymous = -1,
            User = 0,
            Superuser = 1,
            Developer = 2
        }

        /// <summary>
        ///     Executes the logic for the command.
        /// </summary>
        /// <returns>
        ///     Returns false it means this is blocking further progression through
        ///     the queue of outstanding commands.
        /// </returns>
        public abstract bool Evaluate(float deltaTime);

        /// <summary>
        ///     Returns the minimum access level required to execute a command.
        /// </summary>
        /// <remarks>Overrideable, but defaults to having user level access.</remarks>
        /// <returns>The required user access level to utilize a given command.</returns>
        public virtual ConsoleCommandLevel GetAccessLevel()
        {
            return ConsoleCommandLevel.User;
        }

        public abstract string GetKeyword();

        public virtual string GetHelpUsage()
        {
            return GetKeyword();
        }

        public abstract string GetHelpMessage();

        public virtual string GetArgumentAutoComplete(string hint, int offset)
        {
            return null;
        }

        public virtual bool IsEditorOnly()
        {
            return false;
        }

        /// <summary>
        ///     Gets the instance of the work to be added to the <see cref="Console" />'s command buffer.
        /// </summary>
        /// <remarks>
        ///     This work is processed later so in the case where context is needed, a new instance should be returned,
        ///     however if it is a pure functionality you can just return the existing instance, which is the default
        ///     non-overridden behaviour.
        /// </remarks>
        /// <param name="context">Unique information needed to configure a newly created instance.</param>
        /// <returns>A qualified instance of the class ready to be executed, null if something went wrong.</returns>
        public virtual ConsoleCommandBase GetInstance(string context)
        {
            return this;
        }
    }
#endif // UNITY_2021_3_OR_NEWER
}