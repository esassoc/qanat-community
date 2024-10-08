namespace Qanat.Models.DataTransferObjects;

public abstract class BaseResponseDto
{
    public List<AlertMessageDto> Messages { get; set; }
}