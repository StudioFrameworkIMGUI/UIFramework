using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;

namespace UIFramework
{
    public class Globals
    {
        public static string ExecutableDir {
            get { return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location); }
        }
    }
}
