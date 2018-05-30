using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.ILR.ValidationErrors.Interface;
using ESFA.DC.ILR.ValidationErrors.Interface.Models;
using ESFA.DC.ILR.ValidationService.IO.Model;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ValidationErrors
{
    public class ValidationErrorsService : IValidationErrorsService
    {
        private readonly IKeyValuePersistenceService _ioPersistenceService;
        private readonly ISerializationService _serializationService;
        private readonly ILogger _logger;

        public ValidationErrorsService(
            IKeyValuePersistenceService ioPersistenceService,
            ISerializationService serializationService,
            ILogger logger)
        {
            _ioPersistenceService = ioPersistenceService;
            _serializationService = serializationService;
            _logger = logger;
        }

        public async Task<IEnumerable<ValidationErrorDto>> GetValidationErrorsAsync(string validationErrorsStorageKey, string validationErrorsLookupStorageKey)
        {
            var result = new List<ValidationErrorDto>();
            try
            {
                var reportExists = await _ioPersistenceService.ContainsAsync(validationErrorsStorageKey);

                if (reportExists)
                {
                    _logger.LogInfo($"Error report exists for validationErrorsStorageKey: {validationErrorsStorageKey}, validationErrorsLookupStorageKey : {validationErrorsLookupStorageKey}");
                    var validationErrorsData = await _ioPersistenceService.GetAsync(validationErrorsStorageKey);
                    var errorsLookupData = await _ioPersistenceService.GetAsync(validationErrorsLookupStorageKey);

                    var validationErrors =
                        _serializationService.Deserialize<IEnumerable<ValidationError>>(validationErrorsData);
                    var errorMessageLookups =
                        _serializationService.Deserialize<IEnumerable<ValidationErrorMessageLookup>>(errorsLookupData);

                    validationErrors.ToList().ForEach(x =>
                        result.Add(new ValidationErrorDto()
                        {
                            AimSequenceNumber = x.AimSequenceNumber,
                            LearnerReferenceNumber = x.LearnerReferenceNumber,
                            RuleName = x.RuleName,
                            Severity = x.Severity,
                            ErrorMessage = errorMessageLookups.Single(y => x.RuleName == y.RuleName).Message,
                            FieldValues = GetValidationErrorParameters(x.ValidationErrorParameters.ToList()),
                        }));
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error occured trying to get validation errors for validationErrorsStorageKey: {validationErrorsStorageKey}, validationErrorsLookupStorageKey : {validationErrorsLookupStorageKey}", e);
                throw;
            }

            return result;
        }

        public string GetValidationErrorParameters(List<ValidationErrorParameter> validationErrorParameters)
        {
            var result = new System.Text.StringBuilder();
            validationErrorParameters.ForEach(x =>
            {
                result.Append($"{x.PropertyName}={x.Value}");
                result.Append("|");
            });
            return result.ToString();
        }
    }
}