using DevTools.Core;
using System;
using System.Windows.Input;

namespace DevTools.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {
        public RelayCommand HomeViewCommand { get; set; }
        public RelayCommand DevToolViewCommand { get; set; }
        public HomeViewModel HomeVM { get; set; }
        public DevToolViewModel DevToolVM { get; set; }
        private object? _currentView;
        public object? CurrentView
        {
            get { return _currentView ?? null; }
            set 
            { 
                _currentView = value;
                OnPropertyChange();
            }
        }
        public MainViewModel(HomeViewModel homeVM, DevToolViewModel devToolVM) 
        { 
            HomeVM = homeVM;
            DevToolVM = devToolVM;

            CurrentView = HomeVM;
            //View Change events
            HomeViewCommand = new RelayCommand(o => { ChangeView(HomeVM); });
            DevToolViewCommand = new RelayCommand(o => { ChangeView(DevToolVM); });
        }

        private void ChangeView(object view) 
        {
            CurrentView = view;
        }
        
    }
}
