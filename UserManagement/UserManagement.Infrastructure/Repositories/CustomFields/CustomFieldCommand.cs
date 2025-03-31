using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.ICustomField;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using UserManagement.Infrastructure.Data;

namespace UserManagement.Infrastructure.Repositories.CustomFields
{
    public class CustomFieldCommand : ICustomFieldCommand
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public CustomFieldCommand(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
        public async Task<int> CreateAsync(CustomField customField)
        {
            await _applicationDbContext.CustomField.AddAsync(customField);
            await _applicationDbContext.SaveChangesAsync();
            return customField.Id;
        }

        public async Task<bool> DeleteAsync(int id, CustomField customField)
        {
            var existingCustomField = await _applicationDbContext.CustomField.FirstOrDefaultAsync(u => u.Id == id);
            if (existingCustomField != null)
            {
                existingCustomField.IsDeleted = customField.IsDeleted;
                return await _applicationDbContext.SaveChangesAsync() >0;
            }
            return false; 
        }

        public async Task<bool> UpdateAsync(CustomField customField)
        {
            var existingCustomField = await _applicationDbContext.CustomField.FirstOrDefaultAsync(u => u.Id == customField.Id);
            if (existingCustomField != null)
            {
                existingCustomField.LabelName = customField.LabelName;
                existingCustomField.DataTypeId = customField.DataTypeId;
                existingCustomField.Length = customField.Length;
                existingCustomField.LabelTypeId = customField.LabelTypeId;
                existingCustomField.IsRequired = customField.IsRequired;
                existingCustomField.IsActive = customField.IsActive;

                _applicationDbContext.CustomField.Update(existingCustomField);
                return await _applicationDbContext.SaveChangesAsync() > 0;
            }
            return false;
        }
    }
}