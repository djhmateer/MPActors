using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
using Dapper;
using MPActorsConsole.Polly;
using Polly;
using Polly.Retry;
using Xunit;
using Xunit.Abstractions;

namespace MPActorsConsole
{
    public class PollyWith
    {
        private const string ConnectionString = "Server=(localdb)\\mssqllocaldb;Database=IMDBChallenge;Trusted_Connection=True;MultipleActiveResultSets=true";

        public static async Task Main()
        {
            //var actors = await GetTop10Actors(ConnectionString);
            var actors = await GetTop10Actors(ConnectionString);

            foreach (var actor in actors) Console.WriteLine(actor);
        }

        // Function 
        public static async Task<IEnumerable<Actor>> GetTop10Actors(string connectionString)
            => await WithRetryConnection(connectionString, async x =>
            {
                var result = await x.QueryAsync<Actor>(
                    @"SELECT TOP 10 *
                    FROM Actors");

                return result;
            });

        // Wrapper returns a generic T eg IEnumerable<Actor>
        // It takes as arguments: A Func takes an IDbConnection (which is what we make here) and returns a T of the same type
        public static async Task<T> WithRetryConnection<T>(
            string connectionString,
            Func<IDbConnection, Task<T>> func)
        {
            await using var conn = new SqlConnection(connectionString);
            //return await func(conn);
            return await DapperExtensions.RetryPolicy.ExecuteAsync(() => func(conn));
        }

        public class Actor
        {
            public int actorid { get; set; }
            public string name { get; set; }
            public string sex { get; set; }

            public override string ToString() => $"{actorid} {name} {sex}";
        }
    }
}
