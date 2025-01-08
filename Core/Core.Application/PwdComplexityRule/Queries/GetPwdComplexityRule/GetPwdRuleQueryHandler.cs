using Dapper;
using AutoMapper;
using Core.Application.Common.Interfaces;
using MediatR;
using System.Data;
using Core.Domain.Entities;

namespace Core.Application.PwdComplexityRule.Queries
{
    public class GetPwdRuleQueryHandler  :IRequestHandler<GetPwdRuleQuery, List<PwdRuleDto>>
    {
       private readonly IDbConnection _dbConnection;
      


        public GetPwdRuleQueryHandler( IDbConnection dbConnection)
        {
            _dbConnection=dbConnection;
        }

        public async Task<List<PwdRuleDto>> Handle(GetPwdRuleQuery request, CancellationToken cancellationToken)
        {
              const string query = @"SELECT  * FROM AppSecurity.PasswordComplexityRule";
        var pwdComplexityRules = await _dbConnection.QueryAsync<Core.Domain.Entities.PasswordComplexityRule>(query);
        return pwdComplexityRules.Select(rule => new PwdRuleDto
        {
            Id = rule.Id,
            PwdComplexityRule = rule.PwdComplexityRule,
            IsActive = rule.IsActive
        }).ToList();

        }
    }

    

}