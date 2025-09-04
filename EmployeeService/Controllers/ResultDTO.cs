namespace EmployeeService.Controllers
{
	public class ResultDTO
	{
		public bool Ok { get; set; }    // true 代表呼叫成功，false 代表失敗
		public int Code { get; set; }   // 可以定義錯誤編號或狀態碼
	}
}