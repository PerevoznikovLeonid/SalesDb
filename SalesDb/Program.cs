using System;
using System.Collections.Generic;
using SalesDb;
using SQLitePCL;

const string connectionString = @"Data Source=C:\Users\User\RiderProjects\SalesDb\SalesDb\sales.db;";

Batteries.Init();
var dbContext = new DbContext(connectionString);

PrintStock(dbContext.GetStock());

var product = dbContext.GetProductById(1);
var seller = dbContext.GetPersonById(1);
var customer = dbContext.GetPersonById(2);
var outOfStockProduct = dbContext.GetProductById(3);

if (product != null && seller != null && customer != null && outOfStockProduct != null)
{
    Console.WriteLine(dbContext.SellProduct(product, seller, customer,
        out var msg1)
        ? "Успешно." : msg1);

    Console.WriteLine(dbContext.SellProduct(product, seller, customer,
        out var msg2)
        ? "Успешно."
        : msg2);
    
    Console.WriteLine(dbContext.SellProduct(outOfStockProduct, seller, customer,
        out var msg3)
        ? "Успешно."
        : msg3);

    PrintStock(dbContext.GetStock());
}
else
{
    Console.WriteLine("Данные не найдены.");
}

return;

void PrintStock(IEnumerable<ProductDto> stock)
{
    Console.WriteLine("Остатки:");
    foreach (var productDto in stock)
    {
        Console.WriteLine(productDto);
    }
}