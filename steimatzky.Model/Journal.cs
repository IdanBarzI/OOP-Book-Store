using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace steimatzky.Model
{
    public enum JournalGenre
    {
        Unknown,
        NewsArticles,
        Features,
        Portraits,
        Reportages,
        Interviews,
        Editorials,
        Columns,
        Reviews,
        Essays
    }
    [Serializable]
    public class Journal : BaseProduct
    {
        private JournalGenre[] _journalGenres;

        public JournalGenre[] JournalGenres { get { return _journalGenres; } set { _journalGenres = value; } }

        public Journal(string name, double productPrice, DateTime printDate, int quntity, ProductType productType, JournalGenre[] journalGenres)
        : base(name, productPrice, printDate, quntity, productType)
        {
            if (journalGenres == null)
            {
                _journalGenres = new JournalGenre[1];
                _journalGenres[0] = JournalGenre.Unknown;
            }
            else
                _journalGenres = journalGenres;
        }

        public override string ToString()
        {
            return $"Product type: {ProductType}, Name: {Name}\n" +
                   $"Price:{ProductPrice:C}, Id {Id}\n" +
                   $"Print date:{PrintDate}, Adding date:{AddingDate}\n" +
                   $"Quntity:{QuantityInStock},Quntity removed:{QuantityRemoved}\n"+
                   $"Ganres: {GenersToString()}";
        }

        public override string GenersToString()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < JournalGenres.Length; i++)
            {
                if (JournalGenres[i] == JournalGenre.Unknown)
                {
                    break;
                }
                        sb.Append($"{JournalGenres[i]} ,");
            }

            if (sb.ToString().Length == 0)
            {
                return JournalGenre.Unknown.ToString();
            }
            return sb.ToString().Remove(sb.ToString().Length - 1);
        }
    }
}
