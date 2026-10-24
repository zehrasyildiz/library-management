using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement
{
    public enum BorrowResult { Borrowed, AddedToQueue, AlreadyHasBook, AlreadyInQueue }

    public enum ReturnResult { ReturnedToShelf, PassedToNextMember, NotBorrowed }

    public enum DeleteResult { Deleted, NotFound, BookIsBorrowed }
}
