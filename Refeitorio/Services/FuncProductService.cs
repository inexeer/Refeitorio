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
        private readonly string _filePath;

        public FuncProductService()
        {
            _filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Data", "Products.json");
            Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
            LoadProductsFromFile();
        }

        public void AddProduct(Product product)
        {
            m_products.Add(product);
            SerializeProducts();
        }

        public void DeleteProduct(Product product)
        {
            m_products.Remove(product);
            SerializeProducts();
        }

        public void UpdateProduct(Product updatedProduct)
        {
            var index = m_products.FindIndex(p => p.Id == updatedProduct.Id);
            if (index != -1)
            {
                m_products[index] = updatedProduct;
                SerializeProducts();
            }
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
            //var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Data", "Products.json");

            //Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(m_products, options);
            File.WriteAllText(_filePath, json);
        }

        public void LoadProductsFromFile()
        {
            

            if (!File.Exists(_filePath))
                return;

            var json = File.ReadAllText(_filePath).Trim();

            if (string.IsNullOrWhiteSpace(json))
            {
                m_products = new List<Product>();
                return;
            }

            try
            {
                var loaded = JsonSerializer.Deserialize<List<Product>>(json);
                m_products = loaded ?? new List<Product>();
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Corrupted Products.json: {ex.Message}");
                m_products = new List<Product>();
            }
        }
    }
}
