using Microsoft.EntityFrameworkCore;

namespace WebFront.Models
{
	public partial class NorthwindContext : DbContext
	{
		public NorthwindContext() { }
		// 到時候要用NorthwindContext context = new NorthwindContext(); ，是叫用建構函式，所以有建構函式才能 new 出來

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured)
			{
				IConfiguration Config = new ConfigurationBuilder()
				.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
				.AddJsonFile("appsettings.json")
				.Build();
				optionsBuilder.UseSqlServer(Config.GetConnectionString("Northwind"));
			}
		}
	}
}
