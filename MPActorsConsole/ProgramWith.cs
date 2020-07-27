using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Dapper;

namespace MPActorsConsole
{
    class Program
    {
        public static void Main()
        {
            Console.WriteLine("Experimenting with a HOF for db connection and using statements");
            Console.WriteLine("so don't have code duplication");
            var actors = GetActors();

            foreach (var actor in actors) Console.WriteLine(actor);
        }

        public static IEnumerable<Actor> GetActors()
        {
            using (var connection = GetOpenConnection())
            {
                return connection.Query<Actor>(
                    @"SELECT TOP 10 *
                    FROM Actors");
            }
        }

        public static IDbConnection GetOpenConnection()
        {
            var connection = new SqlConnection(ConnectionString);
            connection.Open();
            return connection;
        }

        public class Actor
        {
            public int actorid { get; set; }
            public string name { get; set; }
            public string sex { get; set; }

            public override string ToString() => $"{actorid} {name} {sex}";
        }

        private const string ConnectionString = "Server=(localdb)\\mssqllocaldb;Database=IMDBChallenge;Trusted_Connection=True;MultipleActiveResultSets=true";

        public static Task<IEnumerable<Actor>> GetActorsx()
        {
            // passing a lambda expression (function) into 
            //var foo = WithConnection(async conn =>
            //{
            //    var result = await conn.QueryAsync<Actor>(
            //        @"SELECT TOP 10 * 
            //        FROM actors");
            //    return result;
            //});
            var foo = WithConnection(Bar);

            return foo;
        }

        // function that does something
        // we are passing in an IDbConnection
        private static async Task<IEnumerable<Actor>> Bar(IDbConnection conn)
        {
            var result = await conn.QueryAsync<Actor>(@"SELECT TOP 10 * 
                    FROM actors");
            return result;
        }

        // Wrapper function
        // does the connection to the db (
        private static async Task<T> WithConnection<T>(
            Func<IDbConnection, Task<T>> connectionFunction)
        {
            using (var conn = new SqlConnection("Server=(localdb)\\mssqllocaldb;Database=IMDBChallenge;Trusted_Connection=True;MultipleActiveResultSets=true"))
            {
                conn.Open();
                return await connectionFunction(conn);
            }
        }

        //private static async Task WithConnection(
        //    string connectionString,
        //    Func<IDbConnection, Task> connectionFunction)
        //{
        //    // await using?
        //    using (var conn = new SqlConnection("Server=(localdb)\\mssqllocaldb;Database=IMDBChallenge;Trusted_Connection=True;MultipleActiveResultSets=true"))
        //    {
        //        // why not openasync?
        //        conn.Open();
        //        await connectionFunction(conn);
        //    }
        //}





        public static async Task<IDbConnection> GetOpenConnectionAsync()
        {
            var connection = new SqlConnection("Server=(localdb)\\mssqllocaldb;Database=IMDBChallenge;Trusted_Connection=True;MultipleActiveResultSets=true");
            await connection.OpenAsync();
            return connection;
        }




        // Wrapper
        // Return a tuple with the int output of the inner function, and the elapsedMilliseconds of this WithTimer wrapper
        // pass in the string input to the inner function
        public static (int, string) WithTimer(string input, Func<string, int> function)
        {
            var sw = Stopwatch.StartNew();
            int output = function(input);
            return (output, sw.ElapsedMilliseconds.ToString());
        }

        // Function that DoesSomething
        public static int DoSomething(string input)
        {
            Thread.Sleep(500);
            return 2;
        }

        // Example 1
        //public static void Main()
        //{
        //    // Calling code
        //    var (output, elapsedMilliseconds) = WithTimer("input for DoSomething", DoSomething);
        //    Console.WriteLine($"output is: {output} which took: {elapsedMilliseconds}ms");
        //}

        //// Wrapper
        //// Return a tuple with the int output of the inner function, and the elapsedMilliseconds of this WithTimer wrapper
        //// pass in the string input to the inner function
        //public static (int, string) WithTimer(string input, Func<string, int> function)
        //{
        //    var sw = Stopwatch.StartNew();
        //    int output = function(input);
        //    return (output, sw.ElapsedMilliseconds.ToString());
        //}

        //// Function that DoesSomething
        //public static int DoSomething(string input)
        //{
        //    Thread.Sleep(500);
        //    return 2;
        //}
    }
}
