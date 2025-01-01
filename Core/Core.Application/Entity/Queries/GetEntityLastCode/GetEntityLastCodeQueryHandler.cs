using MediatR;
using System.Data;
using Dapper;


namespace Core.Application.Entity.Queries.GetEntityLastCode
{
    public class GetEntityLastCodeQueryHandler : IRequestHandler<GetEntityLastCodeQuery,string>
    {
    private readonly IDbConnection _dbConnection;

    public GetEntityLastCodeQueryHandler(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    } 
       // Last Entity Code Check 
       public async Task<string> Handle(GetEntityLastCodeQuery request, CancellationToken cancellationToken)
       {
          var query = "SELECT TOP 1 EntityCode FROM AppData.Entity ORDER BY Id DESC";
          var lastCode = await _dbConnection.QueryFirstOrDefaultAsync<string>(query);

          if (string.IsNullOrEmpty(lastCode))
          {
            lastCode = "ENT-00000";
          }

          var nextCodeNumber = int.Parse(lastCode[(lastCode.IndexOf('-') + 1)..]) + 1;

          return $"ENT-{nextCodeNumber:D5}";
       
       }


    }
}