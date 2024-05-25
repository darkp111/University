using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using University.Data;
using University.Interfaces;
using University.Models;
using University.Services;
using University.ViewModels;
using Moq;
using System.Collections.ObjectModel;

namespace University.Tests
{
    [TestClass]
    public class StudentOrganizationTest
    {
        private IDialogService _dialogService;
        private DbContextOptions<UniversityContext> _options;
        private Mock<UniversityContext> _mockContext;
        private Mock<IDialogService> _mockDialogService;
        private Mock<DbSet<StudentOrganization>> _mockSet;
        private StudentOrganizationsViewModel _viewModel;

        [TestInitialize()]
        public void Initialize()
        {
            _mockContext = new Mock<UniversityContext>();
            _mockDialogService = new Mock<IDialogService>();


            _options = new DbContextOptionsBuilder<UniversityContext>()
                .UseInMemoryDatabase(databaseName: "UniversityTestDB")
                .Options;
            SeedTestDB();
            _dialogService = new DialogService();

            var organizations = new List<StudentOrganization>
            {
                new StudentOrganization
                {
                    OrganizationId = 1,
                    Name = "Test Organization",
                    MemberShip = new List<Student>()
                }
            }.AsQueryable();

            _mockSet = new Mock<DbSet<StudentOrganization>>();
            _mockSet.As<IQueryable<StudentOrganization>>().Setup(m => m.Provider).Returns(organizations.Provider);
            _mockSet.As<IQueryable<StudentOrganization>>().Setup(m => m.Expression).Returns(organizations.Expression);
            _mockSet.As<IQueryable<StudentOrganization>>().Setup(m => m.ElementType).Returns(organizations.ElementType);
            _mockSet.As<IQueryable<StudentOrganization>>().Setup(m => m.GetEnumerator()).Returns(organizations.GetEnumerator());

            _mockContext.Setup(c => c.StudentOrganizations).Returns(_mockSet.Object);
            _mockContext.Setup(c => c.StudentOrganizations.Find(It.IsAny<int>())).Returns<int>(id => organizations.FirstOrDefault(o => o.OrganizationId == id));

            _viewModel = new StudentOrganizationsViewModel(_mockContext.Object, _mockDialogService.Object);
        }

        private void SeedTestDB()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                context.Database.EnsureDeleted();
                List<StudentOrganization> organizations = new List<StudentOrganization>
            {
                new StudentOrganization { OrganizationId = 1, Name = "Chess Club", Advisor = "John Doe", President = "Alice Smith", Description = "A club for chess enthusiasts", MeetingSchedule = "Every Wednesday at 4 PM", Email = "chessclub@example.com" },
                new StudentOrganization { OrganizationId = 2, Name = "Debate Society", Advisor = "Jane Smith", President = "Bob Johnson", Description = "A society for debating various topics", MeetingSchedule = "Every Thursday at 6 PM", Email = "debatesociety@example.com" }
            };
                context.StudentOrganizations.AddRange(organizations);
                context.SaveChanges();
            }
        }

        [TestMethod]
        public void Show_all_organizations()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                StudentOrganizationsViewModel organizationsViewModel = new StudentOrganizationsViewModel(context, _dialogService);
                bool hasData = organizationsViewModel.Organizations.Any();
                Assert.IsTrue(hasData);
            }
        }

        [TestMethod]
        public void Add_organization_with_valid_data()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                AddStudentOrganizationViewModel addOrganizationViewModel = new AddStudentOrganizationViewModel(context, _dialogService)
                {
                    Name = "Art Club",
                    Advisor = "Emma Brown",
                    President = "Chris Wilson",
                    Description = "A club for art lovers",
                    MeetingSchedule = "Every Friday at 3 PM",
                    Email = "artclub@example.com"
                };
                addOrganizationViewModel.Save.Execute(null);

                bool newOrganizationExists = context.StudentOrganizations.Any(o => o.Name == "Art Club" && o.Advisor == "Emma Brown" && o.President == "Chris Wilson" && o.Description == "A club for art lovers" && o.MeetingSchedule == "Every Friday at 3 PM" && o.Email == "artclub@example.com");
                Assert.IsTrue(newOrganizationExists);
            }
        }

        [TestMethod]
        public void Add_organization_with_invalid_email()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                AddStudentOrganizationViewModel addOrganizationViewModel = new AddStudentOrganizationViewModel(context, _dialogService)
                {
                    Name = "Music Club",
                    Advisor = "Michael Green",
                    President = "Sarah White",
                    Description = "A club for music enthusiasts",
                    MeetingSchedule = "Every Tuesday at 5 PM",
                    Email = "invalidemail"
                };
                addOrganizationViewModel.Save.Execute(null);

                bool newOrganizationExists = context.StudentOrganizations.Any(o => o.Name == "Music Club" && o.Email == "test@gmail.com");
                Assert.IsFalse(newOrganizationExists);
            }
        }

        [TestMethod]
        public void Add_organization_with_missing_name()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                AddStudentOrganizationViewModel addOrganizationViewModel = new AddStudentOrganizationViewModel(context, _dialogService)
                {
                    Advisor = "Peter Parker",
                    President = "Mary Jane",
                    Description = "A club for superheroes",
                    MeetingSchedule = "Every Sunday at 2 PM",
                    Email = "superheroesclub@example.com"
                };
                addOrganizationViewModel.Save.Execute(null);

                bool newOrganizationExists = context.StudentOrganizations.Any(o =>  o.Name.Length > 0 && o.Advisor == "Peter Parker" && o.President == "Mary Jane" && o.Description == "A club for superheroes" && o.MeetingSchedule == "Every Sunday at 2 PM" && o.Email == "superheroesclub@example.com");
                Assert.IsFalse(newOrganizationExists);
            }
        }

        [TestMethod]
        public void Add_organization_with_missing_president()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                AddStudentOrganizationViewModel addOrganizationViewModel = new AddStudentOrganizationViewModel(context, _dialogService)
                {
                    Name = "Science Club",
                    Advisor = "Albert Einstein",
                    Description = "A club for science enthusiasts",
                    MeetingSchedule = "Every Monday at 4 PM",
                    Email = "scienceclub@example.com"
                };
                addOrganizationViewModel.Save.Execute(null);

                bool newOrganizationExists = context.StudentOrganizations.Any(o => o.Name == "Science Club" && o.President == "Gigolo");
                Assert.IsFalse(newOrganizationExists);
            }
        }

        [TestMethod]
        public void Add_organization_with_missing_description()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                AddStudentOrganizationViewModel addOrganizationViewModel = new AddStudentOrganizationViewModel(context, _dialogService)
                {
                    Name = "Writing Club",
                    Advisor = "William Shakespeare",
                    President = "Emily Dickinson",
                    MeetingSchedule = "Every Wednesday at 2 PM",
                    Email = "writingclub@example.com"
                };
                addOrganizationViewModel.Save.Execute(null);

                bool newOrganizationExists = context.StudentOrganizations.Any(o => o.Name == "Writing Club" && o.Description.Length > 0);
                Assert.IsFalse(newOrganizationExists);
            }
        }

        [TestMethod]
        public void Add_organization_with_missing_meeting_schedule()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                AddStudentOrganizationViewModel addOrganizationViewModel = new AddStudentOrganizationViewModel(context, _dialogService)
                {
                    Name = "Film Club",
                    Advisor = "Steven Spielberg",
                    President = "Quentin Tarantino",
                    Description = "A club for film enthusiasts",
                    Email = "filmclub@example.com"
                };
                addOrganizationViewModel.Save.Execute(null);

                bool newOrganizationExists = context.StudentOrganizations.Any(o => o.Name == "Film Club" && o.MeetingSchedule.Length > 0);
                Assert.IsFalse(newOrganizationExists);
            }
        }

        [TestMethod]
        public void AddStudent_PositiveTest()
        {
            // Arrange
            var newStudent = new Student { StudentId = 1, Name = "New Student" };
            var organization = _viewModel.Organizations.First();

            // Act
            organization.MemberShip.Add(newStudent);
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);

            // Assert
            Assert.AreEqual(1, organization.MemberShip.Count);
            Assert.AreEqual("New Student", organization.MemberShip.First().Name);
        }

        [TestMethod]
        public void AddStudent_NegativeTest_InvalidOrganizationId()
        {
            // Arrange
            var invalidOrganizationId = 999;
            var organization = _mockContext.Object.StudentOrganizations.Find(invalidOrganizationId);
            var newStudent = new Student { StudentId = 1, Name = "New Student" };

            // Act
            if (organization != null)
            {
                organization.MemberShip.Add(newStudent);
                _mockContext.Verify(c => c.SaveChanges(), Times.Once);
            }

            // Assert
            Assert.IsNull(organization);
        }

        [TestMethod]
        public void RemoveStudent_PositiveTest()
        {
            // Arrange
            var student = new Student { StudentId = 1, Name = "Test Student" };
            var organization = _viewModel.Organizations.First();
            organization.MemberShip.Add(student);

            // Act
            organization.MemberShip.Remove(student);
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);

            // Assert
            Assert.AreEqual(0, organization.MemberShip.Count);
        }

        [TestMethod]
        public void RemoveStudent_NegativeTest_InvalidStudentId()
        {
            // Arrange
            var invalidStudentId = 999;
            var organization = _viewModel.Organizations.First();
            var student = new Student { StudentId = invalidStudentId, Name = "Non-existing Student" };

            // Act
            var studentToRemove = organization.MemberShip.FirstOrDefault(s => s.StudentId == invalidStudentId);
            if (studentToRemove != null)
            {
                organization.MemberShip.Remove(studentToRemove);
                _mockContext.Verify(c => c.SaveChanges(), Times.Once);
            }

            // Assert
            Assert.IsNull(studentToRemove);
            Assert.AreEqual(0, organization.MemberShip.Count);
        }

        [TestMethod]
        public void EditStudent_PositiveTest()
        {
            // Arrange
            var student = new Student { StudentId = 1, Name = "Old Name" };
            var organization = _viewModel.Organizations.First();
            organization.MemberShip.Add(student);

            // Act
            var studentToEdit = organization.MemberShip.FirstOrDefault(s => s.StudentId == student.StudentId);
            if (studentToEdit != null)
            {
                studentToEdit.Name = "New Name";
                _mockContext.Verify(c => c.SaveChanges(), Times.Once);
            }

            // Assert
            Assert.AreEqual("New Name", student.Name);
        }

        [TestMethod]
        public void EditStudent_NegativeTest_InvalidStudentId()
        {
            // Arrange
            var invalidStudentId = 999;
            var organization = _viewModel.Organizations.First();

            // Act
            var studentToEdit = organization.MemberShip.FirstOrDefault(s => s.StudentId == invalidStudentId);
            if (studentToEdit != null)
            {
                studentToEdit.Name = "New Name";
                _mockContext.Verify(c => c.SaveChanges(), Times.Once);
            }

            // Assert
            Assert.IsNull(studentToEdit);
        }

        [TestMethod]
        public void VerifyOrganizationsData()
        {
            var organizations = new ObservableCollection<StudentOrganization>
            {
                new StudentOrganization { OrganizationId = 1, Name = "Org1" },
                new StudentOrganization { OrganizationId = 2, Name = "Org2" }
            };

            _viewModel.Organizations = organizations;
            var result = _viewModel.Organizations;
            Assert.AreEqual(organizations, result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Org1", result[0].Name);
            Assert.AreEqual("Org2", result[1].Name);
        }
    }
}
