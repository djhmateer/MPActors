using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
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
    public class HttpPolly
    {
        // test the Http polly
        // copying the code in the Polly-Samples project
        // and using their webapi test server
        // https://localhost:44307
        // http://pollytestapp.azurewebsites.net/Help

        public static async Task Main()
        {

            var httpClient = new HttpClient();

            var url = "https://localhost:44307/api/values";

            while (true)
            {
                var httpResponseMessage = await httpClient.GetAsync(url);

                Console.WriteLine($"Status code is {httpResponseMessage.StatusCode}");
            }

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
