using DevTools.Core;
using DevTools.Core.Utility;
using DevTools.Enum;
using DevTools.MVVM.Model;
using DevTools.Services.ConfigurationFile;
using DevTools.Services.Solution;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static System.Net.Mime.MediaTypeNames;

namespace DevTools.MVVM.ViewModel
{
    class DevToolViewModel : INotifyPropertyChanged
    {
        public DevToolModel? DevToolModel { get; set; }

        #region Commands
        //Cmd buttons
        public RelayCommand CreateCfg { get; }
        public RelayCommand CreateCfgAndConnection { get; }
        public RelayCommand CreateComplete { get; }
        public RelayCommand CreateSolution { get; }
        #endregion

        #region Service
        //Service
        public ISolutionBuilder _solutionBuilder { get; set; }
        public IConfigurationFileBuilder _configurationFileBuilder { get; set; }
        #endregion

        #region Bindings
        //Bindings
        private bool _setCfgDebugger;
        public bool SetCfgDebugger
        {
            get => _setCfgDebugger;
            set
            {
                if (_setCfgDebugger != value)
                {
                    _setCfgDebugger = value;
                    OnPropertyChanged(nameof(SetCfgDebugger)); // Notify UI of changes
                }
            }
        }
        private bool _deleteGdpr;
        public bool DeleteGdpr
        {
            get => _deleteGdpr;
            set
            {
                if (_deleteGdpr != value)
                {
                    _deleteGdpr = value;
                    OnPropertyChanged(nameof(DeleteGdpr)); // Notify UI of changes
                }
            }
        }
        private string _solutionsTxt;
        public string SolutionsTxt
        {
            get => _solutionsTxt;
            set
            {
                if (_solutionsTxt != value)
                {
                    _solutionsTxt = value;
                    OnPropertyChanged(nameof(SolutionsTxt)); // Notify UI of changes
                }
            }
        }
        private string _arcprocName;
        public string ArcprocName
        {
            get => _arcprocName;
            set
            {
                if (_arcprocName != value)
                {
                    _arcprocName = value;
                    OnPropertyChanged(nameof(ArcprocName)); // Notify UI of changes
                }
            }
        }
        private string _arcProcServerName;
        public string ArcProcServerName
        {
            get => _arcProcServerName;
            set
            {
                if (_arcProcServerName != value)
                {
                    _arcProcServerName = value;
                    OnPropertyChanged(nameof(ArcProcServerName)); // Notify UI of changes
                }
            }
        }
        private string _cfgName;
        public string CfgName
        {
            get => _cfgName;
            set
            {
                if (_cfgName != value)
                {
                    _cfgName = value;
                    OnPropertyChanged(nameof(CfgName)); // Notify UI of changes
                }
            }
        }
        private string _selectedVersion;
        public string SelectedVersion
        {
            get => _selectedVersion;
            set
            {
                if (_selectedVersion != value)
                {
                    _selectedVersion = value;
                    OnPropertyChanged(nameof(SelectedVersion));
                }
            }
        }
        // Notify property changed method (if needed)
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        public DevToolViewModel(
            ISolutionBuilder solutionBuilder, 
            IConfigurationFileBuilder configurationFileBuilder
            ) 
        {
            //Inizializzazione Injection
            _solutionBuilder = solutionBuilder;
            _configurationFileBuilder = configurationFileBuilder;

            //Comandi
            //Creazione della solutione
            CreateSolution = new RelayCommand(x =>
            {
                DevToolModel = new DevToolModel(version: SelectedVersion, solutionsTxt: SolutionsTxt, setCfgDebugger: SetCfgDebugger);
                _solutionBuilder.BuildSolution(DevToolModel);
            });
            //Creazione del fine di configurazione
            CreateCfg = new RelayCommand(x =>
            {
                DevToolModel = new DevToolModel(arcprocName: ArcprocName, arcProcServerName: ArcProcServerName, cfgName: CfgName, deleteGdpr: DeleteGdpr);
                _configurationFileBuilder.CreaCfg(DevToolModel);
            });
            //Creazione del fine di configurazione e della connessione all'arcproc
            CreateCfgAndConnection = new RelayCommand(x =>
            {
                DevToolModel = new DevToolModel(arcprocName: ArcprocName, arcProcServerName: ArcProcServerName, cfgName: CfgName, deleteGdpr: DeleteGdpr); 
                _configurationFileBuilder.CreaCfg(DevToolModel);
                _configurationFileBuilder.SetUpConnection(DevToolModel);
            });
            //Creazione completa delle fasi
            CreateComplete = new RelayCommand(x =>
            {
                DevToolModel = new DevToolModel(version: SelectedVersion, solutionsTxt: SolutionsTxt, setCfgDebugger: SetCfgDebugger,
                    arcprocName: ArcprocName, arcProcServerName: ArcProcServerName, cfgName: CfgName, deleteGdpr: DeleteGdpr);
                _solutionBuilder.BuildSolution(DevToolModel);
                _configurationFileBuilder.CreaCfg(DevToolModel);
                _configurationFileBuilder.SetUpConnection(DevToolModel);
            });
            var firstBusVersion = System.Enum.GetNames(typeof(BusinessVersion)).FirstOrDefault();
            // Default
            SelectedVersion = firstBusVersion ?? SelectedVersion; 
            SolutionsTxt = string.Empty;
            SetCfgDebugger = true;
        }
    }
}
