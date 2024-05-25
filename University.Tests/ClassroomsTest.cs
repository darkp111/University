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

namespace University.Tests
{
    [TestClass]
    public class ClassroomTest
    {
        private IDialogService _dialogService;
        private DbContextOptions<UniversityContext> _options;

        [TestInitialize()]
        public void Initialize()
        {
            _options = new DbContextOptionsBuilder<UniversityContext>()
                .UseInMemoryDatabase(databaseName: "UniversityTestDB")
                .Options;
            SeedTestDB();
            _dialogService = new DialogService();
        }

        private void SeedTestDB()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                context.Database.EnsureDeleted();
                List<Classroom> classrooms = new List<Classroom>
            {
                new Classroom { ClassroomId = "A101", Location = "Building A", Capacity = 50, AvailableSeats = 50, Projector = true, Whiteboard = true, Microphone = false, Description = "Large classroom" },
                new Classroom { ClassroomId = "B202", Location = "Building B", Capacity = 30, AvailableSeats = 30, Projector = false, Whiteboard = true, Microphone = false, Description = "Medium classroom" },
                new Classroom { ClassroomId = "C303", Location = "Building C", Capacity = 20, AvailableSeats = 20, Projector = false, Whiteboard = false, Microphone = true, Description = "Small classroom" }
            };
                context.Classrooms.AddRange(classrooms);
                context.SaveChanges();
            }
        }

        [TestMethod]
        public void Show_all_classrooms()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                ClassroomViewModel classroomsViewModel = new ClassroomViewModel(context, _dialogService);
                bool hasData = classroomsViewModel.Classrooms.Any();
                Assert.IsTrue(hasData);
            }
        }

        [TestMethod]
        public void Add_classroom_with_valid_capacity()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                AddClassroomViewModel addClassroomViewModel = new AddClassroomViewModel(context, _dialogService)
                {
                    Location = "Building D",
                    Capacity = 40,
                    AvailableSeats = 40,
                    Projector = true,
                    Whiteboard = true,
                    Microphone = true,
                    Description = "Large classroom"
                };
                addClassroomViewModel.Save.Execute(null);

                bool newClassroomExists = context.Classrooms.Any(c => c.Location == "Building D" && c.Capacity == 40 && c.AvailableSeats == 40 && c.Projector && c.Whiteboard && c.Microphone && c.Description == "Large classroom");
                Assert.IsTrue(newClassroomExists);
            }
        }

        [TestMethod]
        public void Add_classroom_with_invalid_capacity()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                AddClassroomViewModel addClassroomViewModel = new AddClassroomViewModel(context, _dialogService)
                {
                    Location = "Building E",
                    Capacity = -10, // Invalid capacity
                    AvailableSeats = -10,
                    Projector = true,
                    Whiteboard = true,
                    Microphone = false,
                    Description = "Invalid capacity classroom"
                };
                addClassroomViewModel.Save.Execute(null);

                bool newClassroomExists = context.Classrooms.Any(c => c.ClassroomId == "E505");
                Assert.IsFalse(newClassroomExists);
            }
        }

        [TestMethod]
        public void Add_classroom_with_valid_location()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                AddClassroomViewModel addClassroomViewModel = new AddClassroomViewModel(context, _dialogService)
                {
                    Location = "Building F",
                    Capacity = 30,
                    AvailableSeats = 30,
                    Projector = true,
                    Whiteboard = true,
                    Microphone = false,
                    Description = "Medium classroom"
                };
                addClassroomViewModel.Save.Execute(null);

                bool newClassroomExists = context.Classrooms.Any(c => c.Location == "Building F");
                Assert.IsTrue(newClassroomExists);
            }
        }

        [TestMethod]
        public void Add_classroom_without_location()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                AddClassroomViewModel addClassroomViewModel = new AddClassroomViewModel(context, _dialogService)
                {
                    Capacity = 20,
                    AvailableSeats = 20,
                    Projector = false,
                    Whiteboard = false,
                    Microphone = true,
                    Description = "Small classroom"
                };
                addClassroomViewModel.Save.Execute(null);

                bool newClassroomExists = context.Classrooms.Any(c => c.ClassroomId == "G707" && string.IsNullOrEmpty(c.Location));
                Assert.IsFalse(newClassroomExists);
            }
        }

        [TestMethod]
        public void Add_classroom_with_valid_description()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                AddClassroomViewModel addClassroomViewModel = new AddClassroomViewModel(context, _dialogService)
                {
                    Location = "Building H",
                    Capacity = 25,
                    AvailableSeats = 25,
                    Projector = true,
                    Whiteboard = true,
                    Microphone = true,
                    Description = "Description for classroom H"
                };
                addClassroomViewModel.Save.Execute(null);

                bool newClassroomExists = context.Classrooms.Any(c => c.Description == "Description for classroom H");
                Assert.IsTrue(newClassroomExists);
            }
        }

        [TestMethod]
        public void Add_classroom_without_description()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                AddClassroomViewModel addClassroomViewModel = new AddClassroomViewModel(context, _dialogService)
                {
                    Location = "Building I",
                    Capacity = 35,
                    AvailableSeats = 35,
                    Projector = false,
                    Whiteboard = true,
                    Microphone = false
                };
                addClassroomViewModel.Save.Execute(null);

                bool newClassroomExists = context.Classrooms.Any(c => string.IsNullOrEmpty(c.Description));
                Assert.IsFalse(newClassroomExists);
            }
        }
    }
}
