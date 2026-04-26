using System;
using System.Collections.Generic;
using Dapper;
using Microsoft.Data.Sqlite;

namespace SalesDb;

public class DbContext(string connectionString)
{
    private readonly string _connectionString = connectionString;
    
    public IEnumerable<ProductDto> GetStock()
    {
        using var db = new SqliteConnection(_connectionString);
        db.Open();
        var products = db.Query<ProductDto>(
            "SELECT * FROM view_stock"
        );
        db.Close();
        
        return products;
    }
    
    public ProductDto? GetProductById(int id)
    {
        using var db = new SqliteConnection(_connectionString);
        db.Open();
        var result = db.QuerySingleOrDefault<ProductDto>(
            "SELECT id, name, quantity, price FROM table_products WHERE id = @id",
            new { id });
        db.Close();
        return result;
    }
    
    public PersonDto? GetPersonById(int id)
    {
        using var db = new SqliteConnection(_connectionString); 
        db.Open();
        var result = db.QuerySingleOrDefault<PersonDto>(
            """
            SELECT p.id,
                   p.first_name AS FirstName,
                   p.last_name AS LastName, 
                   p.patronymic AS Patronymic,
                   pn.phone_number AS PhoneNumber, 
                   em.email AS Email,
                   p.address AS Address
                   FROM table_persons p
                   LEFT JOIN table_phone_numbers pn ON p.phone_number_id = pn.id
                   LEFT JOIN table_emails em ON p.email_id = em.id
                   WHERE p.id = @id
            """,
            new { id });
        db.Close();
        return result;
    }
    
    public bool SellProduct(ProductDto product, PersonDto seller, PersonDto customer,
        out string? errorMessage)
    {
        errorMessage = null;
        using var db = new SqliteConnection(_connectionString);
        db.Open();
        using var transaction = db.BeginTransaction();
        try
        {
            var sellerExists = db.ExecuteScalar<bool>(
                "SELECT COUNT(*) FROM table_persons WHERE id = @id",
                new { id = seller.Id }, transaction);
            if (!sellerExists)
            {
                errorMessage = $"Продавец с ID {seller.Id} не найден.";
                db.Close();
                return false;
            }

            var customerExists = db.ExecuteScalar<bool>(
                "SELECT COUNT(*) FROM table_persons WHERE id = @id",
                new { id = customer.Id }, transaction);
            if (!customerExists)
            {
                errorMessage = $"Покупатель с ID {customer.Id} не найден.";
                db.Close();
                return false;
            }

            db.Execute(
                """
                INSERT INTO table_sales (product_id, seller_id, customer_id, date)
                                  VALUES (@prodId, @sellerId, @custId, @date)
                """,
                new
                {
                    prodId = product.Id,
                    sellerId = seller.Id,
                    custId = customer.Id,
                    date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                },
                transaction);

            transaction.Commit();
            errorMessage = null;
            return true;
        }
        catch (SqliteException ex)
        {
            db.Close();
            errorMessage = $"Ошибка базы данных: {ex.Message}";
            return false;
        }
        catch (Exception ex)
        {
            db.Close();
            errorMessage = $"Неизвестная ошибка: {ex.Message}";
            return false;
        }
    }
}