using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NortwindApi.Models;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace NortwindApi.Controllers
{
    [ApiController]
    public class NortwindController : ControllerBase
    {
        NortwindContext _context;
        public NortwindController(NortwindContext nortwindContext) => _context = nortwindContext;

        [HttpGet("TopSelling3")]
        public IActionResult TopSelling3()
        {
            var topShipCountries = _context.Orders
     .GroupBy(o => o.ShipCountry)
     .Select(group => new
     {
         ShipCountry = group.Key,
         OrderCount = group.Count()
     })
     .OrderByDescending(result => result.OrderCount)
     .Take(3)
     .ToList();

            return Ok(topShipCountries);
        }

        [HttpGet("GetBeveragesCategories")]
        public IActionResult GetBeveragesCategories()
        {
            var productsInCategory1 = _context.Products
           .Where(p => p.CategoryId == 1)
           .OrderBy(p => p.ProductId)
           .Select(p => new
           {
               p.ProductId,
               p.ProductName,
               p.SupplierId,
               p.QuantityPerUnit,
               p.UnitPrice,
               p.UnitsInStock,
               p.UnitsOnOrder,
               p.ReorderLevel,
               p.Discontinued
           })
           .ToList();

            return Ok(productsInCategory1);
        }

        [HttpGet("TotalRevenues1917")]
        public IActionResult TotalRevenues1917()
        {
            var totalRevenues = _context.ProductSalesFor1997s
            .Sum(ps => ps.ProductSales);

            return Ok(totalRevenues);
        }

        [HttpGet("GetTop3Suppliers")]
        public IActionResult GetTop3Suppliers()
        {
            var topSuppliers = _context.Suppliers
    .Join(
        _context.AlphabeticalListOfProducts,
        s => s.SupplierId,
        p => p.SupplierId,
        (s, p) => new
        {
            SupplierId = s.SupplierId,
            CompanyName = s.CompanyName,
            TotalRevenue = p.UnitPrice * p.UnitsInStock
        }
    )
    .GroupBy(result => new { result.SupplierId, result.CompanyName })
    .Select(group => new
    {
        group.Key.SupplierId,
        group.Key.CompanyName,
        TotalRevenue = group.Sum(result => result.TotalRevenue)
    })
    .OrderByDescending(result => result.TotalRevenue)
    .Take(3)
    .ToList();

            return Ok(topSuppliers);
        }

        [HttpGet("ShipperOrderCounts")]
        public IActionResult ShipperOrderCounts()
        {

            var shipperOrderCounts = _context.Shippers
    .Join(
        _context.Orders,
        s => s.ShipperId,
        o => o.ShipVia,
        (s, o) => new
        {
            ShipperId = s.ShipperId,
            CompanyName = s.CompanyName,
            ShipVia = o.ShipVia
        }
    )
    .GroupBy(result => new { result.ShipperId, result.CompanyName })
    .Select(group => new
    {
        group.Key.ShipperId,
        group.Key.CompanyName,
        OrderCount = group.Count()
    })
    .OrderByDescending(result => result.OrderCount)
    .ToList();

            return Ok(shipperOrderCounts);
        }

        [HttpGet("CustomerOrdersLeast15")]
        public IActionResult CustomerOrdersLeast15()
        {
            var customersWithTotalOrderCount = _context.Customers
            .Join(
                _context.Orders,
                c => c.CustomerId,
                o => o.CustomerId,
                (c, o) => new
                {
                    CustomerId = c.CustomerId,
                    ContactName = c.ContactName,
                    CompanyName = c.CompanyName,
                    City = c.City,
                    OrderCount = o.CustomerId
                }
            )
            .GroupBy(result => new { result.CustomerId, result.ContactName, result.CompanyName, result.City })
            .Where(group => group.Count() >= 15)
            .Select(group => new
            {
                CustomerId = group.Key.CustomerId,
                ContactName = group.Key.ContactName,
                CompanyName = group.Key.CompanyName,
                City = group.Key.City,
                TotalOrderCount = group.Count()
            })
            .OrderByDescending(result => result.TotalOrderCount)
            .ToList();

            return Ok(customersWithTotalOrderCount);
        }

        [HttpGet("CustomerNameAfter1917")]
        public IActionResult CustomerNameAfter1917()
        {

            var customersWithRecentOrders = _context.Customers
        .Join(
            _context.Orders,
            c => c.CustomerId,
            o => o.CustomerId,
            (c, o) => new
            {
                CustomerId = c.CustomerId,
                ContactName = c.ContactName,
                OrderDate = o.OrderDate
            }
        )
        .Where(result => result.OrderDate > new DateTime(1998, 1, 1))
        .OrderBy(result => result.OrderDate)
        .Select(result => new
        {
            result.CustomerId,
            result.ContactName,
            result.OrderDate
        })
        .ToList();

            return Ok(customersWithRecentOrders);

        }
        [HttpGet("ShippingWithFederal")]
        public IActionResult ShippingWithFederal()
        {
            try
            {
                var query = from s in _context.Shippers
                            join o in _context.Orders on s.ShipperId equals o.ShipVia
                            where s.ShipperId == 3 && o.ShipVia == 3
                            select s;


                // JSON döngüsel referansları yönetmek için JsonSerializerOptions kullanın
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                    MaxDepth = 32
                };

                // JSON olarak serileştirilmiş veriyi döndürün
                return Ok(query);
            }
            catch (Exception ex)
            {
                // Hata durumunda 500 kodu ile cevap oluşturun
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        [HttpGet("Steven97Report")]
        public IActionResult Steven97Report()
        {
            try
            {
                var orders = _context.Orders
                    .Where(o => o.EmployeeId == 5 && o.OrderDate.Value.Year == 1997 && o.OrderDate.Value.Month == 03)
                    .ToList();

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }

        }

        [HttpGet("SpeedyOrderNuncyAlfki")]
        public IActionResult SpeedyExpress()
        {

            var speedy = _context.Orders.Where(t => t.EmployeeId == 1 && t.ShipVia == 1).ToList();

            return Ok(speedy);
        }


        [HttpGet("GetGermanyCustomer")]

        public IActionResult GetGermanyCustomer()
        {


            return Ok(_context.Customers.Where(i => i.Country == "Germany").ToList());

        }

        [HttpGet("Seafood")]

        public IActionResult Seafood()
        {
            var query = from p in _context.Products
                        join s in _context.Suppliers on p.SupplierId equals s.SupplierId
                        join c in _context.Categories on p.CategoryId equals c.CategoryId
                        join d in _context.OrderDetails on p.ProductId equals d.ProductId
                        join o in _context.Orders on d.OrderId equals o.OrderId
                        join e in _context.Employees on o.EmployeeId equals e.EmployeeId
                        where c.CategoryId != 8 && s.PostalCode == "33007"
                        select new
                        {
                            EmployeeInfo = new
                            {
                                FirstName = e.FirstName,
                                LastName = e.LastName,
                                HomePhone = e.HomePhone
                            }
                        };

            var distinctEmployeeInfo = query.Distinct().ToList();

            return Ok(distinctEmployeeInfo);


        }

        [HttpGet("SpeedyExpWithNancy")]


        public IActionResult SpeedyExpWithN()
        {

            var query = from o in _context.Orders
                        join sh in _context.Shippers on o.ShipVia equals sh.ShipperId
                        join emp in _context.Employees on o.EmployeeId equals emp.EmployeeId
                        join cst in _context.Customers on o.CustomerId equals cst.CustomerId
                        where (sh.ShipperId == 1 && emp.EmployeeId == 1 && (cst.CustomerId == "ALFKI" || cst.CustomerId == "DUMON"))
                        select o;

            var orders = query.ToList();

            return Ok(orders);


        }

        public class CategoryViewModel
        {
            public int CategoryId { get; set; }
            public string CategoryName { get; set; }
        }

        [HttpGet("Eastren")]
        public IActionResult Eastren()
        {
            using (var context = new NortwindContext())
            {
                var query = from cts in context.Categories
                            join prd in context.Products on cts.CategoryId equals prd.CategoryId
                            join ordt in context.OrderDetails on prd.ProductId equals ordt.ProductId
                            join ord in context.Orders on ordt.OrderId equals ord.OrderId
                            join cst in context.Customers on ord.CustomerId equals cst.CustomerId
                            join emp in context.Employees on ord.EmployeeId equals emp.EmployeeId
                            join empt in context.EmployeeTerritories on emp.EmployeeId equals empt.EmployeeId
                            join tr in context.Territories on empt.TerritoryId equals tr.TerritoryId
                            join r in context.Regions on tr.RegionId equals r.RegionId
                            join shp in context.Shippers on ord.ShipVia equals shp.ShipperId
                            where r.RegionDescription == "Eastern" && shp.ShipperId == 3
                            select new{
                    cts.CategoryId,cts.CategoryName
                }
                    ;

                var categories = query.ToList();
  

                return Ok(categories);
            }
           
        }

        [HttpGet("ShippersLondon")]
        public IActionResult ShippersLondon()
        {
            using (var context = new NortwindContext())
            {
                var query = from o in context.Orders
                            join s in context.Shippers on o.ShipVia equals s.ShipperId
                            join odt in context.OrderDetails on o.OrderId equals odt.OrderId
                            join prd in context.Products on odt.ProductId equals prd.ProductId
                            join spp in context.Suppliers on prd.SupplierId equals spp.SupplierId
                            where spp.City == "London" && s.CompanyName.EndsWith("e")
                                  && prd.UnitsInStock > 0 && prd.UnitPrice >= 10 && prd.UnitPrice <= 60
                            select new
                            {
                                s.CompanyName,
                                spp.City,
                                prd.UnitsInStock,
                                prd.UnitPrice,
                                o
                            };

                var results = query.ToList();
                return Ok(results);
            }
        }

        [HttpGet("DiscontinuedSale")]
        public IActionResult DiscontinuedSale()
        {
            using (var context = new NortwindContext())
            {
                var query = from prd in context.Products
                            join s in context.Suppliers on prd.SupplierId equals s.SupplierId
                            where prd.Discontinued && prd.UnitsInStock == 0
                            select new
                            {
                                s.ContactName,
                                s.Phone
                            };

                var results = query.ToList();
                return Ok(results);
            }
        }

        [HttpGet("NewYorkManager")]

        public IActionResult NewYorkManager() {

            using (var context = new NortwindContext())
            {
                var query = from t in context.Territories
                            join empt in context.EmployeeTerritories on t.TerritoryId equals empt.TerritoryId
                            join emp in context.Employees on empt.EmployeeId equals emp.EmployeeId
                            where t.TerritoryDescription == "New York"
                            select new
                            {
                                emp.EmployeeId,
                                emp.FirstName,
                                emp.LastName,
                                emp.Address,
                                emp.Country,
                                emp.City,
                                emp.BirthDate,
                                emp.PhotoPath
                            };

                var employees = query.ToList();
                return Ok(employees);
            }
        }

        [HttpGet("OrderLast1998")]

        public IActionResult OrderLast1998() {
            using (var context = new NortwindContext())
            {
                var query = from o in context.Orders
                            join c in context.Customers on o.CustomerId equals c.CustomerId
                            where o.OrderDate > new DateTime(1998, 1, 1)
                            orderby o.OrderDate ascending
                            select new
                            {
                                o.OrderDate,
                                c.ContactName
                            };

                var results = query.ToList();
                return Ok(results);
            }

        }

        [HttpPost("MakeRaise/{discountFactor}")]
        public IActionResult MakeRaise(decimal discountFactor)
        {
            try
            {
                var products = _context.Products.ToList();

                foreach (var product in products)
                {
                    product.UnitPrice = product.UnitPrice + (product.UnitPrice * discountFactor);
                }

                _context.SaveChanges();

                return Ok($"Up to  % {discountFactor}  raise successfully done and saved");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        //ismi verilen ürünün fiyatına dışarıdan gelen sayı kadar zam yap
        [HttpPost("UpdatePrice/{productName}/{increaseAmount}")]
        public IActionResult UpdatePrice(string productName, decimal increaseAmount)
        {
            try
            {
                var product = _context.Products.FirstOrDefault(p => p.ProductName == productName);

                if (product == null)
                {
                    return NotFound($"Ürün adı '{productName}' ile eşleşen ürün bulunamadı.");
                }

                product.UnitPrice += increaseAmount;

                _context.SaveChanges();

                return Ok($"Ürün '{productName}' fiyatına {increaseAmount} kadar zam yapıldı.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Hata: {ex.Message}");
            }
        }
        //dışarıdan girilen kargo firmasıyla taşınmış, kargo ücreti 30 dan yüksek olan ve dışarıdan girilen yıla ait olan siparişlerin bilgilerini raporla
        [HttpPost("ReportFreight")]
        public IActionResult ReportFreight([FromBody] ReportFilter filtre)
        {
            try
            {
                var query = from o in _context.Orders
                            join s in _context.Shippers on o.ShipVia equals s.ShipperId
                            where s.CompanyName == filtre.Shipment
                                  && o.Freight > filtre.MinShippingCost
                                  && o.OrderDate.Value.Year == filtre.OrderYear
                            select new
                            {
                                o.OrderId,
                                o.ShipVia,
                                o.Freight,
                                o.OrderDate
                            };

                var results = query.ToList();

                return Ok(results);
            }
            catch (Exception ex)
            {
                return BadRequest($"Hata: {ex.Message}");
            }
        }

        [HttpGet("Tacoma")]
        public IActionResult Tacoma()
        {
            var query = from o in _context.Orders
                        join emp in _context.Employees on o.EmployeeId equals emp.EmployeeId
                        where emp.City == "Tacoma"
                        select o;

            var results = query.ToList();   

            return Ok(results); 
        }

        [HttpGet("BesSellerProduct")]

        public IActionResult BesSellerProduct()
        {
            using (var context = new NortwindContext())
            {
                var query = from od in context.OrderDetails
                            join p in context.Products on od.ProductId equals p.ProductId
                            join c in context.Categories on p.CategoryId equals c.CategoryId
                            group od by new { p.ProductId, p.ProductName, c.CategoryId, c.CategoryName } into g
                            orderby g.Sum(x => x.Quantity) descending
                            select new
                            {
                                ProductId = g.Key.ProductId,
                                ProductName = g.Key.ProductName,
                                NumberOfOrders = g.Count(),
                                CategoryId = g.Key.CategoryId,
                                CategoryName = g.Key.CategoryName,
                                TotalQuantityOrdered = g.Sum(x => x.Quantity)
                            };

                var result = query.FirstOrDefault();
                return Ok(result);
            }
        }

        [HttpGet("ListBestOrders")]

        public IActionResult ListBestOrders()
        {
            var query = from od in _context.OrderDetails
                        join pr in _context.Products on od.ProductId equals pr.ProductId
                        group od by new { pr.ProductId, pr.ProductName } into g
                        orderby g.Sum(t => t.Quantity) descending
                        select new
                        {
                            ProductId = g.Key.ProductId,
                            ProductName = g.Key.ProductName,
                            NumberOfOrders = g.Count(),
                            TotalQuantityOrdered = g.Sum(x => x.Quantity)
                        };

                var result = query.ToList();

            return Ok(result);
        }


        [HttpGet("NancysProducts")]
        public IActionResult NancysProducts()
        {
            var result = (from o in _context.Orders
                          join e in _context.Employees on o.EmployeeId equals e.EmployeeId
                          join od in _context.OrderDetails on o.OrderId equals od.OrderId
                          join p in _context.Products on od.ProductId equals p.ProductId
                          join c in _context.Categories on p.CategoryId equals c.CategoryId
                          join cm in _context.Customers on o.CustomerId equals cm.CustomerId

                          where e.FirstName == "Nancy"
                          select new
                          {
                              firstName = e.FirstName,
                              productName = p.ProductName,
                              categoryName = c.CategoryName,
                              customerName = cm.ContactName
                          }).ToList();


            return Ok(result);
        }


        [HttpGet("HighAverageSales")]
        public IActionResult HighAverageSales()
        {
            var highAverageSales = _context.OrderDetails
             .Join(
                 _context.Orders,
                 od => od.OrderId,
                 o => o.OrderId,
                 (od, o) => new { OrderDetail = od, Order = o }
             )
             .Where(result => result.OrderDetail.Quantity > _context.OrderDetails.Average(od => od.Quantity))
             .Select(result => new
             {
                 result.OrderDetail.Quantity,
                 result.OrderDetail.UnitPrice,
                 result.Order.OrderId,
                 result.Order.CustomerId,
                 result.Order.EmployeeId,
                 result.Order.OrderDate,
                 result.Order.RequiredDate,
                 result.Order.ShippedDate,
                 result.Order.ShipVia,
                 result.Order.Freight,
                 result.Order.ShipName,
                 result.Order.ShipAddress,
                 result.Order.ShipCity,
                 result.Order.ShipRegion,
                 result.Order.ShipPostalCode,
                 result.Order.ShipCountry
             })
             .ToList();

            return Ok(highAverageSales);
        }

        [HttpGet("USASuplliersBeverages")]
        public IActionResult USASuplliersBeverages()
        {
            var usaBeveragesProducts = (from p in _context.Products
                                        join c in _context.Categories on p.CategoryId equals c.CategoryId
                                        join s in _context.Suppliers on p.SupplierId equals s.SupplierId
                                        where c.CategoryName == "Beverages" && p.UnitsInStock > 0 && s.Country == "USA"
                                        select new
                                        {
                                            ProductName = p.ProductName,
                                            KDV = Math.Round(Convert.ToDouble(p.UnitPrice) * 1.2, 2),
                                            UnitPrice = p.UnitPrice,
                                            UnitsInStock = p.UnitsInStock,
                                            SupplierId = p.SupplierId,
                                            CategoryName = c.CategoryName,
                                            Country = s.Country

                                        }).ToList();
            return Ok(usaBeveragesProducts);
        }

        [HttpGet("ProductTypeCount")]
        public IActionResult ProductTypeCount()
        {
            var productTypeCounts = (from od in _context.OrderDetails
                                     join sa in _context.SalesTotalsByAmounts on od.OrderId equals sa.OrderId
                                     group new { od, sa } by new { od.OrderId, sa.ShippedDate } into grouped
                                     orderby grouped.Key.OrderId
                                     select new
                                     {
                                         OrderId = grouped.Key.OrderId,
                                         DifferentProductCount = grouped.Select(g => g.od.ProductId).Distinct().Count(),
                                         TotalProductCount = grouped.Sum(g => g.od.Quantity),
                                         ShippedDate = grouped.Key.ShippedDate
                                     }).ToList();

            return Ok(productTypeCounts);

        }

        public class ReportFilter
        {
            public string Shipment { get; set; }
            public decimal MinShippingCost { get; set; }
            public int OrderYear { get; set; }
        }
    }



}
