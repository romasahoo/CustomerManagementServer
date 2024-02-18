using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CustomerManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private static readonly List<Customer> _customers = new List<Customer>();
        private const string DataFilePath = "CustomerData.json";

        public CustomersController()
        {
            LoadCustomersFromFile();
        }

        [HttpPost]
        public IActionResult AddCustomers([FromBody] List<Customer> customers)
        {
            if (customers == null || !customers.Any())
                return BadRequest("Invalid customer data.");

            var existingIds = new HashSet<Guid>(_customers.Select(c => c.Id));

            foreach (var customer in customers)
            {
                if (string.IsNullOrWhiteSpace(customer.FirstName) ||
                    string.IsNullOrWhiteSpace(customer.LastName) ||
                    customer.Age <= 18)
                {
                    return BadRequest($"Invalid data for customer {customer.FirstName} {customer.LastName}.");
                }

                
                if (existingIds.Contains(customer.Id))
                {
                    continue;
                }

                
                customer.Id = Guid.NewGuid();

                
                var insertIndex = _customers.FindIndex(c =>
                    string.Compare(c.LastName, customer.LastName, StringComparison.OrdinalIgnoreCase) > 0 ||
                    (string.Compare(c.LastName, customer.LastName, StringComparison.OrdinalIgnoreCase) == 0 &&
                     string.Compare(c.FirstName, customer.FirstName, StringComparison.OrdinalIgnoreCase) > 0));

                if (insertIndex == -1)
                    _customers.Add(customer);
                else
                    _customers.Insert(insertIndex, customer);
            }

            SaveCustomersToFile(); 
            return Ok("Customers added successfully.");
        }


        // GET api/customers
        [HttpGet]
        public IActionResult GetCustomers()
        {
            return Ok(_customers);
        }

        private void LoadCustomersFromFile()
        {
            if (System.IO.File.Exists(DataFilePath))
            {
                var json = System.IO.File.ReadAllText(DataFilePath);
                _customers.AddRange(JsonConvert.DeserializeObject<List<Customer>>(json));
            }
        }

        private void SaveCustomersToFile()
        {
            var json = JsonConvert.SerializeObject(_customers, Formatting.Indented);
            System.IO.File.WriteAllText(DataFilePath, json);
        }

    }

    public class Customer
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }

       
    }


}
