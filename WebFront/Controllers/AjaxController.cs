using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;
using WebFront.Models;

namespace WebFront.Controllers
{
	public class AjaxController : Controller
	{
		NorthwindContext _context;
		public AjaxController(NorthwindContext context)
		{
			_context = context;
		}

		// GET: Ajax/Greet
		[HttpGet]
		public string Greet(string name)
		{
			Thread.Sleep(2000); // 模擬伺服器處理時間(毫秒)，只是為了看loading動畫
			return $"Hello, {name}!";
		}

		// POST: Ajax/Greet
		[HttpPost]
		public string PostGreet(string name)
		{
			return $"Hello, {name}!";
		}

		// POST: Ajax/FetchPostGreet
		[HttpPost]
		public string FetchPostGreet(Models.Parameter p)
		{
			Thread.Sleep(2000); // 只是為了看loading動畫
			return $"Hello, {p.Name}!";
		}

		// POST: Ajax/CheckName
		[HttpPost]
		public string CheckName(string FirstName)
		{
			bool Exists = _context.Employees.Any(e => e.FirstName == FirstName);
			return Exists ? "true" : "false";
		}

		// POST: Ajax/FetchCheckName
		[HttpPost]
		public string FetchCheckName(Models.Parameter p)
		{
			bool Exists = _context.Employees.Any(e => e.FirstName == p.Name);
			return Exists ? "true" : "false";
		}
	}
}
