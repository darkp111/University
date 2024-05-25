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
    public class EditStudentOrganizationViewModel : ViewModelBase, IDataErrorInfo
    {
        private readonly UniversityContext _context;
        private readonly IDialogService _dialogService;
        private StudentOrganization? _organization = new StudentOrganization();

        public string Error
        {
            get { return string.Empty; }
        }

        public string this[string columnName]
        {
            get
            {
                if (columnName == "Name")
                {
                    if (string.IsNullOrEmpty(Name))
                    {
                        return "Name is Required";
                    }
                }
                // Add validation for other properties if needed

                return string.Empty;
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
                LoadOrganizationData();
            }
        }

        private ICommand? _back = null;
        public ICommand Back
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
        public ICommand Save
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

        private void SaveData(object? obj)
        {
            if (!IsValid())
            {
                Response = "Please complete all required fields";
                return;
            }

            if (_organization is null)
            {
                return;
            }
            _organization.Name = Name;
            _organization.Advisor = Advisor;
            _organization.President = President;
            _organization.Description = Description;
            _organization.MeetingSchedule = MeetingSchedule;
            _organization.MemberShip = AssignedStudents.Where(s => s.IsSelected).ToList();
            _organization.Email = Email;

            _context.Entry(_organization).State = EntityState.Modified;
            _context.SaveChanges();

            Response = "Data Updated";
        }

        public EditStudentOrganizationViewModel(UniversityContext context, IDialogService dialogService)
        {
            _context = context;
            _dialogService = dialogService;
        }

        private bool IsValid()
        {
            string[] properties = { "Name" };
            foreach (string property in properties)
            {
                if (!string.IsNullOrEmpty(this[property]))
                {
                    return false;
                }
            }
            return true;
        }

        private void LoadOrganizationData()
        {
            if (_context?.StudentOrganizations is null)
            {
                return;
            }
            _organization = _context.StudentOrganizations.Find(OrganizationId);
            if (_organization is null)
            {
                return;
            }
            this.Name = _organization.Name;
            this.Advisor = _organization.Advisor;
            this.President = _organization.President;
            this.Description = _organization.Description;
            this.MeetingSchedule = _organization.MeetingSchedule;
            if (_organization.MemberShip is null)
            {
                return;
            }
            foreach (Student student in _organization.MemberShip)
            {
                if (student is not null && AssignedStudents is not null)
                {
                    var assignedStudents = AssignedStudents
                        .FirstOrDefault(s => s.StudentId == student.StudentId);
                    if (assignedStudents is not null)
                    {
                        assignedStudents.IsSelected = true;
                    }
                }
            }
            this.Email = _organization.Email;
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
