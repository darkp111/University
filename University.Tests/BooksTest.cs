using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using University.Data;
using University.Interfaces;
using University.Models;
using University.Services;
using University.ViewModels;

namespace University.Tests
{
    [TestClass]
    public class BooksTest
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
                List<Book> books = new List<Book>
            {
                new Book { Title = "Book1", Author = "Author1", Publisher = "Publisher1", PublicationDate = new DateTime(2000, 1, 1), Isbn = "ISBN1", Genre = "Genre1", Description = "Description1" },
                new Book { Title = "Book2", Author = "Author2", Publisher = "Publisher2", PublicationDate = new DateTime(2005, 5, 5), Isbn = "ISBN2", Genre = "Genre2", Description = "Description2" },
                new Book { Title = "Book3", Author = "Author3", Publisher = "Publisher3", PublicationDate = new DateTime(2010, 10, 10), Isbn = "ISBN3", Genre = "Genre3", Description = "Description3" }
            };
                context.Books.AddRange(books);
                context.SaveChanges();
            }
        }

        [TestMethod]
        public void Show_all_books()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                BooksViewModel booksViewModel = new BooksViewModel(context, _dialogService);
                bool hasData = booksViewModel.Books.Any();
                Assert.IsTrue(hasData);
            }
        }

        [TestMethod]
        public void Add_book_without_description()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                AddBookViewModel addBookViewModel = new AddBookViewModel(context, _dialogService)
                {
                    Title = "New Book",
                    Author = "New Author",
                    Publisher = "New Publisher",
                    PublicationDate = new DateTime(2020, 1, 1),
                    Isbn = "New ISBN",
                    Genre = "New Genre"
                };
                addBookViewModel.Save.Execute(null);

                bool newBookExists = context.Books.Any(b => b.Title == "New Book" && b.Author == "New Author" && b.Publisher == "New Publisher" && b.PublicationDate == new DateTime(2020, 1, 1) && b.Isbn == "New ISBN" && b.Genre == "New Genre" && b.Description == "");
                Assert.IsTrue(newBookExists);
            }
        }

        [TestMethod]
        public void Add_book_with_description()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                AddBookViewModel addBookViewModel = new AddBookViewModel(context, _dialogService)
                {
                    Title = "New Book with Description",
                    Author = "New Author",
                    Publisher = "New Publisher",
                    PublicationDate = new DateTime(2020, 1, 1),
                    Isbn = "New ISBN",
                    Genre = "New Genre",
                    Description = "New Description"
                };
                addBookViewModel.Save.Execute(null);

                bool newBookExists = context.Books.Any(b => b.Title == "New Book with Description" && b.Author == "New Author" && b.Publisher == "New Publisher" && b.PublicationDate == new DateTime(2020, 1, 1) && b.Isbn == "New ISBN" && b.Genre == "New Genre" && b.Description == "New Description");
                Assert.IsTrue(newBookExists);
            }
        }

        [TestMethod]
        public void Add_book_without_author()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                AddBookViewModel addBookViewModel = new AddBookViewModel(context, _dialogService)
                {
                    Title = "Book without Author",
                    Publisher = "Publisher",
                    PublicationDate = new DateTime(2022, 1, 1),
                    Isbn = "ISBN",
                    Genre = "Genre",
                    Description = "Description"
                };
                addBookViewModel.Save.Execute(null);

                bool newBookExists = context.Books.Any(b => b.Title == "Book without Author" && b.Publisher == "Publisher" && b.PublicationDate == new DateTime(2022, 1, 1) && b.Isbn == "ISBN" && b.Genre == "Genre" && b.Description == "Description" && string.IsNullOrEmpty(b.Author));
                Assert.IsFalse(newBookExists);
            }
        }

        [TestMethod]
        public void Add_book_with_author()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                AddBookViewModel addBookViewModel = new AddBookViewModel(context, _dialogService)
                {
                    Title = "Book with Author",
                    Author = "Author",
                    Publisher = "Publisher",
                    PublicationDate = new DateTime(2022, 1, 1),
                    Isbn = "ISBN",
                    Genre = "Genre",
                    Description = "Description"
                };
                addBookViewModel.Save.Execute(null);

                bool newBookExists = context.Books.Any(b => b.Title == "Book with Author" && b.Author == "Author" && b.Publisher == "Publisher" && b.PublicationDate == new DateTime(2022, 1, 1) && b.Isbn == "ISBN" && b.Genre == "Genre" && b.Description == "Description");
                Assert.IsTrue(newBookExists);
            }
        }

        [TestMethod]
        public void Add_book_with_invalid_isbn()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                AddBookViewModel addBookViewModel = new AddBookViewModel(context, _dialogService)
                {
                    Title = "Invalid ISBN Book",
                    Author = "Author",
                    Publisher = "Publisher",
                    PublicationDate = new DateTime(2023, 1, 1),
                    Isbn = "Invalid ISBN",
                    Genre = "Genre",
                    Description = "Description"
                };
                addBookViewModel.Save.Execute(null);

                bool newBookExists = context.Books.Any(b => b.Title == "Invalid ISBN Book");
                Assert.IsTrue(newBookExists);
            }
        }
    }
}
