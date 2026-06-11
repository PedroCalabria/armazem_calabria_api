using FluentValidation.Results;
using System.Collections;

namespace ArmazemCalabria.CrossCutting
{
    public interface IUserContext
    {
        DateTime StartDateTime { get; set; }
        ISourceInfo SourceInfo { get; set; }
        Guid ResquestId { get; set; }
        Hashtable AdditionalData { get; set; }
        ValidationResult ValidationResult { get; set; }
        Hashtable UnhandledExceptions { get; set; }
    }

    public class UserContext : IUserContext
    {
        public UserContext()
        {
            ValidationResult = new ValidationResult();
            SourceInfo = new SourceInfo();
            AdditionalData = [];
        }

        public DateTime StartDateTime { get; set; }
        public ISourceInfo SourceInfo { get; set; }
        public Guid ResquestId { get; set; }
        public Hashtable AdditionalData { get; set; }
        public ValidationResult ValidationResult { get; set; }
        public Hashtable UnhandledExceptions { get; set; }
    }
}