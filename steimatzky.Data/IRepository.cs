using steimatzky.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace steimatzky.Data
{
    public interface IRepository<T>
    {
        bool AddOrRemoveOrUpdate(T item, ProductOperation opp, int quntityToRemove = 0);// add or remove or update by the enum

        List<T> GetAll();

        List<T> GetData(ProductStatus status);

        BaseProduct GetById(int id, ProductStatus status);// return a product if available by id if not return null

        IEnumerable<T> GetBy(Predicate<T> boolFunc);
    }
}
