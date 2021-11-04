using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using steimatzky.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace steimatzky.Data
{
    public enum ProductOperation
    {
        Unknow,
        Update,
        Add,
        Remove
    }
    public enum ProductStatus
    {
        Unknow,
        All,
        InStock,
        Remove,
        Sold
    }
    public class ProductRepo<T> : IRepository<BaseProduct>
    {
        private const string dir = @"E";
        private static string filePathAll;
        private List<BaseProduct> productsList;

        public ProductRepo()
        {
            filePathAll = Path.Combine(dir, "ProductsData.txt");
            productsList = GetData(ProductStatus.All);
        }

        public static bool SetData(List<BaseProduct> data)
        {
            string filePath = GetDataPath();
            try
            {
                using (var sw = new StreamWriter(filePath))
                {
                    sw.Write(JsonConvert.SerializeObject(data,
                             Formatting.Indented,
                             new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto }));

                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static string GetDataPath()
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return filePathAll;
        }

        public bool AddOrRemoveOrUpdate(BaseProduct item, ProductOperation opp, int quntityToRemove = 0)
        {
            bool isExist = false;
            if (opp == ProductOperation.Update)
            {
                return SetData(productsList);
            }
            foreach (var product in productsList)
            {
                if (product.Equals(item))
                {
                    isExist = true;

                    if (opp == ProductOperation.Add)
                    {
                        product.QuantityInStock += item.QuantityInStock;
                    }

                    if (opp == ProductOperation.Remove)
                    {
                        product.QuantityInStock -= quntityToRemove;
                        product.QuantityRemoved += quntityToRemove;
                    }
                }
            }
            if (!isExist)
            {
                productsList.Add(item);
            }
            SetData(productsList);
            return isExist;
        }// add or remove or update by the enum

        public List<BaseProduct> GetAll()
        {
            productsList = GetData(ProductStatus.All);
            return productsList;
        }

        public List<BaseProduct> GetData(ProductStatus status)
        {
            string filePath = GetDataPath();

            if (!File.Exists(filePath))
            {
                using (File.Create(filePath))
                    return new List<BaseProduct>();
            }
            else
            {
                List<BaseProduct> tmpBaseProducts = new List<BaseProduct>();
                using (var sr = new StreamReader(filePath))
                {
                    tmpBaseProducts = JsonConvert.DeserializeObject<List<BaseProduct>>(sr.ReadToEnd(), new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto
                    });
                }
                if (tmpBaseProducts == null) return new List<BaseProduct>();
                tmpBaseProducts = tmpBaseProducts.FindAll(p => IsQuntityExist(p, status));
                return tmpBaseProducts;
            }
        }

        private bool IsQuntityExist(BaseProduct product, ProductStatus status)
        {
            if (status == ProductStatus.All)
            {
                return true;
            }
            if (status == ProductStatus.InStock)
            {
                return (product.QuantityInStock > 0);
            }
            if (status == ProductStatus.Remove)
            {
                return (product.QuantityRemoved > 0);
            }
            return false;
        }

        public BaseProduct GetById(int id, ProductStatus status)// return a product if available by id if not return null
        {
            foreach (var item in productsList)
            {
                if (item.Id == id && status == ProductStatus.All)
                {
                    return item;
                }
                if (item.Id == id && IsQuntityExist(item, ProductStatus.InStock))
                {
                    return item;
                }
                if (item.Id == id && IsQuntityExist(item, ProductStatus.Remove))
                {
                    return item;
                }
            }
            return null;
        }

        public IEnumerable<BaseProduct> GetBy(Predicate<BaseProduct> func)
        {
            IEnumerable<BaseProduct> tmpProducts;
            tmpProducts = productsList.FindAll(func);
            return tmpProducts;
        }
    }
}
