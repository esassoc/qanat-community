namespace Qanat.Models.DataTransferObjects;

public class OpenETException : Exception
{
    public OpenETException()
    {
    }

    public OpenETException(string message) : base(message)
    {
    }

    public OpenETException(string message, Exception inner) : base(message, inner)
    {
    }
}