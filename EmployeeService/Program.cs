
using EmployeeService.Models;
using Microsoft.EntityFrameworkCore;
using EmployeeService.Controllers;
using Microsoft.Extensions.FileProviders;

namespace EmployeeService
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the DI container.
			// ++
			builder.Services.AddDbContext<NorthwindContext>(options =>
			{
				options.UseSqlServer(builder.Configuration.GetConnectionString("Northwind"));
			});

			// ++
			// 定了一個策略名稱為 WebFront，套用範圍是全公開
			builder.Services.AddCors(options =>
			{
				options.AddPolicy(
						name: "WebFront",
						policy =>
						{
							// 選擇開放的範圍
							// 先做全公開，成功後再收斂
							policy.AllowAnyOrigin()              // 全部來源都能存取
																 //policy.WithOrigins("https://localhost:7044") // 若要限制單一來源，改用這行
								  .AllowAnyHeader()              // 允許任意 HTTP Header
								  .AllowAnyMethod();             // 允許任意 HTTP Method (GET/POST/PUT/DELETE…)
						}
					);
				// 如果有多個分別要開放的對象，可以Add多個Policy，各自命名
			});

			builder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}
			;

			app.UseHttpsRedirection();
			app.UseStaticFiles(new StaticFileOptions
			{
				// 組合放置圖片資料夾路徑(內容檔案的根目錄，內容檔案的路徑)，images資料夾要先自加好
				FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath,"images")),    
				RequestPath = "/images"
			});
			app.UseAuthorization();

			// ++
			// 1.全域套用策略
			// 在 program.cs 套用策略，()裡就要寫指定策略名稱 => 影響的是全部的 Controller
			// app.UseCors("WebFront");

			// 2.局部套用策略
			// 若要延後到各個 Controller/Action 來套用，可以在類別或方法加上 [EnableCors("WebFront")] 
			// 程式管線中仍然要呼叫 app.UseCors(); 這行
			app.UseCors();


			app.MapControllers();

			app.Run();
		}
	}
}
