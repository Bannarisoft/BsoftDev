using Core.Application.AssetMaster.AssetMasterGeneral.Commands.UploadAssetMasterGeneral;
using Core.Application.AssetMaster.AssetMasterGeneral.Commands.UploadDocumentAssetMaster;
using FAM.API.Validation.Common;
using FluentValidation;

namespace FAM.API.Validation.AssetMaster.AssetMasterGeneral
{
    public class UploadDocumentAssetMasterGeneralCommandValidator : AbstractValidator<UploadDocumentAssetMasterGeneralCommand>
    {
        private readonly List<ValidationRule> _validationRules;
        public UploadDocumentAssetMasterGeneralCommandValidator()
        {
            _validationRules = ValidationRuleLoader.LoadValidationRules();
             if (_validationRules == null || !_validationRules.Any())
            {
                throw new InvalidOperationException("Validation rules could not be loaded.");
            }
             foreach (var rule in _validationRules)
            {
                switch (rule.Rule)
                {
                    case "NotEmpty":
                        RuleFor(x => x.File)
                            .NotNull()
                            .WithMessage($"{nameof(UploadDocumentAssetMasterGeneralCommand.File)} {rule.Error}")
                            .NotEmpty()
                            .WithMessage($"{nameof(UploadDocumentAssetMasterGeneralCommand.File)} {rule.Error}");
                        break;
                    case "FilePDFValidation":
                        RuleFor(x => x.File)
                            .Must(file => file != null && IsValidFileType(file, rule.allowedExtensions))
                            .WithMessage($"{nameof(UploadFileAssetMasterGeneralCommand.File)} {rule.Error}")
                            .Must(file => file != null && file.Length <= 2 * 1024 * 1024) // 2MB size limit
                            .WithMessage($"{nameof(UploadDocumentAssetMasterGeneralCommand.File)} {rule.Error}");
                        break;                   
                }
            }        
        }
       private bool IsValidFileType(IFormFile file, List<string> allowedExtensions)
        {
            if (file == null || allowedExtensions == null || !allowedExtensions.Any())
            {
                return false;
            }

            var fileExtension = System.IO.Path.GetExtension(file.FileName).ToLowerInvariant();
            return allowedExtensions.Contains(fileExtension);
        }
    }
}