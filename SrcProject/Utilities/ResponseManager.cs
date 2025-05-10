namespace SrcProject.Utilities
{
    public class ResponseManager
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public DateTime ExpireDate { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
