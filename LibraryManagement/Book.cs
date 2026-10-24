using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace LibraryManagement
{
    public class Book : IComparable<Book>
    {
        // Sıralama her bilgisayarda aynı sonucu versin diye kültürü sabitliyoruz
        private static readonly CultureInfo TurkishCulture = new CultureInfo("tr-TR");

        public string Isbn { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public int Year { get; set; }

        // private set: dışarıdan doğrudan değiştirilemez, sadece Borrow/Return ile değişir
        public bool IsAvailable { get; private set; } = true;
        public string? BorrowedBy { get; private set; }

        // Sadece get: kuyruğun kendisi dışarıdan yeniden atanamaz
        public Queue<string> WaitingList { get; } = new Queue<string>();

        public BorrowResult Borrow(string memberName)
        {
            if (!IsAvailable && string.Equals(BorrowedBy, memberName, StringComparison.OrdinalIgnoreCase))
                return BorrowResult.AlreadyHasBook;

            if (WaitingList.Any(m => string.Equals(m, memberName, StringComparison.OrdinalIgnoreCase)))
                return BorrowResult.AlreadyInQueue;

            if (IsAvailable)
            {
                IsAvailable = false;
                BorrowedBy = memberName;
                return BorrowResult.Borrowed;
            }

            WaitingList.Enqueue(memberName);
            return BorrowResult.AddedToQueue;
        }

        public ReturnResult Return()
        {
            if (IsAvailable)
                return ReturnResult.NotBorrowed;

            if (WaitingList.Count > 0)
            {
                BorrowedBy = WaitingList.Dequeue();
                return ReturnResult.PassedToNextMember;
            }

            IsAvailable = true;
            BorrowedBy = null;
            return ReturnResult.ReturnedToShelf;
        }

        public int CompareTo(Book? other)
        {
            if (other == null) return 1;
            return string.Compare(Title, other.Title, TurkishCulture, CompareOptions.None);
        }
    }
}