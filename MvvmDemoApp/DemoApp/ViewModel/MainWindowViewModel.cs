using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Data;
using DemoApp.DataAccess;
using DemoApp.Model;
using DemoApp.Properties;
using System.Windows;
using System.Windows.Controls;

namespace DemoApp.ViewModel
{
    /// <summary>
    /// The ViewModel for the application's main window.
    /// </summary>
    public class MainWindowViewModel : WorkspaceViewModel
    {
        #region Fields
                
        ReadOnlyCollection<CommandViewModel> _commands;
        readonly CustomerRepository _customerRepository;
        ObservableCollection<WorkspaceViewModel> _workspaces;
        private List<ReturnType> _comboItems;
        public List<ReturnType> ComboItems
        {
            get { return _comboItems; }
            set
            {
                _comboItems = value;

                base.OnPropertyChanged("ComboItems");
            }
        }

        #endregion // Fields

        #region Constructor

        public MainWindowViewModel(string customerDataFile)
        {


            base.DisplayName = Strings.MainWindowViewModel_DisplayName;
            _comboItems = new List<ReturnType>();
            _comboItems.Add(new ReturnType() { Name = "11111" });
            _comboItems.Add(new ReturnType() { Name = "22222" });
            _comboItems.Add(new ReturnType() { Name = "33333" });
            _comboItems.Add(new ReturnType() { Name = "44444" });
            _comboItems.Add(new ReturnType() { Name = "55555" });
            _comboItems.Add(new ReturnType() { Name = "66666" });

            _customerRepository = new CustomerRepository(customerDataFile);
            simpleCommand = new RelayCommand(DoSimpleCommand);
            simpleCommand1 = new RelayCommand(DoSimpleCommand1);
        }

        public class ReturnType
        {
            public string Name;
        }

        private List<ReturnType> _SourceBinding;

        public List<ReturnType> SourceBinding
        {
            get
            {
                return _SourceBinding;
            }
            set
            {
                _SourceBinding = value;
                base.OnPropertyChanged("SourceBinding");
            }
        }

        private string _TextBinding;

        public string TextBinding
        {
            get { return _TextBinding; }
            set
            {
                _TextBinding = value;
                Filter(value);
                base.OnPropertyChanged("TextBinding");
            }
        }

        private void Filter(string value)
        {
            SourceBinding = (from m in ComboItems
                             where m.Name.ToLower().StartsWith(value.ToLower())
                             select m).ToList();
            if (SourceBinding != null && SourceBinding.Count > 0)
            {
                IsDropDownOpen = true;
            }
        }

        private bool _IsDropDownOpen;

        public bool IsDropDownOpen
        {
            get { return _IsDropDownOpen; }
            set
            {
                _IsDropDownOpen = value;
                base.OnPropertyChanged("IsDropDownOpen");
            }
        }  

        private bool _isExpand;
        public bool IsExpand
        {
            get { return _isExpand; }
            set
            {
                _isExpand = value;

                base.OnPropertyChanged("IsExpand");
            }
        }

        private string _cbItem;
        public string CbItem
        {
            get { return _cbItem; }
            set
            {
                _cbItem = value;

                base.OnPropertyChanged("CbItem");
            }
        }

        private void DoSimpleCommand(object obj)
        {
            if (IsExpand)
            {
                MessageBox.Show(CbItem);
                IsExpand = false; 
            }
        }

        private void DoSimpleCommand1(object obj)
        {
            IsExpand = false;
            MessageBox.Show(CbItem);
        }

        #endregion // Constructor

        #region Commands

        private RelayCommand simpleCommand;

        public RelayCommand SimpleCommand
        {
            get { return simpleCommand; }
        }

        private RelayCommand simpleCommand1;

        public RelayCommand SimpleCommand1
        {
            get { return simpleCommand1; }
        }

        /// <summary>
        /// Returns a read-only list of commands 
        /// that the UI can display and execute.
        /// </summary>
        public ReadOnlyCollection<CommandViewModel> Commands
        {
            get
            {
                if (_commands == null)
                {
                    List<CommandViewModel> cmds = this.CreateCommands();
                    _commands = new ReadOnlyCollection<CommandViewModel>(cmds);
                }
                return _commands;
            }
        }

        List<CommandViewModel> CreateCommands()
        {
            return new List<CommandViewModel>
            {
                new CommandViewModel(
                    Strings.MainWindowViewModel_Command_ViewAllCustomers,
                    new RelayCommand(param => this.ShowAllCustomers())),

                new CommandViewModel(
                    Strings.MainWindowViewModel_Command_CreateNewCustomer,
                    new RelayCommand(param => this.CreateNewCustomer()))
            };
        }

        #endregion // Commands

        #region Workspaces

        /// <summary>
        /// Returns the collection of available workspaces to display.
        /// A 'workspace' is a ViewModel that can request to be closed.
        /// </summary>
        public ObservableCollection<WorkspaceViewModel> Workspaces
        {
            get
            {
                if (_workspaces == null)
                {
                    _workspaces = new ObservableCollection<WorkspaceViewModel>();
                    _workspaces.CollectionChanged += this.OnWorkspacesChanged;
                }
                return _workspaces;
            }
        }

        void OnWorkspacesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count != 0)
                foreach (WorkspaceViewModel workspace in e.NewItems)
                    workspace.RequestClose += this.OnWorkspaceRequestClose;

            if (e.OldItems != null && e.OldItems.Count != 0)
                foreach (WorkspaceViewModel workspace in e.OldItems)
                    workspace.RequestClose -= this.OnWorkspaceRequestClose;
        }

        void OnWorkspaceRequestClose(object sender, EventArgs e)
        {
            WorkspaceViewModel workspace = sender as WorkspaceViewModel;
            workspace.Dispose();
            this.Workspaces.Remove(workspace);
        }

        #endregion // Workspaces

        #region Private Helpers

        void CreateNewCustomer()
        {
            Customer newCustomer = Customer.CreateNewCustomer();
            CustomerViewModel workspace = new CustomerViewModel(newCustomer, _customerRepository);
            this.Workspaces.Add(workspace);
            this.SetActiveWorkspace(workspace);
        }

        void ShowAllCustomers()
        {
            AllCustomersViewModel workspace =
                this.Workspaces.FirstOrDefault(vm => vm is AllCustomersViewModel)
                as AllCustomersViewModel;

            if (workspace == null)
            {
                workspace = new AllCustomersViewModel(_customerRepository);
                this.Workspaces.Add(workspace);
            }

            this.SetActiveWorkspace(workspace);
        }

        void SetActiveWorkspace(WorkspaceViewModel workspace)
        {
            Debug.Assert(this.Workspaces.Contains(workspace));

            ICollectionView collectionView = CollectionViewSource.GetDefaultView(this.Workspaces);
            if (collectionView != null)
                collectionView.MoveCurrentTo(workspace);
        }

        #endregion // Private Helpers
    }
}
