using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("Welcome to the Library Management System!\n");

            Library library = new Library();
            SeedData(library);

            while (true)
            {
                Console.WriteLine("=========================================");
                Console.WriteLine("1. List books (Kitapları Listele)");
                Console.WriteLine("2. Add book (Yeni Kitap Ekle)");
                Console.WriteLine("3. Search books (Kitap Ara)");
                Console.WriteLine("4. Delete book (Kitap Sil)");
                Console.WriteLine("5. Borrow book (Ödünç Al)");
                Console.WriteLine("6. Return book (İade Et)");
                Console.WriteLine("7. List books sorted (Sıralı Listele)");
                Console.WriteLine("0. Exit (Çıkış)");
                Console.WriteLine("=========================================");

                string choice = ReadInput("Seçiminiz / Your choice: ");
                Console.WriteLine();

                switch (choice)
                {
                    case "1": ListBooks(library.GetAll(), "Kütüphane Kitap Listesi"); break;
                    case "2": AddBook(library); break;
                    case "3": SearchBooks(library); break;
                    case "4": DeleteBook(library); break;
                    case "5": BorrowBook(library); break;
                    case "6": ReturnBook(library); break;
                    case "7": ListBooks(library.GetSorted(), "Alfabetik Sıralı Kitap Listesi"); break;
                    case "0":
                        Console.WriteLine("Programdan çıkılıyor... İyi günler!");
                        return;
                    default:
                        Console.WriteLine("Geçersiz seçim! Lütfen 0 ile 7 arasında bir değer giriniz.\n");
                        break;
                }
            }
        }

        // Girdi okuma ve boşluk temizleme tek yerde / read-and-trim in one place
        static string ReadInput(string prompt)
        {
            Console.Write(prompt);
            return (Console.ReadLine() ?? string.Empty).Trim();
        }

        static void SeedData(Library library)
        {
            library.AddBook(new Book { Isbn = "978-975-07-3860-1", Title = "Kördüğüm", Author = "Ayşe Kulin", Year = 2002 });
            library.AddBook(new Book { Isbn = "978-975-363-802-2", Title = "Kürk Mantolu Madonna", Author = "Sabahattin Ali", Year = 1943 });

            Book inceMemed = new Book { Isbn = "978-975-08-1158-3", Title = "İnce Memed", Author = "Yaşar Kemal", Year = 1955 };
            inceMemed.Borrow("Ahmet Yılmaz"); // IsAvailable artık dışarıdan set edilemediği için Borrow ile
            library.AddBook(inceMemed);
        }

        static void PrintBook(Book book)
        {
            string status = book.IsAvailable ? "Rafta" : $"Ödünç Verildi ({book.BorrowedBy})";
            string queueInfo = book.WaitingList.Count > 0 ? $" | Bekleyen: {book.WaitingList.Count}" : "";

            Console.WriteLine($"ISBN: {book.Isbn} | Başlık: {book.Title} | Yazar: {book.Author} | Yıl: {book.Year} | Durum: {status}{queueInfo}");
        }

        // Hem listeleme hem sıralı listeleme hem arama sonucu için tek metot
        static void ListBooks(List<Book> books, string heading)
        {
            Console.WriteLine($"--- {heading} ---");

            if (books.Count == 0)
            {
                Console.WriteLine("Gösterilecek kitap yok.\n");
                return;
            }

            foreach (Book book in books)
            {
                PrintBook(book);
            }
            Console.WriteLine();
        }

        static void AddBook(Library library)
        {
            Console.WriteLine("--- Yeni Kitap Ekleme ---");

            string isbn = ReadInput("ISBN giriniz: ");
            if (string.IsNullOrWhiteSpace(isbn))
            {
                Console.WriteLine("Hata: ISBN boş bırakılamaz!\n");
                return;
            }

            if (library.TryGetBook(isbn, out _))
            {
                Console.WriteLine($"Hata: Bu ISBN ({isbn}) zaten mevcut!\n");
                return;
            }

            string title = ReadInput("Başlık giriniz: ");
            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Hata: Başlık boş bırakılamaz!\n");
                return;
            }

            string author = ReadInput("Yazar giriniz: ");
            if (string.IsNullOrWhiteSpace(author))
            {
                Console.WriteLine("Hata: Yazar boş bırakılamaz!\n");
                return;
            }

            int year;
            while (true)
            {
                string yearInput = ReadInput($"Yıl giriniz (1000 - {DateTime.Now.Year}): ");
                if (int.TryParse(yearInput, out year) && year >= 1000 && year <= DateTime.Now.Year)
                    break;

                Console.WriteLine($"Hatalı giriş! 1000 ile {DateTime.Now.Year} arasında geçerli bir yıl giriniz.");
            }

            library.AddBook(new Book { Isbn = isbn, Title = title, Author = author, Year = year });
            Console.WriteLine("Kitap başarıyla eklendi!\n");
        }

        static void SearchBooks(Library library)
        {
            Console.WriteLine("--- Kitap Arama ---");
            string term = ReadInput("Aranacak kelime (Başlık veya Yazar): ");

            if (string.IsNullOrWhiteSpace(term))
            {
                Console.WriteLine("Arama kelimesi boş olamaz!\n");
                return;
            }

            ListBooks(library.Search(term), "Arama Sonuçları");
        }

        static void DeleteBook(Library library)
        {
            Console.WriteLine("--- Kitap Silme ---");
            string isbn = ReadInput("Silinecek kitabın ISBN numarası: ");

            if (string.IsNullOrWhiteSpace(isbn))
            {
                Console.WriteLine("ISBN boş bırakılamaz!\n");
                return;
            }

            switch (library.DeleteBook(isbn))
            {
                case DeleteResult.Deleted:
                    Console.WriteLine($"ISBN: {isbn} numaralı kitap silindi!\n");
                    break;
                case DeleteResult.BookIsBorrowed:
                    Console.WriteLine("Hata: Kitap ödünçte veya bekleme sırası var, silinemez!\n");
                    break;
                case DeleteResult.NotFound:
                    Console.WriteLine("Bu ISBN ile kitap bulunamadı.\n");
                    break;
            }
        }

        static void BorrowBook(Library library)
        {
            Console.WriteLine("--- Kitap Ödünç Alma ---");
            string isbn = ReadInput("Ödünç alınacak kitabın ISBN numarası: ");

            if (!library.TryGetBook(isbn, out Book? book) || book == null)
            {
                Console.WriteLine("Bu ISBN ile kitap bulunamadı.\n");
                return;
            }

            string member = ReadInput("Üye adını giriniz: ");
            if (string.IsNullOrWhiteSpace(member))
            {
                Console.WriteLine("Üye adı boş bırakılamaz!\n");
                return;
            }

            switch (book.Borrow(member))
            {
                case BorrowResult.Borrowed:
                    Console.WriteLine($"'{book.Title}' kitabı {member} adlı üyeye ödünç verildi!\n");
                    break;
                case BorrowResult.AddedToQueue:
                    Console.WriteLine($"Kitap ödünçte. Bekleme sırasına eklendiniz. Sıra numaranız: {book.WaitingList.Count}\n");
                    break;
                case BorrowResult.AlreadyHasBook:
                    Console.WriteLine("Hata: Bu kitap zaten sizin elinizde!\n");
                    break;
                case BorrowResult.AlreadyInQueue:
                    Console.WriteLine("Hata: Bu kitabın bekleme sırasında zaten kayıtlısınız!\n");
                    break;
            }
        }

        static void ReturnBook(Library library)
        {
            Console.WriteLine("--- Kitap İade Etme ---");
            string isbn = ReadInput("İade edilecek kitabın ISBN numarası: ");

            if (!library.TryGetBook(isbn, out Book? book) || book == null)
            {
                Console.WriteLine("Bu ISBN ile kitap bulunamadı.\n");
                return;
            }

            switch (book.Return())
            {
                case ReturnResult.NotBorrowed:
                    Console.WriteLine("Hata: Bu kitap zaten rafta, ödünç verilmemiş!\n");
                    break;
                case ReturnResult.PassedToNextMember:
                    Console.WriteLine($"Kitap iade edildi ve sıradaki '{book.BorrowedBy}' kişisine verildi!\n");
                    break;
                case ReturnResult.ReturnedToShelf:
                    Console.WriteLine("Kitap iade edildi ve rafa kaldırıldı.\n");
                    break;
            }
        }
    }
}