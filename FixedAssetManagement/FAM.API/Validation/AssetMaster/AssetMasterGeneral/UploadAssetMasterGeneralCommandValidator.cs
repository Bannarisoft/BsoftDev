using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetMaster.AssetMasterGeneral.Commands.UploadAssetMasterGeneral;
using FAM.API.Validation.Common;
using FluentValidation;

namespace FAM.API.Validation.AssetMaster.AssetMasterGeneral
{
    public class UploadAssetMasterGeneralCommandValidator : AbstractValidator<UploadFileAssetMasterGeneralCommand>
    {
        private readonly List<ValidationRule> _validationRules;
        public UploadAssetMasterGeneralCommandValidator()
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
                        .WithMessage($"{nameof(UploadFileAssetMasterGeneralCommand.File)} {rule.Error}")
                        .NotEmpty()
                        .WithMessage($"{nameof(UploadFileAssetMasterGeneralCommand.File)} {rule.Error}");
                        break;
                    case "FileValidation":
                        RuleFor(x => x.File)
                        .Must(file => IsValidFileType(file, rule.allowedExtensions))
                        .WithMessage($"{nameof(UploadFileAssetMasterGeneralCommand.File)} {rule.Error}")
                        .Must(file => file.Length <= 2 * 1024 * 1024)
                        .WithMessage($"{nameof(UploadFileAssetMasterGeneralCommand.File)} {rule.Error}");
                        break;
                }
            }        }
        private bool IsValidFileType(IFormFile file, List<string> allowedExtensions)
        {           
            foreach (var extension in allowedExtensions)
            {
                Console.WriteLine(extension);
                
            }
            
            if (file == null) return false;
        
            var fileExtension = System.IO.Path.GetExtension(file.FileName).ToLowerInvariant();
            return allowedExtensions.Contains(fileExtension);
        }
        }
}