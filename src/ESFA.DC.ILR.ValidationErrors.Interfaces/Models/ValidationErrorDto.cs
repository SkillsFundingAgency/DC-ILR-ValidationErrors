namespace ESFA.DC.ILR.ValidationErrors.Interface.Models
{
    public class ValidationErrorDto
    {
        public string ErrorMessage { get; set; }

        public string LearnerReferenceNumber { get; set; }

        public string FieldValues { get; set; }

        public string Severity { get; set; }

        public long? AimSequenceNumber { get; set; }

        public string RuleName { get; set; }
    }
}
