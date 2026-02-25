namespace CQRS.Application.Shared.Exceptions
{
    public class JobOrderNotFoundException : Exception
    {
        public JobOrderNotFoundException(string jono, string reason, string v)
                : base($"Joborder '{jono}' {reason}.") { }
    }
}
