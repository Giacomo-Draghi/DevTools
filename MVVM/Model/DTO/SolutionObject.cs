using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevTools.MVVM.Model.DTO
{
    public class SolutionObject
    {
        /// <summary>
        /// Lista dei progetti
        /// </summary>
        public ICollection<string>? ProjectList { get; set; }
    }
}
