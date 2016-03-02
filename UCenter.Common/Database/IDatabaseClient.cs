using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UCenter.Common.Database
{
    public interface IDatabaseClient
    {
        Task<TResponse> ExecuteSingleAsync<TResponse>(IDatabaseRequest request, CancellationToken token);

        Task<int> ExecuteNoQueryAsync(IDatabaseRequest request, CancellationToken token);

        Task<ICollection<TResponse>> ExecuteListAsync<TResponse>(IDatabaseRequest request, CancellationToken token);
    }

    public interface IDatabaseClient<TDatabaseRequest> : IDatabaseClient where TDatabaseRequest : IDatabaseRequest
    {
        Task<TResponse> ExecuteSingleAsync<TResponse>(TDatabaseRequest request, CancellationToken token);

        Task<int> ExecuteNoQueryAsync(TDatabaseRequest request, CancellationToken token);

        Task<ICollection<TResponse>> ExecuteListAsync<TResponse>(TDatabaseRequest request, CancellationToken token);
    }
}
