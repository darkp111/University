﻿using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
    public class EditBookViewModel : ViewModelBase, IDataErrorInfo
    {
        private readonly UniversityContext _context;
        private readonly IDialogService _dialogService;
        private Book? _book = new Book();

        public string Error
        {
            get { return string.Empty; }
        }

        public string this[string columnName]
        {
            get
            {
                if (columnName == "Title")
                {
                    if (string.IsNullOrEmpty(Title))
                    {
                        return "Title is Required";
                    }
                }
                if (columnName == "Author")
                {
                    if (string.IsNullOrEmpty(Author))
                    {
                        return "Author is Required";
                    }
                }
                if (columnName == "Publisher")
                {
                    if (string.IsNullOrEmpty(Publisher))
                    {
                        return "Publisher is Required";
                    }
                }
                if (columnName == "PublicationDate")
                {
                    if (PublicationDate is null)
                    {
                        return "Publication Date is Required";
                    }
                }
                return string.Empty;
            }
        }

        private string _title = string.Empty;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        private string _author = string.Empty;
        public string Author
        {
            get
            {
                return _author;
            }
            set
            {
                _author = value;
                OnPropertyChanged(nameof(Author));
            }
        }

        private string _publisher = string.Empty;
        public string Publisher
        {
            get
            {
                return _publisher;
            }
            set
            {
                _publisher = value;
                OnPropertyChanged(nameof(Publisher));
            }
        }

        private DateTime? _publicationDate = null;
        public DateTime? PublicationDate
        {
            get
            {
                return _publicationDate;
            }
            set
            {
                _publicationDate = value;
                OnPropertyChanged(nameof(PublicationDate));
            }
        }

        private string _isbn = string.Empty;
        public string Isbn
        {
            get
            {
                return _isbn;
            }
            set
            {
                _isbn = value;
                OnPropertyChanged(nameof(Isbn));
            }
        }

        private string _genre = string.Empty;
        public string Genre
        {
            get
            {
                return _genre;
            }
            set
            {
                _genre = value;
                OnPropertyChanged(nameof(Genre));
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

        private long _bookId = 0;
        public long BookId
        {
            get
            {
                return _bookId;
            }
            set
            {
                _bookId = value;
                OnPropertyChanged(nameof(BookId));
                LoadBookData();
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
                instance.BooksSubView = new BooksViewModel(_context, _dialogService);
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

            if (_book is null)
            {
                return;
            }
            _book.Title = Title;
            _book.Author = Author;
            _book.Publisher = Publisher;
            _book.PublicationDate = PublicationDate;
            _book.Isbn = Isbn;
            _book.Genre = Genre;
            _book.Description = Description;

            _context.Entry(_book).State = EntityState.Modified;
            _context.SaveChanges();

            Response = "Data Updated";
        }

        public EditBookViewModel(UniversityContext context, IDialogService dialogService)
        {
            _context = context;
            _dialogService = dialogService;
        }

        private bool IsValid()
        {
            string[] properties = { "Title", "Author", "Publisher", "PublicationDate" };
            foreach (string property in properties)
            {
                if (!string.IsNullOrEmpty(this[property]))
                {
                    return false;
                }
            }
            return true;
        }

        private void LoadBookData()
        {
            if (_context?.Books is null)
            {
                return;
            }
            _book = _context.Books.Find(BookId);
            if (_book is null)
            {
                return;
            }
            this.Title = _book.Title;
            this.Author = _book.Author;
            this.Publisher = _book.Publisher;
            this.PublicationDate = _book.PublicationDate;
            this.Isbn = _book.Isbn;
            this.Genre = _book.Genre;
            this.Description = _book.Description;
        }
    }

}
