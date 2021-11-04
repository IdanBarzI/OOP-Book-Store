using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace steimatzky.Model
{
    public enum BookGenre
    {
        Unknown,//Defult
        ComicBook,
        DetectiveAndMystery,
        Fantasy,
        Horror,
        Romance,
        ShortStories,
        BiographiesAndAutobiographies,
        Cookbooks,
        History,
        Poetry,
        TrueCrime
    }
    [Serializable]
    public class Book : BaseProduct
    {
        private BookGenre[] _bookGenres;
        private string _edition;
        private string _summary;
        private string _author;

        public BookGenre[] BookGenres { get { return _bookGenres; } set { _bookGenres = value; } }
        public string Author { get { return _author; } set { _author = value; } }
        public string Summary { get { return _summary; } set { _summary = value; } }
        public string Edition { get { return _edition; } set { _edition = value; } }

        public Book(string name, string author, double productPrice,
                    DateTime printDate, int quntity, ProductType productType = ProductType.Book,
                    BookGenre[] bookGenres = null, string summary = "", string edition = "")
                    : base(name, productPrice, printDate, quntity, productType)
        {
            if (bookGenres == null)
            {
                _bookGenres = new BookGenre[1];
                _bookGenres[0] = BookGenre.Unknown;
            }
            else
                _bookGenres = bookGenres;
            _author = author;
            _summary = summary;
            _edition = edition;
        }

        public override string ToString()
        {
            return $"Product type: {ProductType}, {ProductType} Name: {Name}, Author {Author}\n" +
                   $"Price:{ProductPrice:C}, Id {Id}, Print date:{PrintDate}\n" +
                   $"Quntity:{QuantityInStock},Quntity removed:{QuantityRemoved}\n" +
                   $"Adding date:{AddingDate}, Print date:{PrintDate}\n" +
                   $"Ganre: {GenersToString()}\n" +
                   $" Summary: {Summary}, Edition: {Edition}";
                   
        }

        public override string GenersToString()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < BookGenres.Length; i++)
            {
                if (BookGenres[i] == BookGenre.Unknown)
                {
                    break;
                }
                    sb.Append($"{BookGenres[i]} ,");
            }
            if (sb.ToString().Length == 0)
            {
                return BookGenre.Unknown.ToString();
            }
            return sb.ToString().Remove(sb.ToString().Length -1);
        }
    }
}
