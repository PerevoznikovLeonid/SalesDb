using System;
using System.Collections.Generic;
using Dapper;
using Microsoft.Data.Sqlite;

namespace SalesDb;

public class DbContext(string connectionString)
{
    private readonly SqliteConnection _db = new(connectionString);
    
    public IEnumerable<ProductDto> GetStock()
    {
        _db.Open();
        var products = _db.Query<ProductDto>(
            "SELECT * FROM view_stock"
        );
        _db.Close();
        
        return products;
    }
    
    public ProductDto? GetProductById(int id)
    {
        return _db.QuerySingleOrDefault<ProductDto>(
            "SELECT id, name, quantity, price FROM table_products WHERE id = @id",
            new { id });
    }

    /// <summary>Возвращает персону (с контактами) по ID или null.</summary>
    public PersonDto? GetPersonById(int id)
    {
        return _db.QuerySingleOrDefault<PersonDto>(
            """
            SELECT p.id,
                   p.first_name AS FirstName,
                   p.last_name AS LastName, 
                   p.patronymic AS Patronymic,
                   pn.phone_number AS PhoneNumber, 
                   em.email AS Email,
                   p.address AS Address
                   FROM table_persons p
                   JOIN table_phone_numbers pn ON p.phone_number_id = pn.id
                   JOIN table_emails em ON p.email_id = em.id
                   WHERE p.id = @id
            """,
            new { id });
    }
    
    public bool TrySellProduct(ProductDto product, PersonDto seller, PersonDto customer,
        out string? errorMessage)
    {
        errorMessage = null;
        _db.Open();
        using var transaction = _db.BeginTransaction();
        try
        {
            var sellerExists = _db.ExecuteScalar<bool>(
                "SELECT COUNT(*) FROM table_persons WHERE id = @id",
                new { id = seller.Id }, transaction);
            if (!sellerExists)
            {
                errorMessage = $"Продавец с ID {seller.Id} не найден.";
                _db.Close();
                return false;
            }

            var customerExists = _db.ExecuteScalar<bool>(
                "SELECT COUNT(*) FROM table_persons WHERE id = @id",
                new { id = customer.Id }, transaction);
            if (!customerExists)
            {
                errorMessage = $"Покупатель с ID {customer.Id} не найден.";
                _db.Close();
                return false;
            }

            _db.Execute(
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
            _db.Close();
            errorMessage = $"Ошибка базы данных: {ex.Message}";
            return false;
        }
        catch (Exception ex)
        {
            _db.Close();
            errorMessage = $"Неизвестная ошибка: {ex.Message}";
            return false;
        }
    }
}