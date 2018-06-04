/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers
 * for more information concerning the license and the contributors participating to this project.
 */

using System;
using Newtonsoft.Json.Linq;

namespace Microsoft.AspNetCore.Authentication.QQ
{
    /// <summary>
    /// Contains static methods that allow to extract user's information from a <see cref="JObject"/>
    /// instance retrieved from QQ after a successful authentication process.
    /// </summary>
    public static class QQHelper
    {
        /// <summary>
        /// Gets the user's nickname.
        /// </summary>
        public static string GetNickname(JObject user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return user.Value<string>("nickname");
        }

        /// <summary>
        /// Gets the user's QQ profile picture URL (30px*30px).
        /// </summary>
        public static string GetPicture(JObject user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return user.Value<string>("figureurl");
        }

        /// <summary>
        /// Gets the user's QQ profile picture URL (50px*50px).
        /// </summary>
        public static string GetPictureMedium(JObject user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return user.Value<string>("figureurl_1");
        }

        /// <summary>
        /// Gets the user's QQ profile picture URL (100px*100px).
        /// </summary>
        public static string GetPictureFull(JObject user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return user.Value<string>("figureurl_2");
        }

        /// <summary>
        /// Gets the user's QQ avatar URL (40px*40px).
        /// </summary>
        public static string GetAvatar(JObject user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return user.Value<string>("figureurl_qq_1");
        }

        /// <summary>
        /// Gets the user's QQ space avatar URL (100px*100px).
        /// </summary>
        public static string GetAvatarFull(JObject user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return user.Value<string>("figureurl_qq_2");
        }

        /// <summary>
        /// Gets the user's gender.
        /// </summary>
        public static string GetGender(JObject user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return user.Value<string>("gender");
        }
    }
}
