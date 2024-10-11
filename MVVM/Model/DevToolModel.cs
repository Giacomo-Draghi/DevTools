using DevTools.Core.Utility;
using DevTools.Enum;
using DevTools.MVVM.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DevTools.MVVM.Model
{
    public class DevToolModel
    {
        public BusinessCube? BusinessCube { get; set; }
        public SolutionObject? SolutionObject { get; set; }
        public CfgConfiguration? CfgConfiguration { get; set; }
        public string? SolutionName { get; set; }
        /// <summary>
        /// Gesctire l'eventuale rimozione dei GDPR
        /// </summary>
        public bool DeleteGdpr { get; set; }
        /// <summary>
        /// Gestisce la predisposizione per usare il corretto fine Cfg nel debugger
        /// </summary>
        public bool SetCfgDebugger { get; set; }

        public DevToolModel()
        {
            BusinessCube = new BusinessCube();
            SolutionObject = new SolutionObject();
            CfgConfiguration = new CfgConfiguration();
        }
        /// <summary>
        /// Creazione di un oggetto DevToolModel
        /// </summary>
        /// <param name="version">Versione di Business da usare</param>
        /// <param name="folder">Cartella in cui è presente business</param>
        /// <param name="solutionsTxt">Lista dei progetti</param>
        /// <param name="arcprocName">Database delle procedure di Business</param>
        /// <param name="arcProcServerName">Server su cui si trova l'Arcproc</param>
        /// <param name="dittaName">Database Ditta di Business</param>
        /// <param name="dittaServerName">Server su cui si trova il DB della Ditta</param>
        /// <param name="cfgName">Nome da dare dal file cfg</param>
        /// <param name="setCfgDebugger">Gestisce la predisposizione per usare il corretto fine Cfg nel debugger</param>
        /// <param name="deleteGdpr">Gesctire l'eventuale rimozione dei GDPR</param>
        public DevToolModel(
            //BusinessCube
            string? version = null, string? folder = null, 
            //Solution Object
            string? solutionsTxt = null, 
            //CfgConfiguration
            string? arcprocName = null,
            string? arcProcServerName = null,
            string? dittaName = null,
            string? dittaServerName = null,
            string? cfgName =  null,
            //Other Params
            bool setCfgDebugger = false,
            bool deleteGdpr = false
            )
        {
            BusinessCube = new BusinessCube()
            {
                Folder = folder,
                Version = version != null ? BusinessVersionHelper.StringToEnum(version) : System.Enum.GetValues(typeof(BusinessVersion)).Cast<BusinessVersion>().First()
            };
            SolutionObject = new SolutionObject()
            {
                ProjectList = solutionsTxt?.Split(';').ToList()
            };
            CfgConfiguration = new CfgConfiguration()
            {
                ArcprocName = arcprocName,
                ArcProcServerName = arcProcServerName,
                DittaName = dittaName,
                CfgName = cfgName,
                DittaServerName = dittaServerName
            };
            DeleteGdpr = deleteGdpr;
            SetCfgDebugger = setCfgDebugger;
        }

        public bool isExpirience() { return this.BusinessCube!.Version.ToString().Contains("exp", StringComparison.InvariantCultureIgnoreCase);}
        public bool isExpirienceRTM() { return this.BusinessCube!.Version == BusinessVersion.BusExpRTM;}
    }
}
