using DevTools.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevTools.MVVM.Model.DTO
{
    public class BusinessCube
    {
        /// <summary>
        /// Versione di Business da usare
        /// </summary>
        public BusinessVersion Version { get; set; }
        /// <summary>
        /// Cartella in cui è presente business
        /// </summary>
        public string? Folder { get; set; }
    }
}
