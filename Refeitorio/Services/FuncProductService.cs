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
        private List<string> m_categories = new List<string>();
        public List<string> GetAllCategories() => m_categories.Distinct().OrderBy(c => c).ToList();
        public List<Product> GetAll() => m_products;
        private readonly string _filePath;
        private readonly string _categoriesFilePath;

        public FuncProductService()
        {
            var dataFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Data");
            Directory.CreateDirectory(Path.GetDirectoryName(dataFolder)!);
            _filePath = Path.Combine(dataFolder, "Products.json");
            _categoriesFilePath = Path.Combine(dataFolder, "Categories.json");
            LoadProductsFromFile();
            LoadCategoriesFromFile();
        }

        public void AddProduct(Product product)
        {
            m_products.Add(product);
            m_categories.Add(product.Category);
            SerializeCategories();
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


            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(m_products, options);
            File.WriteAllText(_filePath, json);
        }

        public void SerializeCategories()
        {
            var uniqueCategories = m_categories
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Distinct()
            .OrderBy(c => c)
            .ToList();

            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(uniqueCategories, options);
            File.WriteAllText(_categoriesFilePath, json);
        }

        public void LoadCategoriesFromFile()
        {
            if (!File.Exists(_categoriesFilePath))
            {
                m_categories = new List<string>();
                return;
            }

            var json = File.ReadAllText(_categoriesFilePath).Trim();
            if (string.IsNullOrWhiteSpace(json))
            {
                m_categories = new List<string>();
                return;
            }

            try
            {
                var loaded = JsonSerializer.Deserialize<List<string>>(json);
                m_categories = loaded ?? new List<string>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao carregar Categories.json: {ex.Message}");
                m_categories = new List<string>();
            }
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

        public bool TryDecrementStock(int productId, int quantity)
        {
            var product = m_products.FirstOrDefault(p => p.Id == productId);
            if (product == null || product.Stock < quantity)
                return false;

            product.Stock -= quantity;
            SerializeProducts();
            return true;
        }
    }
}
