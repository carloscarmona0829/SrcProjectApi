namespace SrcProject.Models.OutModels
{
    public class ResponseManagerOM
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public dynamic Response { get; set; }
    }
}
