using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace steimatzky.Model
{
    public enum ProductType
    {
        Unknown,
        Book,
        Journal
    }
    [Serializable]
    public abstract class BaseProduct
    {
        private ProductType _productType;
        private string _name;
        private double _productPrice;
        private int _id;
        private DateTime _printDate;
        private DateTime _addingDate = DateTime.MinValue;
        private int _quantityIntStock;
        private int _quantityRemoved;
        private Guid _isbn = Guid.Empty;

        public int Id { get { return _id; } set { if (_id == 0) _id = value; } }
        public string Name { get { return _name; } set { _name = value; } }
        public double ProductPrice { get { return _productPrice; } set { _productPrice = value; } }
        public DateTime PrintDate { get { return _printDate; } set { _printDate = value; } }
        public DateTime AddingDate { get { return _addingDate; } set { if (_addingDate == DateTime.MinValue) _addingDate = value; } }
        public int QuantityInStock { get { return _quantityIntStock; } set { _quantityIntStock = value; } }
        public int QuantityRemoved { get { return _quantityRemoved; } set { _quantityRemoved = value; } }
        public ProductType ProductType { get { return _productType; } set { _productType = value; } }
        public Guid ISBN { get { return _isbn; } set {if (this.ProductType == ProductType.Book && _isbn == Guid.Empty) _isbn = value;} }

        public BaseProduct(string name, double productPrice, DateTime printDate, int quantity, ProductType productType)
        {
            _productType = productType;
            _name = name;
            _productPrice = productPrice;
            _printDate = printDate;
            _quantityIntStock = quantity;
            _quantityRemoved = 0;
        }

        public override string ToString()
        {
            string res = $"Product name: {Name}, Product price:{ProductPrice:C}, Product id {Id}\n" +
                   $"Product print date:{PrintDate}, Product quntity in stock:{QuantityInStock}\n" +
                   $"Product quntity removed:{QuantityRemoved}, Product adding date:{AddingDate}";

            if (QuantityInStock <= 0)
            {
                res += "NOT IN STOCK";
            }

            return res;
        }

        public override sealed bool Equals(object obj)
        {
            if (!(obj is BaseProduct)) return false;
            BaseProduct tmp = obj as BaseProduct;
            return (this.Name == tmp.Name && this.ProductPrice == tmp.ProductPrice);//ignore the id for new books
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public abstract string GenersToString();
    }
}
