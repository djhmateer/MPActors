using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Dapper;

namespace MPActorsConsole
{
    class ProgramW
    {
        public static void MainW()
        {
            Console.WriteLine("Experimenting with a HOF for db connection and using statements");
            Console.WriteLine("so don't have code duplication");
            var actors = GetActors();

            foreach (var actor in actors) Console.WriteLine(actor);
        }

        // Function 
        public static IEnumerable<Actor> GetActors()
            // calling a static function
            // passing a multi line lambda (essentially another function) for the WithConnection to run
            => WithConnection(conn =>
            {
               var result = conn.Query<Actor>(
                   @"SELECT TOP 10 *
                   FROM Actors");
               return result;
            });

        // Wrapper returns a generic T eg IEnumerable<Actor>
        // It takes as arguments: A Func takes an IDbConnection (which is what we make here) and returns a T of the same type
        public static T WithConnection<T>(Func<IDbConnection, T> func)
        {
            using var conn = new SqlConnection(ConnectionString);
            conn.Open();
            return func(conn);
        }

        public class Actor
        {
            public int actorid { get; set; }
            public string name { get; set; }
            public string sex { get; set; }

            public override string ToString() => $"{actorid} {name} {sex}";
        }


<<<<<<< HEAD
=======
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

        // function that does something
        // we are passing in an IDbConnection
        private static async Task<IEnumerable<Actor>> Bar(IDbConnection conn)
        {
            var result = await conn.QueryAsync<Actor>(@"SELECT TOP 10 * 
                    FROM actors");
            return result;
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

        private const string ConnectionString = "Server=(localdb)\\mssqllocaldb;Database=IMDBChallenge;Trusted_Connection=True;MultipleActiveResultSets=true";

>>>>>>> 81537801f46de7e3a7b826e6d166ce28cbcdd64d
    }
}
