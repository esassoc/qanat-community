namespace Qanat.Models.DataTransferObjects
{
    public class ErrorMessage()
    {
        public string Type { get; set; }
        public string Message { get; set; }

        public ErrorMessage(string type, string message) : this()
        {
            Type = type;
            Message = message;
        }
    }
}