using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using University.Data;
using University.Interfaces;
using University.Models;

namespace University.ViewModels
{
    public class AddStudentOrganizationViewModel : ViewModelBase, IDataErrorInfo
    {
        private readonly UniversityContext _context;
        private readonly IDialogService _dialogService;



        public string Error
        {
            get { return string.Empty; }
        }

        public string this[string columnName]
        {
            get
            {
                // Validation logic for properties
                return string.Empty;
            }
        }

        private int _organizationId = 0;
        public int OrganizationId
        {
            get
            {
                return _organizationId;
            }
            set
            {
                _organizationId = value;
                OnPropertyChanged(nameof(OrganizationId));
            }
        }

        private string _name = string.Empty;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private string _advisor = string.Empty;
        public string Advisor
        {
            get
            {
                return _advisor;
            }
            set
            {
                _advisor = value;
                OnPropertyChanged(nameof(Advisor));
            }
        }

        private string _president = string.Empty;
        public string President
        {
            get
            {
                return _president;
            }
            set
            {
                _president = value;
                OnPropertyChanged(nameof(President));
            }
        }

        private string _description = string.Empty;
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        private string _meetingSchedule = string.Empty;
        public string MeetingSchedule
        {
            get
            {
                return _meetingSchedule;
            }
            set
            {
                _meetingSchedule = value;
                OnPropertyChanged(nameof(MeetingSchedule));
            }
        }

        private string _membership = string.Empty;
        public string MemberShip
        {
            get
            {
                return _membership;
            }
            set
            {
                _membership = value;
                OnPropertyChanged(nameof(MemberShip));
            }
        }

        private string _email = string.Empty;
        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        private string _response = string.Empty;
        public string Response
        {
            get
            {
                return _response;
            }
            set
            {
                _response = value;
                OnPropertyChanged(nameof(Response));
            }
        }

        private ICommand? _back = null;
        public ICommand? Back
        {
            get
            {
                if (_back is null)
                {
                    _back = new RelayCommand<object>(NavigateBack);
                }
                return _back;
            }
        }

        private void NavigateBack(object? obj)
        {
            var instance = MainWindowViewModel.Instance();
            if (instance is not null)
            {
                instance.StudentOrganizationsSubView = new StudentOrganizationsViewModel(_context, _dialogService);
            }
        }

        private ICommand? _save = null;
        public ICommand? Save
        {
            get
            {
                if (_save is null)
                {
                    _save = new RelayCommand<object>(SaveData);
                }
                return _save;
            }
        }


        public void SaveData(object? obj)
        {
            if (!IsValid())
            {
                Response = "Please complete all required fields";
                return;
            }

            var organization = new StudentOrganization
            {
                Name = this.Name,
                Advisor = this.Advisor,
                President = this.President,
                Description = this.Description,
                MeetingSchedule = this.MeetingSchedule,
                MemberShip = AssignedStudents.Where(s => s.IsSelected).ToList(),
                Email = this.Email
            };

            _context.StudentOrganizations.Add(organization);
            _context.SaveChanges();

            Response = "Data Saved";
        }

        // Constructor
        public AddStudentOrganizationViewModel(UniversityContext context, IDialogService dialogService)
        {
            _context = context;
            _dialogService = dialogService;
        }

        private bool IsValid()
        {
            string[] properties = { "Name", "Advisor", "President", "Description", "MeetingSchedule", "Email" };
            foreach (string property in properties)
            {
                if (!string.IsNullOrEmpty(this[property]))
                {
                    return false;
                }
            }
            return true;
        }

        private ObservableCollection<Student>? _assignedStudents = null;
        public ObservableCollection<Student> AssignedStudents
        {
            get
            {
                if (_assignedStudents is null)
                {
                    _assignedStudents = LoadStudents();
                    return _assignedStudents;
                }
                return _assignedStudents;
            }
            set
            {
                _assignedStudents = value;
                OnPropertyChanged(nameof(AssignedStudents));
            }
        }

        private ObservableCollection<Student> LoadStudents()
        {
            _context.Database.EnsureCreated();
            _context.Students.Load();
            return _context.Students.Local.ToObservableCollection();
        }

    }
}
