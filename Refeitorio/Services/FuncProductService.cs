using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Refeitorio.Models;

namespace Refeitorio.Services
{
    public class FuncProductService
    {
        private List<Product> m_products = new List<Product>();
        public List<Product> GetAll() => m_products;

        public void AddProduct(Product product)
        {
            m_products.Add(product);
        }

        public List<Product> GetAllProducts()
        {
            return m_products;
        }

        public Product? GetById(int id)
        {   
            return m_products.FirstOrDefault(p => p.Id == id);
        }

        public void SerializeProducts()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Data", "Products.json");

            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(m_products, options);
            File.WriteAllText(filePath, json);
        }
    }
}
