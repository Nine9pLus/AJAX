namespace EmployeeService.DTO
{
	public class EmployeeDTO
	{
		public int EmployeeId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }

		// 因為想要做 Single Page，所以要比較多欄位
		// Title允許沒有值，加個?
		public string? Title { get; set; }
		public DateTime? BirthDate { get; set; }
		public DateTime? HireDate { get; set; }
		public string? Address { get; set; }
		public string? City { get; set; }
		public string? PostalCode { get; set; }
		public string? Country { get; set; }
		public string? HomePhone { get; set; }

		// 圖片不能用 byte陣列，且要加?
		//public byte[] Photo { get; set; }
		public IFormFile? Photo { get; set; }   // IFormFile是檔案上傳
	}
}