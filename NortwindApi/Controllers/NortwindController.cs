using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NortwindApi.Models;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Serialization;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Diagnostics.Metrics;
using System.Numerics;
using System.Security.Cryptography.Xml;

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
            try
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

                if (topShipCountries == null || topShipCountries.Count == 0)
                {
                    return NotFound("Veri bulunamadı.");
                }

                return Ok(topShipCountries);
            }
            catch (Exception ex)
            {
                return BadRequest($"Hata: {ex.Message}");
            }
        }

        [HttpGet("GetBeveragesCategories")]
        public IActionResult GetBeveragesCategories()
        {
            try
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
                if (productsInCategory1 == null || productsInCategory1.Count == 0)
                {
                    return NotFound("Veri bulunamadı.");
                }
                return Ok(productsInCategory1);
            }
            catch (Exception ex)
            {
                return BadRequest($"Hata: {ex.Message}");
            }
        }

    

        [HttpGet("GetTop3Suppliers")]
        public IActionResult GetTop3Suppliers()
        {
            try
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

                if (topSuppliers == null || topSuppliers.Count == 0)
                {
                    return NotFound("Veri bulunamadı.");
                }

                return Ok(topSuppliers);
            }
            catch (Exception ex)
            {
                return BadRequest($"Hata: {ex.Message}");
            }
        }

        [HttpGet("ShipperOrderCounts")]
        public IActionResult ShipperOrderCounts()
        {
            try
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

                if (shipperOrderCounts == null || shipperOrderCounts.Count == 0)
                {
                    return NotFound("Veri bulunamadı.");
                }

                return Ok(shipperOrderCounts);
            }
            catch (Exception ex)
            {
                return BadRequest($"Hata: {ex.Message}");
            }
        }

        [HttpGet("CustomerOrdersLeast15")]
        public IActionResult CustomerOrdersLeast15()
        {
            try
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


                if (customersWithTotalOrderCount == null || customersWithTotalOrderCount.Count == 0)
                {
                    return NotFound("Veri bulunamadı.");
                }


                return Ok(customersWithTotalOrderCount);
            }
            catch (Exception ex)
            {
                return BadRequest($"Hata: {ex.Message}");
            }
        }



        [HttpGet("ShippingWithFederal")]
        public IActionResult ShippingWithFederal()
        {
            try
            {
                var query = from s in _context.Shippers                            
                            join o in _context.Orders on s.ShipperId equals o.ShipVia
                            join emp in _context.Employees on o.EmployeeId equals emp.EmployeeId
                            join cs in _context.Customers on o.CustomerId equals cs.CustomerId
                            where s.ShipperId == 3 && o.ShipVia == 3
                            select new
                            {
                                orderId = o.OrderId,
                                customer = cs.CompanyName,
                                employee = emp.FirstName,
                                orderDate = o.OrderDate.Value.ToString("dd/MM/yyyy"),
                                shippedDate = o.ShippedDate,
                                shipVia = s.CompanyName,
                                freight = o.Freight,
                                shipName = o.ShipName,
                                shipCountry = o.ShipCountry,
                                shipAddress = o.ShipAddress,
                                shipCity = o.ShipCity,
                                shipPostalCode = o.ShipPostalCode
                                
                            };
                          


                // JSON döngüsel referansları yönetmek için JsonSerializerOptions kullanın
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                    MaxDepth = 32
                };

                if (query == null)
                {
                    return NotFound("Veri bulunamadı.");
                }
                // JSON olarak serileştirilmiş veriyi döndürün
                return Ok(query);
            }
            catch (Exception ex)
            {
                return BadRequest($"Hata: {ex.Message}");
            }
        }

        [HttpGet("Steven97Report")]
        public IActionResult Steven97Report()
        {
            try
            {
            

                var query = from o in _context.Orders
                            join em in _context.Employees on o.EmployeeId equals em.EmployeeId
                            join sh in _context.Shippers on o.ShipVia equals sh.ShipperId
                            where o.EmployeeId == 5 && o.OrderDate.Value.Year == 1997 && o.OrderDate.Value.Month == 03
                            select new
                            {
                                orderID = o.OrderId,
                                employeeName = em.FirstName,
                                orderDate = o.OrderDate.Value.ToString("dd/MM/yyyy"),
                                shippedDate = o.ShippedDate,
                                shipVia = sh.CompanyName,
                                freight = o.Freight,
                                shipName = o.ShipName,
                                shipAddress = o.ShipAddress,
                                shipCity = o.ShipCity,
                                shipCountry = o.ShipCountry

                            };

                if (query == null || query.Count() == 0)
                {
                    return NotFound("Veri bulunamadı.");
                }

                return Ok(query);
            }
            catch (Exception ex)
            {
                return BadRequest($"Hata: {ex.Message}");
            }

        }

        [HttpGet("SpeedyOrderNuncyAlfki")]
        public IActionResult SpeedyExpress()
        {
            try
            {

                var query = from o in _context.Orders
                            join emp in _context.Employees on o.EmployeeId equals emp.EmployeeId 
                            join sh in _context.Shippers on o.ShipVia equals sh.ShipperId
                            select new
                            {
                                orderıd = o.OrderId,
                                customerıd = emp.FirstName,
                                orderDate = o.OrderDate.Value.ToString("dd/MM/yyyy"),
                                shippedDate = o.ShippedDate,
                                shipVia = sh.CompanyName,
                                freight = o.Freight,
                                shipName = o.ShipName,
                                shipAddress = o.ShipAddress,
                                shipCity = o.ShipCity,
                                shipCountry = o.ShipCountry,

                            };

            


                if (query == null || query.Count() == 0)
                {
                    return NotFound("Veri bulunamadı.");
                }


                return Ok(query);
            }
            catch (Exception ex)
            {
                return BadRequest($"Hata: {ex.Message}");
            }
        }


        [HttpGet("GetGermanyCustomer")]

        public IActionResult GetGermanyCustomer()
        {
            try
            {
                var query = from c in _context.Customers
                            select new
                            {
                                customerId=c.CustomerId,
                                companyName=c.CompanyName,
                                contactName=c.ContactName,
                                contactTitle = c.ContactTitle,
                                address  = c.Address,
                                city = c.City,
                                postalCode = c.PostalCode,
                                country  = c.Country,
                                phone = c.Phone,
                                fax = c.Fax,
                            };


                return Ok(query);
            }
            catch (Exception ex)
            {
                return BadRequest($"Hata: {ex.Message}");
            }
        }

        [HttpGet("Seafood")]

        public IActionResult Seafood()
        {
            try
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
                              
                                    FirstName = e.FirstName,
                                    LastName = e.LastName,
                                    HomePhone = e.HomePhone
                                
                            };

                var distinctEmployeeInfo = query.Distinct().ToList();


                if (distinctEmployeeInfo == null || distinctEmployeeInfo.Count == 0)
                {
                    return NotFound("Veri bulunamadı.");
                }

                return Ok(distinctEmployeeInfo);
            }
            catch (Exception ex)
            {
                return BadRequest($"Hata: {ex.Message}");
            }


        }

        [HttpGet("SpeedyExpWithNancy")]


        public IActionResult SpeedyExpWithN()
        {
            try
            {
                var query = from o in _context.Orders
                            join sh in _context.Shippers on o.ShipVia equals sh.ShipperId
                            join emp in _context.Employees on o.EmployeeId equals emp.EmployeeId
                            join cst in _context.Customers on o.CustomerId equals cst.CustomerId
                            where (sh.ShipperId == 1 && emp.EmployeeId == 1 && (cst.CustomerId == "ALFKI" || cst.CustomerId == "DUMON"))
                            select new
                            {
                                orderId = o.OrderId,
                                customerId = o.CustomerId,
                                employeeName = emp.FirstName,
                                orderDate = o.OrderDate.Value.ToString("dd/MM/yyyy"),
                                shippedDate = o.ShippedDate,
                                shipVia = sh.CompanyName,
                                freiht = o.Freight,
                                shipName = o.ShipName,
                                shipAddress = o.ShipAddress,
                                shipCity = o.ShipCity,
                                shipCountry = o.ShipCountry

                            };

                var orders = query.ToList();


                if (orders == null || orders.Count == 0)
                {
                    return NotFound("Veri bulunamadı.");
                }


                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest($"Hata: {ex.Message}");
            }

        }

        public class CategoryViewModel
        {
            public int CategoryId { get; set; }
            public string CategoryName { get; set; }
        }

        [HttpGet("Eastern")]
        public IActionResult Eastern()
        {
            try
            {
              
                    var query = from cts in _context.Categories
                                join prd in _context.Products on cts.CategoryId equals prd.CategoryId
                                join ordt in _context.OrderDetails on prd.ProductId equals ordt.ProductId
                                join ord in _context.Orders on ordt.OrderId equals ord.OrderId
                                join cst in _context.Customers on ord.CustomerId equals cst.CustomerId
                                join emp in _context.Employees on ord.EmployeeId equals emp.EmployeeId
                                join empt in _context.EmployeeTerritories on emp.EmployeeId equals empt.EmployeeId
                                join tr in _context.Territories on empt.TerritoryId equals tr.TerritoryId
                                join r in _context.Regions on tr.RegionId equals r.RegionId
                                join shp in _context.Shippers on ord.ShipVia equals shp.ShipperId
                                where r.RegionDescription == "Eastern" && shp.ShipperId == 3
                                select new
                                {
                                    cts.CategoryId,
                                    cts.CategoryName
                                }
                        ;

                    var categories = query.Distinct().ToList();

                    if (categories == null || categories.Count == 0)
                    {
                        return NotFound("Veri bulunamadı.");
                    }

                    return Ok(categories);
                
            }
            catch (Exception ex)
            {
                return BadRequest($"Hata: {ex.Message}");
            }

        }

        [HttpGet("ShippersLondon")]
        public IActionResult ShippersLondon()
        {
            try
            {

                    var query = from o in _context.Orders
                                join emp in _context.Employees on o.EmployeeId equals emp.EmployeeId
                                join s in _context.Shippers on o.ShipVia equals s.ShipperId
                                join odt in _context.OrderDetails on o.OrderId equals odt.OrderId
                                join prd in _context.Products on odt.ProductId equals prd.ProductId
                                join spp in _context.Suppliers on prd.SupplierId equals spp.SupplierId
                                where spp.City == "London" && s.CompanyName.EndsWith("e")
                                      && prd.UnitsInStock > 0 && prd.UnitPrice >= 10 && prd.UnitPrice <= 300
                                select new
                                {
                                    orderId = o.OrderId,
                                    customerId = o.CustomerId,
                                    employeeName = emp.FirstName,
                                    orderDate = o.OrderDate.Value.ToString("dd/MM/yyyy"),
                                    shippedDate = o.ShippedDate,
                                    shipVia = s.CompanyName,
                                    freiht = o.Freight,
                                    shipName = o.ShipName,
                                    shipAddress = o.ShipAddress,
                                    shipCity = o.ShipCity,
                                    shipCountry = o.ShipCountry

                                };

                    var results = query.ToList();

                    if (results == null || results.Count == 0)
                    {
                        return NotFound("Veri bulunamadı.");
                    }


                    return Ok(results);
                
            }
            catch (Exception ex)
            {
                return BadRequest($"Hata: {ex.Message}");
            }
        }

        [HttpGet("DiscontinuedSale")]
        public IActionResult DiscontinuedSale()
        {
            try
            {
                
                    var query = from prd in _context.Products
                                join s in _context.Suppliers on prd.SupplierId equals s.SupplierId
                                where prd.Discontinued && prd.UnitsInStock == 0
                                select new
                                {
                                    s.ContactName,
                                    s.Phone
                                };

                    var results = query.ToList();

                    if (results == null || results.Count == 0)
                    {
                        return NotFound("Veri bulunamadı.");
                    }


                    return Ok(results);
                
            }
            catch (Exception ex)
            {
                return BadRequest($"Hata: {ex.Message}");
            }
        }

        [HttpGet("NewYorkManager")]

        public IActionResult NewYorkManager()
        {
            try
            {
              
                    var query = from t in _context.Territories
                                join empt in _context.EmployeeTerritories on t.TerritoryId equals empt.TerritoryId
                                join emp in _context.Employees on empt.EmployeeId equals emp.EmployeeId
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

                    if (employees == null || employees.Count == 0)
                    {
                        return NotFound("Veri bulunamadı.");
                    }



                    return Ok(employees);
                
            }
            catch (Exception ex)
            {
                return BadRequest($"Hata: {ex.Message}");
            }
        }

        [HttpGet("OrderLast1998")]

        public IActionResult OrderLast1998()
        {
            try
            {
              
                    var query = from o in _context.Orders
                                join c in _context.Customers on o.CustomerId equals c.CustomerId
                                where o.OrderDate > new DateTime(1998, 1, 1)
                                orderby o.OrderDate ascending
                                select new
                                {
                                    o.OrderDate,
                                    c.ContactName
                                };

                    var results = query.ToList();

                    if (results == null || results.Count == 0)
                    {
                        return NotFound("Veri bulunamadı.");
                    }


                    return Ok(results);
                
            }
            catch (Exception ex)
            {
                return BadRequest($"Hata: {ex.Message}");
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
                    product.UnitPrice = product.UnitPrice - product.UnitPrice * ( (discountFactor / 100));
                }

                _context.SaveChanges();

                var get5Products = products.Take(5);

                return Ok(get5Products);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Hata: {ex.Message}" });
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
                    return NotFound(new { message = $"Ürün adı '{productName}' ile eşleşen ürün bulunamadı." });
                }


                product.UnitPrice += increaseAmount;

                _context.SaveChanges();

               


                return Ok(product);

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Hata: {ex.Message}" });
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

                return Ok(   results ); // JSON olarak raporu döndür
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Hata: {ex.Message}" });
            }
        }
        [HttpGet("Tacoma")]
        public IActionResult Tacoma()
        {

            try
            {

                var query = from o in _context.Orders
                            join emp in _context.Employees on o.EmployeeId equals emp.EmployeeId
                            join sh in _context.Shippers on o.ShipVia equals sh.ShipperId
                            where emp.City == "Tacoma"
                            select new
                            {
                                orderId = o.OrderId,
                                customerId = o.CustomerId,
                                employeeName = emp.FirstName,
                                orderDate = o.OrderDate.Value.ToString("dd/MM/yyyy"),
                                shippedDate = o.ShippedDate,
                                shipVia = sh.CompanyName,
                                freiht = o.Freight,
                                shipName = o.ShipName,
                                shipAddress = o.ShipAddress,
                                shipCity = o.ShipCity,
                                shipCountry = o.ShipCountry

                            };


                var results = query.ToList();

                if (results == null || results.Count == 0)
                {
                    return NotFound("Veri bulunamadı.");
                }

                return Ok(results);
            }
            catch (Exception ex)
            {
                return BadRequest($"Hata: {ex.Message}");
            }
        }

        [HttpGet("BestSellerProduct")]

        public IActionResult BestSellerProduct()
        {
            try
            {
               
                    var query = from od in _context.OrderDetails
                                join p in _context.Products on od.ProductId equals p.ProductId
                                join c in _context.Categories on p.CategoryId equals c.CategoryId
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

                    var result = query.ToList().Take(1);

                    if (result == null )
                    {
                        return NotFound("Veri bulunamadı.");
                    }

                    return Ok(result);
                
            }
            catch (Exception ex)
            {
                return BadRequest($"Hata: {ex.Message}");
            }
        }

        [HttpGet("TotalRevenues1997")]
        public IActionResult TotalRevenues1997()
        {
            try
            {
               
                    //var totalUnitPrice = context.OrderDetails
                    //    .Join(context.Orders, od => od.OrderId, o => o.OrderId, (od, o) => new { OrderDetail = od, Order = o })
                    //    .Where(entry => entry.Order.OrderDate.HasValue && entry.Order.OrderDate.Value.Year < 1997)
                    //    .Sum(entry => entry.OrderDetail.UnitPrice);
                        
                    var query = from od in _context.OrderDetails
                                join o in _context.Orders on od.OrderId equals o.OrderId
                                where o.OrderDate.HasValue && o.OrderDate.Value.Year < 1997
                                select od.UnitPrice;


                    var totalUnitPrice = query.Sum();

                    var response = new
                    {
                        totalUnitPrice = totalUnitPrice
                    };
                    string jsonResponse = Newtonsoft.Json.JsonConvert.SerializeObject(response);

                    return Ok("["+jsonResponse+"]");
                
            }
            catch (Exception ex)
            {
                return BadRequest($"Hata: {ex.Message}");
            }
        }

        [HttpGet("ListBestOrders")]

        public IActionResult ListBestOrders()
        {
            try
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

                if (result == null || result.Count == 0)
                {
                    return NotFound("Veri bulunamadı.");
                }


                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Hata: {ex.Message}");
            }
        }

        [HttpGet("NancysProducts")]
        public IActionResult NancysProducts()
        {
            try
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
                if (result == null || result.Count == 0)
                {
                    return NotFound("Veri bulunamadı.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Hata: {ex.Message}");
            }
        }


        [HttpGet("HighAverageSales")]
        public IActionResult HighAverageSales()
        {
            try
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
                 result.Order.ShipPostalCode,
                 result.Order.ShipCountry
             })
             .ToList();
                if (highAverageSales == null || highAverageSales.Count == 0)
                {
                    return NotFound("Veri bulunamadı.");
                }

                return Ok(highAverageSales);
            }
            catch (Exception ex)
            {
                return BadRequest($"Hata: {ex.Message}");
            }
        }

        [HttpGet("USASuplliersBeverages")]
        public IActionResult USASuplliersBeverages()
        {
            try
            {
                var usaBeveragesProducts = (from p in _context.Products
                                            join c in _context.Categories on p.CategoryId equals c.CategoryId
                                            join s in _context.Suppliers on p.SupplierId equals s.SupplierId
                                            where c.CategoryName == "Beverages" && p.UnitsInStock > 0 && s.Country == "USA"
                                            select new
                                            {
                                                ProductName = p.ProductName,
                                                IncreasedPrice = Math.Round(Convert.ToDouble(p.UnitPrice) * 1.2, 2),
                                                UnitPrice = p.UnitPrice,
                                                UnitsInStock = p.UnitsInStock,
                                                SupplierId = p.SupplierId,
                                                CategoryName = c.CategoryName,
                                                Country = s.Country

                                            }).ToList();

                if (usaBeveragesProducts == null || usaBeveragesProducts.Count() == 0)
                    return NotFound("Veri bulunamadı.");


                return Ok(usaBeveragesProducts);

            }
            catch(Exception ex)
            {
                return BadRequest($"Hata: {ex.Message}");
            }
        }

        [HttpGet("TotalOrderCount")]
        public IActionResult TotalOrderCount()
        {
            var TOrderCount = _context.Customers
    .Join(
        _context.Orders,
        c => c.CustomerId,
        o => o.CustomerId,
        (c, o) => new
        {
            CustomerId = c.CustomerId,
            ContactName = c.ContactName,
            OrderCount = o.CustomerId
        }
    )
    .GroupBy(result => new { result.CustomerId, result.ContactName })
    .Select(group => new
    {
        CustomerId = group.Key.CustomerId,
        ContactName = group.Key.ContactName,
        TotalOrderCount = group.Count()
    })
    .OrderByDescending(result => result.TotalOrderCount)
    .ToList();

            return Ok(TOrderCount);
        }

        [HttpGet("ProductTypeCount")]
        public IActionResult ProductTypeCount()
        {
            try
            {

                var query = from o in _context.Orders
                            join ord in _context.OrderDetails on o.OrderId equals ord.OrderId
                            join prd in _context.Products on ord.ProductId equals prd.ProductId
                            group new { o, prd, ord } by new { o.OrderDate, prd.ProductName } into grouped
                            orderby grouped.Key.OrderDate
                            select new
                            {
                                OrderDate = grouped.Key.OrderDate,
                                ProductName = grouped.Key.ProductName,
                                Quantity = grouped.Sum(x => x.ord.Quantity)
                            };

                if (query == null || query.Count() == 0)
                    return NotFound("Veri bulunamadı.");
                



                return Ok(query);
            }
            catch (Exception ex)
            {
                return BadRequest($"Hata: {ex.Message}");
            }

        }


  
        public class ReportFilter
        {
            public string Shipment { get; set; }
            public decimal MinShippingCost { get; set; }
            public int OrderYear { get; set; }
        }
    }



}
