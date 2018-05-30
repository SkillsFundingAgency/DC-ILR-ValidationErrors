using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.ILR.ValidationErrors.Interface.Models;

namespace ESFA.DC.ILR.ValidationErrors.Interface
{
    public interface IValidationErrorsService
    {
        Task<IEnumerable<ValidationErrorDto>> GetValidationErrorsAsync(string validationErrorsStorageKey, string validationErrorsLookupStorageKey);
    }
}
