using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevTools.MVVM.Model.DTO
{
    public class CfgConfiguration
    {
        /// <summary>
        /// Database delle procedure di Business
        /// </summary>
        public string? ArcprocName { get; set; }
        /// <summary>
        /// Server su cui si trova l'Arcproc
        /// </summary>
        public string? ArcProcServerName { get; set; }
        /// <summary>
        /// Database Ditta di Business
        /// </summary>
        public string? DittaName { get; set; }
        /// <summary>
        /// Server su cui si trova il DB della Ditta
        /// </summary>
        public string? DittaServerName { get; set; }
        /// <summary>
        /// Nome da dare dal file cfg
        /// </summary>
        public string? CfgName { get; set; }
        public string GetCfgFileName() { return $"{((this.CfgName is null) ? "Business_" + DateTime.Now.ToString("MM-dd-yyyy") : this.CfgName)}"; }
        public string GetArcprocConnetionString() { return $"Server={this.ArcProcServerName};Database={this.ArcprocName};UID=sa;pwd=nts"; }
    }
}
