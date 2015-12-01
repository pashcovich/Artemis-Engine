using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Artemis.Engine.Utilities.Groups
{
    public static class UriUtilities
    {

        public const char URI_SEPARATOR = '.';

        /// <summary>
        /// Get the separate parts of a URI name.
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
        /// Get the last part of a URI name.
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
        /// Get the first part of a URI name.
        /// 
        /// Example: GetFirstPart("a.b.c") returns "a"
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetFirstPart(string name)
        {
            return new String(name.TakeWhile(c => c != URI_SEPARATOR).ToArray());
        }

    }
}
