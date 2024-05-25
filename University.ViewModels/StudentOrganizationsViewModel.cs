using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using University.Data;
using University.Interfaces;
using University.Models;

namespace University.ViewModels
{
    public class StudentOrganizationsViewModel : ViewModelBase
    {
        private readonly UniversityContext _context;
        private readonly IDialogService _dialogService;

        private bool? _dialogResult = null;
        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { _dialogResult = value; }
        }

        private ObservableCollection<StudentOrganization>? _organizations = null;
        public ObservableCollection<StudentOrganization>? Organizations
        {
            get
            {
                if (_organizations is null)
                {
                    _organizations = new ObservableCollection<StudentOrganization>();
                    return _organizations;
                }
                return _organizations;
            }
            set
            {
                _organizations = value;
                OnPropertyChanged(nameof(Organizations));
            }
        }

        private ICommand? _add = null;
        public ICommand? Add
        {
            get
            {
                if (_add is null)
                {
                    _add = new RelayCommand<object>(AddNewOrganization);
                }
                return _add;
            }
        }

        private void AddNewOrganization(object? obj)
        {
            var instance = MainWindowViewModel.Instance();
            if (instance is not null)
            {
                instance.StudentOrganizationsSubView = new AddStudentOrganizationViewModel(_context, _dialogService);
            }
        }

        private ICommand? _edit = null;
        public ICommand? Edit
        {
            get
            {
                if (_edit is null)
                {
                    _edit = new RelayCommand<object>(EditOrganization);
                }
                return _edit;
            }
        }

        private void EditOrganization(object? obj)
        {
            if (obj is not null)
            {
                int organizationId = (int)obj;
                EditStudentOrganizationViewModel editOrganizationViewModel = new EditStudentOrganizationViewModel(_context, _dialogService)
                {
                    OrganizationId = organizationId
                };
                var instance = MainWindowViewModel.Instance();
                if (instance is not null)
                {
                    instance.StudentOrganizationsSubView = editOrganizationViewModel;
                }
            }
        }

        private ICommand? _remove = null;
        public ICommand? Remove
        {
            get
            {
                if (_remove is null)
                {
                    _remove = new RelayCommand<object>(RemoveOrganization);
                }
                return _remove;
            }
        }

        private void RemoveOrganization(object? obj)
        {
            if (obj is not null)
            {
                int organizationId = (int)obj;
                StudentOrganization? organization = _context.StudentOrganizations.Find(organizationId);
                if (organization is not null)
                {
                    DialogResult = _dialogService.Show(organization.Name);
                    if (DialogResult == false)
                    {
                        return;
                    }

                    _context.StudentOrganizations.Remove(organization);
                    _context.SaveChanges();
                }
            }
        }

        public StudentOrganizationsViewModel(UniversityContext context, IDialogService dialogService)
        {
            _context = context;
            _dialogService = dialogService;

            _context.Database.EnsureCreated();
            _context.StudentOrganizations.Load();
            Organizations = _context.StudentOrganizations.Local.ToObservableCollection();
        }
    }

}
