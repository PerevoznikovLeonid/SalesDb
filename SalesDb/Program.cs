using System;
using SalesDb;

const string connectionString = @"Data Source=C:\Users\User\RiderProjects\SalesDb\SalesDb\sales.db;";

var dbContext = new DbContext(connectionString);

Console.WriteLine("Начальные остатки:");
var stock = dbContext.GetStock();
foreach (var productDto in stock)
{
    Console.WriteLine(productDto);
}