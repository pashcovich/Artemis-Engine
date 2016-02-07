#region Using Statements

using System;
using System.Linq;

#endregion

namespace Artemis.Engine.Utilities.UriTree
{
    public static class UriUtilities
    {

        /// <summary>
        /// The character that is used to separate individual parts in a Uri.
        /// </summary>
        public const char URI_SEPARATOR = '.';

        /// <summary>
        /// Get the separate parts of a Uri name.
        /// 
        /// Example: GetParts("a.b.c") returns {"a", "b", "c"}
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string[] GetParts(string name)
        {
            return name.Split(URI_SEPARATOR).ToArray();
        }

        /// <summary>
        /// Get the last part of a Uri name.
        /// 
        /// Example: GetLastPart("a.b.c") returns "c"
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetLastPart(string name)
        {
            return name.Split(URI_SEPARATOR).Last();
        }

        /// <summary>
        /// Get the first part of a Uri name.
        /// 
        /// Example: GetFirstPart("a.b.c") returns "a"
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetFirstPart(string name)
        {
            return new String(name.TakeWhile(c => c != URI_SEPARATOR).ToArray());
        }

        /// <summary>
        /// Return everything in a Uri except for the first part of the name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string AllButFirstPart(string name)
        {
            return new String(name.SkipWhile(c => c != URI_SEPARATOR).Skip(1).ToArray());
        }

        /// <summary>
        /// Return everything in a Uri except for the last part of the name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string AllButLastPart(string name)
        {
            return name.Substring(0, name.LastIndexOf(URI_SEPARATOR));
        }

    }
}
