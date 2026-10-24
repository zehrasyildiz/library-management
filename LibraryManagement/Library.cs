using System;
using System.Collections.Generic;

namespace LibraryManagement
{
    public class Library
    {
        // private: Dictionary'ye dışarıdan erişim yok, sadece bu sınıfın metotlarıyla
        private readonly Dictionary<string, Book> _books = new Dictionary<string, Book>();

        // true: eklendi, false: bu ISBN zaten var
        public bool AddBook(Book book)
        {
            return _books.TryAdd(book.Isbn, book);
        }

        public bool TryGetBook(string isbn, out Book? book)
        {
            return _books.TryGetValue(isbn, out book);
        }

        public DeleteResult DeleteBook(string isbn)
        {
            if (!_books.TryGetValue(isbn, out Book? book) || book == null)
                return DeleteResult.NotFound;

            if (!book.IsAvailable || book.WaitingList.Count > 0)
                return DeleteResult.BookIsBorrowed;

            _books.Remove(isbn);
            return DeleteResult.Deleted;
        }

        public List<Book> Search(string term)
        {
            List<Book> results = new List<Book>();

            foreach (Book book in _books.Values)
            {
                if (book.Title.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    book.Author.Contains(term, StringComparison.OrdinalIgnoreCase))
                {
                    results.Add(book);
                }
            }
            return results;
        }

        // Kopya liste döner, dışarıdaki değişiklik Dictionary'yi bozmaz
        public List<Book> GetAll()
        {
            return new List<Book>(_books.Values);
        }

        public List<Book> GetSorted()
        {
            List<Book> sorted = new List<Book>(_books.Values);
            sorted.Sort();
            return sorted;
        }
    }
}