namespace CQRS.Application.Shared.Exceptions
{
    public class MainSerialNotFoundException : Exception
    {
        public MainSerialNotFoundException(string mainSerial, string reason, string v)
            : base($"MainSerial '{mainSerial}' {reason}.") { }
    }
}
