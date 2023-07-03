using System;

namespace RideShare.Exceptions
{
    public class RideSharingException : Exception
{
    public RideSharingException(string message) : base(message)
    {
    }
}
}