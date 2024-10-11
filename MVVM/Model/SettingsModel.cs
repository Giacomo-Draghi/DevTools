using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevTools.MVVM.Model
{
    public class SettingsModel
    {
        /// <summary>
        /// Directory dove verranno messe tutte le solution create
        /// </summary>
        public string SolutionDir {  get; set; } = @"C:\DevSln";
        public string? VsPath {  get; set; }
    }
}
