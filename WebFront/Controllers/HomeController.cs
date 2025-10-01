using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebFront.Models;

namespace WebFront.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;

		public HomeController(ILogger<HomeController> logger)
		{
			_logger = logger;
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		// GET: Home/Employee
		[HttpGet]
		public IActionResult Employee()
		{
			return View();
		}

		// GET: Home/Greet
		[HttpGet]
		public IActionResult Greet()
		{
			return View();
		}

		// GET: Home/CheckName
		[HttpGet]
		public IActionResult CheckName()
		{
			return View();
		}

		// GET: Home/Basic
		[HttpGet]
		public IActionResult Basic()
		{
			return View();
		}

		// GET: Home/Rate
		[HttpGet]
		public IActionResult Rate()
		{
			return View();
		}

		// �N�z�{��
		// GET: Home/GetExchangeRate
		[HttpGet]
		public async Task<string> GetExchangeRate()
		{
			string Uri = "https://openapi.taifex.com.tw/v1/DailyForeignExchangeRates";
			HttpClient Client = new HttpClient();
			HttpResponseMessage Response = await Client.GetAsync(Uri);
			Response.EnsureSuccessStatusCode();     // �P�_���S�����A�p�G���A�X���O 200 (OK)�A�N�|��ҥ~�]Exception�^
			string Data = await Response.Content.ReadAsStringAsync();   // ��^���D��]Response Body�^�ন�r��
			ExchangeRate[] Rate = JsonSerializer.Deserialize<ExchangeRate[]>(Data); // ��r��ϧǦC�Ʀ� ExchangeRate �}�C
			return Rate.Last().USDNTD;
		}

		// GET: Home/Buy
		[HttpGet]
		public IActionResult Buy()
		{
			return View();
		}


		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
