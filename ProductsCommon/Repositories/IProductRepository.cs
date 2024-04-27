using System.Collections.Generic;
using ProductsCommon.Models;

namespace ProductsCommon.Repositories
{
    public interface IProductRepository
    {
        void CreateOrUpdate(Product product);
        IEnumerable<Product> GetAllProducts();
        void UpdateDiscount((string, decimal) discount);
    }
}