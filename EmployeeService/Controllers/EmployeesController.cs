using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeService.Models;
using EmployeeService.DTO;
using Microsoft.AspNetCore.Cors;

namespace EmployeeService.Controllers
{
	[EnableCors("WebFront")]
	[Route("api/[controller]")]
	[ApiController]
	public class EmployeesController : ControllerBase
	{
		private readonly NorthwindContext _context;

		public EmployeesController(NorthwindContext context)
		{
			_context = context;
		}

		// 做篩選，[FromBody]=>傳過來的參數在body裡
		// Post: api/Employees/Filter
		[HttpPost("Filter")]
		public async Task<IEnumerable<EmployeeDTO>> FilterEmployees([FromBody] EmployeeDTO EmpDTO)
		{
			// 二進位不能篩選，public IFormFile? Photo { get; set; } X
			return _context.Employees.Where(e => e.EmployeeId == EmpDTO.EmployeeId ||
									e.FirstName.Contains(EmpDTO.FirstName) ||
									e.LastName.Contains(EmpDTO.LastName) ||
									e.Title.Contains(EmpDTO.Title) ||
									e.Address.Contains(EmpDTO.Address) ||
									e.City.Contains(EmpDTO.City) ||
									e.PostalCode.Contains(EmpDTO.PostalCode) ||
									e.Country.Contains(EmpDTO.Country) ||
									e.HomePhone.Contains(EmpDTO.HomePhone))
				.Select(e => new EmployeeDTO
				{
					//Model 轉 DTO
					EmployeeId = e.EmployeeId,
					FirstName = e.FirstName,
					LastName = e.LastName,
					Title = e.Title,
					Address = e.Address,
					City = e.City,
					PostalCode = e.PostalCode,
					Country = e.Country,
					HomePhone = e.HomePhone,
					BirthDate = e.BirthDate,    // 雖然剛篩選沒有要，但查詢要
					HireDate = e.HireDate,
					Photo = null,   // 填空值=>跳過這個欄位不讀
									// 不回傳圖片(之後要另做 GET 顯示圖，所以不重複叫圖)
				});
		}

		// 抓替代圖片
		// Get: /api/Employees/GetPhoto/1
		[HttpGet("GetPhoto/{id}")]
		// 路由樣板 = 固定字串區段 "GetPhoto" + 動態參數區段 "{id}"
		// 例如呼叫 /api/Employees/GetPhoto/1 時，id=1 會被路由自動解析並傳進方法參數
		public async Task<FileResult> GetPhoto(int id)
		{
			// 抓預設替代圖案的路徑：wwwroot/images/NoImages.png
			// 第一段：images資料夾，第二段：檔名NoImages.png
			string Filename = Path.Combine("images", "NoImages.png");

			// 依據 id 取出員工
			Employee Emp = await _context.Employees.FindAsync(id);

			// 若找不到員工，或員工沒有 Photo，則回傳預設圖片
			// Photo的型態是二進位，二進位資料型態是byte陣列
			byte[] ImageContent = Emp?.Photo ?? System.IO.File.ReadAllBytes(Filename);

			// 可能找不到此員工 id、有 id 但其 Photo 沒有值(因為 Photo 這個欄位允許沒有值)

			// 1. Emp可能為null
			// 如果Emp不加問號：Emp可能為null，如果沒有值再去抓他的屬性(Photo)，會run time error，執行時期錯誤
			// 所以加個問號，Emp?.Photo：如果Emp有值再抓他的Photo，沒值就直接傳回null

			// 2.Photo可能為null
			// Photo有值就拿他的值去用，如果沒有值，就讀Filename

			// 要用的函式是File的ReadAllBytes，為什麼要用全名?
			// System 是 namespace
			// 因為如果只寫File，他會以為這是函式而不是類別，報錯：函式沒有ReadAllBytes的屬性可以用
			// 雖然.NET平台本來就有一個類別叫File，但因為controller繼承的父類別ControllerBase也有一個函式叫File，用來產生 FileResult
			// 所以如果只寫File，他會覺得你在呼叫父類別的File，所以才寫全名 System.IO.File

			// 這個File就是真的函式了，由父類別提供的，ControllerBase.File() 方法回傳檔案
			return File(ImageContent, "image/png");
			// 第二個參數是 MIME 類型，會在 HTTP 回應頭 Content-Type 中標明
			// 告訴瀏覽器這是一張 PNG 圖片，瀏覽器就會用正確的方式處理與顯示
		}

		// 修改
		// PUT: api/Employees/5
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPut("{id}")]
		public async Task<ResultDTO> PutEmployee(int id, EmployeeDTO EmpDTO)
		{
			// 先判斷傳在網址列的id跟DTO的id有沒有一樣
			if (id != EmpDTO.EmployeeId)
			{
				return new ResultDTO
				{
					Ok = false,
					Code = 400,
				};
			}

			// 依據id取出要修改的紀錄
			Employee Emp = await _context.Employees.FindAsync(id);
			if (Emp == null)
			{	
				// 取不到傳false
				return new ResultDTO
				{
					Ok = false,
					Code = 404,
				};
			}
			else
			{
				// 取得到就開始改
				Emp.FirstName = EmpDTO.FirstName;
				Emp.LastName = EmpDTO.LastName;
				Emp.Title = EmpDTO.Title;
				// 補新增的
				Emp.BirthDate = EmpDTO.BirthDate;
				Emp.HireDate = EmpDTO.HireDate;
				Emp.Address = EmpDTO.Address;
				Emp.City = EmpDTO.City;
				Emp.PostalCode = EmpDTO.PostalCode;
				Emp.Country = EmpDTO.Country;
				Emp.HomePhone = EmpDTO.HomePhone;
				// 補圖
				if (EmpDTO.Photo != null)
				{
					using (BinaryReader br = new BinaryReader(EmpDTO.Photo.OpenReadStream()))
					{
						Emp.Photo = br.ReadBytes((int)EmpDTO.Photo.Length);
					}
				}
				// 寫入
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

		// 新增
		// POST: api/Employees
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPost]
		public async Task<ResultDTO> PostEmployee(EmployeeDTO EmpDTO)
		{
			Employee Emp = new Employee
			{
				// EmployeeId 不用設定，因為是 Identity 欄位
				FirstName = EmpDTO.FirstName,
				LastName = EmpDTO.LastName,
				Title = EmpDTO.Title,
				BirthDate = EmpDTO.BirthDate,
				HireDate = EmpDTO.HireDate,
				Address = EmpDTO.Address,
				City = EmpDTO.City,
				PostalCode = EmpDTO.PostalCode,
				Country = EmpDTO.Country,
				HomePhone = EmpDTO.HomePhone,
			};
			// 如果user有上傳圖片
			// Photo類型是IFormFile，IFormFile是檔案上傳
			if (EmpDTO.Photo != null)
			{
				// 已經判斷有值，所以可以做開啟檔案的讀取串流
				using (BinaryReader br=new BinaryReader(EmpDTO.Photo.OpenReadStream()))
				{
					// 就可以讀
					// ReadBytes(int count)：讀取指定長度的二進位資料並回傳 byte[]
					// ReadBytes需要int，強制轉型
					Emp.Photo = br.ReadBytes((int)EmpDTO.Photo.Length);
				}
			}
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

		// 刪除
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
