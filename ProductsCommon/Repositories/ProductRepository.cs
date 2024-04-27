using ProductsCommon.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ProductsCommon.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private string connectionString;
        public ProductRepository(SystemSettings settings)
        {
            connectionString = settings.connectionString;
        }

        public IEnumerable<Product> GetAllProducts()
        {

            var products = new List<Product>();

            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    var command = new SqlCommand("SELECT ProductID, ProductName, Price, Description FROM Products", connection);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var product = new Product
                            {
                                ProductID = reader.GetInt32(reader.GetOrdinal("ProductID")),
                                ProductName = reader.IsDBNull(reader.GetOrdinal("ProductName")) ? null : reader.GetString(reader.GetOrdinal("ProductName")),
                                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description"))
                            };
                            products.Add(product);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        connection.Close();
                        connection.Dispose();
                    }
                }
            }

            return products;
        }

        public void CreateOrUpdate(Product product)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    var command = connection.CreateCommand();
                    command.Transaction = transaction;

                    try
                    {
                        command.CommandText = "SELECT top 1 1 FROM Products WHERE ProductID = @ProductID";
                        command.Parameters.AddWithValue("@ProductID", product.ProductID);

                        var exists = (int)command.ExecuteScalar() > 0;


                        if (exists)
                        {
                            command.CommandText = "UPDATE Products " +
                                "SET ProductName = @ProductName, " +
                                "Price = @Price, " +
                                "Description = @Description " +
                                "WHERE ProductID = @ProductID";
                            command.Parameters.AddWithValue("@ProductID", product.ProductID);
                        }
                        else
                        {
                            command.CommandText = "INSERT INTO Products " +
                                "(ProductName, Price, Description) " +
                                "VALUES (@ProductName, @Price, @Description)";
                            
                        }

                        command.Parameters.AddWithValue("@ProductName", product.ProductName);
                        command.Parameters.AddWithValue("@Description", product.Description);
                        command.Parameters.AddWithValue("@Price", product.Price);

                        command.ExecuteNonQuery();
                        transaction.Commit();

                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                    finally
                    {
                        if (connection.State == System.Data.ConnectionState.Open)
                        {
                            connection.Close();
                            connection.Dispose();
                        }
                    }
                }
            }
        }

        public void UpdateDiscount((string,decimal) discount)
        {
            if (discount.Item2 <= 0)
                return;

            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();


                    string dicQuery =
                        discount.Item1 == "Percent" ? $"Price * (1 -{discount.Item2}/100)" 
                                                    : $"Price - {discount.Item2}";


                    var command = new SqlCommand($"UPDATE Products SET Price = {dicQuery}", connection);

                    command.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        connection.Close();
                        connection.Dispose();
                    }
                }
            }
        }

    }
}

