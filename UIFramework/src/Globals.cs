using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;

namespace UIFramework
{
    public class Globals
    {
        /// <summary>
        /// The executable folder path.
        /// </summary>
        public static string ExecutableDir {
            get { return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location); }
        }
    }
}
