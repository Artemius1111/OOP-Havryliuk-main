namespace IndependentWork17;

public static class SalesPipelines
{
    public static List<CategoryTop> TopProductsByCategory(List<Sale> sales, List<Product> products)
    {
        return sales
            .GroupBy(sale => sale.ProductId)
            .Select(group => new { ProductId = group.Key, Quantity = group.Sum(sale => sale.Quantity) })
            .Join(products,
                total => total.ProductId,
                product => product.Id,
                (total, product) => new { product.Name, product.Category, total.Quantity })
            .GroupBy(item => item.Category)
            .OrderBy(group => group.Key)
            .Select(group => new CategoryTop(
                group.Key,
                group
                    .OrderByDescending(item => item.Quantity)
                    .ThenBy(item => item.Name)
                    .Take(3)
                    .Select(item => new ProductSales(item.Name, item.Quantity))
                    .ToList()))
            .ToList();
    }

    public static List<CityRevenue> RevenueByCity(List<Sale> sales, List<Product> products, List<Customer> customers)
    {
        return sales
            .Join(products,
                sale => sale.ProductId,
                product => product.Id,
                (sale, product) => new { sale.CustomerId, Revenue = sale.Quantity * product.UnitPrice })
            .Join(customers,
                item => item.CustomerId,
                customer => customer.Id,
                (item, customer) => new { customer.City, item.Revenue })
            .GroupBy(item => item.City)
            .Select(group => new CityRevenue(group.Key, group.Sum(item => item.Revenue), group.Count()))
            .OrderByDescending(city => city.Revenue)
            .ToList();
    }

    public static List<MonthlySales> SalesByMonth(List<Sale> sales, List<Product> products)
    {
        return sales
            .Join(products,
                sale => sale.ProductId,
                product => product.Id,
                (sale, product) => new { sale.SaleDate, sale.Quantity, Revenue = sale.Quantity * product.UnitPrice })
            .GroupBy(item => new { item.SaleDate.Year, item.SaleDate.Month })
            .Select(group => new MonthlySales(
                group.Key.Year,
                group.Key.Month,
                group.Sum(item => item.Quantity),
                group.Sum(item => item.Revenue)))
            .OrderBy(month => month.Year)
            .ThenBy(month => month.Month)
            .ToList();
    }

    public static List<LoyalCustomer> LoyalCustomers(List<Sale> sales, List<Product> products, List<Customer> customers, int minActiveMonths, int top)
    {
        return sales
            .Join(products,
                sale => sale.ProductId,
                product => product.Id,
                (sale, product) => new
                {
                    sale.CustomerId,
                    sale.SaleDate,
                    sale.Quantity,
                    product.Category,
                    Revenue = sale.Quantity * product.UnitPrice
                })
            .GroupBy(item => item.CustomerId)
            .Select(group => new
            {
                CustomerId = group.Key,
                ActiveMonths = group.Select(item => new { item.SaleDate.Year, item.SaleDate.Month }).Distinct().Count(),
                TotalSpent = group.Sum(item => item.Revenue),
                FavoriteCategory = group
                    .GroupBy(item => item.Category)
                    .OrderByDescending(categoryGroup => categoryGroup.Sum(item => item.Quantity))
                    .ThenBy(categoryGroup => categoryGroup.Key)
                    .First()
                    .Key
            })
            .Where(item => item.ActiveMonths >= minActiveMonths)
            .Join(customers,
                item => item.CustomerId,
                customer => customer.Id,
                (item, customer) => new LoyalCustomer(customer.Name, customer.City, item.ActiveMonths, item.TotalSpent, item.FavoriteCategory))
            .OrderByDescending(customer => customer.TotalSpent)
            .Take(top)
            .ToList();
    }
}
