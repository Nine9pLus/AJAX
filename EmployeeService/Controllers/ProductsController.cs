using EmployeeService.DTO;
using EmployeeService.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeService.Controllers
{
	[EnableCors("WebFront")]
	[Route("api/[controller]")]
	[ApiController]
	public class ProductsController : ControllerBase
	{
		NorthwindContext _context = new NorthwindContext();
		public ProductsController(NorthwindContext context)
		{
			_context= context;
		}

		// GET: api/Products
		[HttpGet]
		public async Task<IEnumerable<ProductDTO>> GetProducts()
		{
			return _context.Products
				.Select(p => new ProductDTO
				{
					ProductName = p.ProductName,
					UnitPrice = p.UnitPrice,
				});
		}
	}
}
