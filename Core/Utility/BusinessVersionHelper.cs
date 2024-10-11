using DevTools.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevTools.Core.Utility
{
    public class BusinessVersionHelper
    {
        public static BusinessVersion StringToEnum(string version)
        {
            if (System.Enum.TryParse<BusinessVersion>(version, out var businessVersion)) { return businessVersion; }
            throw new Exception($"Versione di Business {businessVersion} non valida.");
        }
    }
}
