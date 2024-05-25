using University.Models;
using Microsoft.EntityFrameworkCore;

namespace University.Data
{
    public class UniversityContext : DbContext
    {
        public UniversityContext()
        {
        }

        public UniversityContext(DbContextOptions<UniversityContext> options) : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Classroom> Classrooms { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<StudentOrganization> StudentOrganizations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseInMemoryDatabase("UniversityDb");
                optionsBuilder.UseLazyLoadingProxies();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Subject>().Ignore(s => s.IsSelected);

            modelBuilder.Entity<Student>().HasData(
                new Student { StudentId = 1, Name = "Wieńczysław", LastName = "Nowakowicz", PESEL = "PESEL1", BirthDate = new DateTime(1987, 05, 22) },
                new Student { StudentId = 2, Name = "Stanisław", LastName = "Nowakowicz", PESEL = "PESEL2", BirthDate = new DateTime(2019, 06, 25) },
                new Student { StudentId = 3, Name = "Eugenia", LastName = "Nowakowicz", PESEL = "PESEL3", BirthDate = new DateTime(2021, 06, 08) });

            modelBuilder.Entity<Subject>().HasData(
                new Subject { SubjectId = 1, Name = "Matematyka", Semester = "1", Lecturer = "Michalina Warszawa" },
                new Subject { SubjectId = 2, Name = "Biologia", Semester = "2", Lecturer = "Halina Katowice" },
                new Subject { SubjectId = 3, Name = "Chemia", Semester = "3", Lecturer = "Jan Nowak" }
            );

            modelBuilder.Entity<Classroom>().HasData(
                new Classroom { ClassroomId = "92", Location = "Haramamburu", Capacity = 4, AvailableSeats = 1, Projector = true, Whiteboard = false, Microphone = false, Description="Baldezh" },
                new Classroom { ClassroomId = "45", Location = "Ko43Za$Pa", Capacity = 665, AvailableSeats = 660, Projector = false, Whiteboard = true, Microphone = false, Description = "KaifoHata" }
            );

            modelBuilder.Entity<Book>().HasData(
                new Book { BookId=222, Title = "Short", Author = "Ray Bredberry", Publisher="Pushkaridze",  PublicationDate = new DateTime(1987, 05, 22), Isbn = "234787234", Genre = "Roman", Description = "Blablabla" }
                );

            modelBuilder.Entity<StudentOrganization>().HasData(
                new StudentOrganization {OrganizationId=123, Name="Nabuzje", Advisor="NoName",President="Artem", Description="OOO Nabuzje", MeetingSchedule="No schedule", Email="yatrahal@ukr.net" }
                );
        }
    }
}
