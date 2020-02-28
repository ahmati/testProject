using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TestProject.Models;
using TestProject.ViewModels;

namespace TestProject.Controllers
{    
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("CorsPolicy")]
    public class EmployeeController : ControllerBase
    {
        private readonly NorthwindContext _context;

        public EmployeeController(NorthwindContext context)
        {
            _context = context;
        }
        
        [HttpGet("get")]
        public  IActionResult GetEmployees()
        {
            var employees = _context.Employees.ToList();
            var result = new List<EmployeeViewModel>();

            foreach (var employee in employees)
            {
                result.Add(new EmployeeViewModel
                {
                    BirthDate = employee.BirthDate,
                    City = employee.City,
                    EmployeeId = employee.EmployeeId,
                    FirstName = employee.FirstName,
                    HireDate = employee.HireDate,
                    LastName = employee.LastName,
                    Title = employee.Title
                });
            }
            var toSend = JsonConvert.SerializeObject(result);
            return Ok(toSend);
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            if (UserExists(id))
                return BadRequest("This user doesnt exist!");

            var result = await _context.Employees.Where(x=>x.EmployeeId==id).ToListAsync();
            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateEmployee([FromBody]EmployeeViewModel employeeViewModel)
        {
        
            var newEmployee = new Employees
            {
                LastName=employeeViewModel.LastName,
                FirstName=employeeViewModel.FirstName,
                Title=employeeViewModel.Title,
                BirthDate=employeeViewModel.BirthDate,
                HireDate=employeeViewModel.HireDate,
                City=employeeViewModel.City
            };
            _context.Add(newEmployee);
            await _context.SaveChangesAsync();

            return Ok(newEmployee);

        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateEmployee(int id,EmployeeViewModel employeeViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (UserExists(id))
                return BadRequest("This user doesnt exist!");

            var userToUpdate = _context.Employees.Find(id);

            userToUpdate.LastName = employeeViewModel.LastName;
            userToUpdate.FirstName = employeeViewModel.FirstName;
            userToUpdate.Title = employeeViewModel.Title;
            userToUpdate.BirthDate = employeeViewModel.BirthDate;
            userToUpdate.HireDate = employeeViewModel.HireDate;
            userToUpdate.City = employeeViewModel.City;
            
            _context.Update(userToUpdate);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            if (UserExists(id))
                return BadRequest("This user already doesnt exist!");

            var employee = await _context.Employees.Where(x => x.EmployeeId == id).FirstOrDefaultAsync();

            _context.Remove(employee);
            _context.SaveChanges();

            return Ok();

        }


        public bool UserExists(int id)
        {
            var result = _context.Employees.Where(x => x.EmployeeId == id).FirstOrDefault();
            if (result == null)
                return true;
            return false;
        }
    }
}
