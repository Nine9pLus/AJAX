using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeService.Models;
using EmployeeService.DTO;

namespace EmployeeService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly NorthwindContext _context;

        public EmployeesController(NorthwindContext context)
        {
            _context = context;
        }

		// GET: api/Employees/Get
		[HttpGet("Get")]
        public ActionResult Get()
        {
			return NoContent();
		}


        // GET: api/Employees
        [HttpGet]
        public async Task<IEnumerable<EmployeeDTO>> GetEmployees()
        {
            return _context.Employees.Select(e => new EmployeeDTO
            {
                EmployeeId = e.EmployeeId,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Title = e.Title,
            });
        }

        // GET: api/Employees/5
        [HttpGet("{id}")]
        public async Task<EmployeeDTO> GetEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return null;
            }

            EmployeeDTO EmpDto = new EmployeeDTO
            {
                EmployeeId = employee.EmployeeId,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Title = employee.Title,
            };

			return EmpDto;
        }

        // PUT: api/Employees/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]   // {} 放變數
		public async Task<ResultDTO> PutEmployee(int id, EmployeeDTO EmpDTO)
        {
            if (id != EmpDTO.EmployeeId)
            {
                return new ResultDTO 
                { 
                    Ok = false, 
                    Code = 400, 
                    // 400 對前端或使用者不一定直觀，Ok 屬性會更易懂
                };  
			}
            
            Employee Emp= await _context.Employees.FindAsync(id);
            if (Emp == null)
            {
				return new ResultDTO
				{
					Ok = false,
					Code = 404,
				};
			}
            else
            { 
                Emp.FirstName = EmpDTO.FirstName;
                Emp.LastName = EmpDTO.LastName;
                Emp.Title = EmpDTO.Title;
				_context.Entry(Emp).State = EntityState.Modified;
            }	

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
                {
					return new ResultDTO
					{
						Ok = false,
						Code = 404,
					};
				}
                else
                {
                    throw;
                }
            }

			return new ResultDTO
			{
				Ok = true,
				Code = 0,
			};
		}

        // POST: api/Employees
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        //public async Task<EmployeeDTO> PostEmployee(EmployeeDTO EmpDTO)
		public async Task<ResultDTO> PostEmployee(EmployeeDTO EmpDTO)
		{
            Employee Emp = new Employee
            {
				// EmployeeId 不用設定，因為是 Identity 欄位
				FirstName = EmpDTO.FirstName,
                LastName = EmpDTO.LastName,
                Title = EmpDTO.Title,
            };

			_context.Employees.Add(Emp);
            await _context.SaveChangesAsync();  // 寫入資料庫

            EmpDTO.EmployeeId = Emp.EmployeeId;  // 將新增後的 EmployeeId 回傳給前端

            //return EmpDTO;
            return new ResultDTO
            {
                Ok = true,
                Code = 0,
            };
		}

        // DELETE: api/Employees/5
        [HttpDelete("{id}")]
        public async Task<ResultDTO> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
				return new ResultDTO
				{
					Ok = false,
					Code = 404,
				};
			}

			try
			{
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
			}
			catch (DbUpdateException ex)
			{
                return new ResultDTO
                {
                    Ok = false,
                    Code = 100,
					// 自定義的錯誤碼
				};
			}
            			
			return new ResultDTO
			{
				Ok = true,
				Code = 0,
			};
		}

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }
    }
}
