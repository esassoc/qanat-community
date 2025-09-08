namespace Qanat.Models.DataTransferObjects;

public class AlertMessageDto
{
    public AlertMessageTypeEnum AlertMessageType { get; set; }
    public string Message { get; set; }
}