namespace dotnetAPI.Helper
{
    public class BaseResult<T>
    {
        public bool Success { get; set; }
        public List<string> Errors { get; set; }
        public string Message { get; set; }
        public int Status { get; set; }
        public int TotalData { get; set; }
        public T Data { get; set; }
    }
}
