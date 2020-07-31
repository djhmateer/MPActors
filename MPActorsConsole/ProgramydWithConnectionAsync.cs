using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
using Dapper;
using Xunit;
using Xunit.Abstractions;

namespace MPActorsConsole
{
    public class MPATests
    {
        private readonly ITestOutputHelper output;
        private readonly string connectionString;

        public MPATests(ITestOutputHelper output)
        {
            this.output = output;
            connectionString = "Server=(localdb)\\mssqllocaldb;Database=IMDBChallengeTest;Trusted_Connection=True;MultipleActiveResultSets=true";
        }

        [Fact]
        public async void ShouldBeAbleToGetActors()
        {
            output.WriteLine("test message");

            var result = await ProgramMPA.GetActors(connectionString);
            Assert.Equal(10,result.Count());
        }
    }

    public class ProgramMPA
    {
        private const string ConnectionString = "Server=(localdb)\\mssqllocaldb;Database=IMDBChallenge;Trusted_Connection=True;MultipleActiveResultSets=true";

        public static async Task Main()
        {
            var actors = await GetActors(ConnectionString);

            foreach (var actor in actors) Console.WriteLine(actor);
        }

        // Function 
        public static async Task<IEnumerable<Actor>> GetActors(string connectionString)
            => await WithConnection(connectionString, async x =>
            {
                var result = await x.QueryAsync<Actor>(
                    @"SELECT TOP 10 *
                    FROM Actors");
                return result;
            });

        // Wrapper returns a generic T eg IEnumerable<Actor>
        // It takes as arguments: A Func takes an IDbConnection (which is what we make here) and returns a T of the same type
        public static async Task<T> WithConnection<T>(
            string connectionString,
            Func<IDbConnection, Task<T>> func)
        {
            await using var conn = new SqlConnection(connectionString);
            return await func(conn);
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
