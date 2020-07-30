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
    class ProgramBare
    {
        static async Task MainBare()
        {
            var sw = Stopwatch.StartNew();

            // 2 tasks will be going simultaneously
            // asynchronous concurrency - this is appropriate when you have I/O bound code eg http
            //Console.WriteLine("Starting DoSomethingA task");
            //Task<string> doSomethingTaskA = DoSomething("A");

            //Console.WriteLine("Starting DoSomethingB task");
            //var doSomethingTaskB = DoSomething("B");

            //// can do work that doesn't rely on doSomething results
            //Console.WriteLine("doing other stuff");

            //// await suspends the function Main, and can't continue until doSomethingTask completes
            //string resultA = await doSomethingTaskA;
            //Console.WriteLine($"resultA is {resultA}");

            //string resultB = await doSomethingTaskB;
            //Console.WriteLine($"resultb is {resultB}");

            //string resultC = await DoSomething("C");
            //Console.WriteLine($"resultc is {resultC}");

            //string resultD = await DoSomething("D");
            string resultD = await MiddleLayer("D");
            Console.WriteLine($"resultd is {resultD}");

            Console.WriteLine($"finished in {sw.ElapsedMilliseconds}");
        }

        // doesn't need to be async
        static Task<string> MiddleLayer(string whereCalledFrom)
        {
            Console.WriteLine("Inside MiddleLater before DoSomething");
            // don't need a state machine as not awaiting anything before 
            Task<string> result = DoSomething(whereCalledFrom);
            return result;
        }

        static async Task<string> DoSomething(string whereCalledFrom)
        {
            Console.WriteLine($"inside DoSomething called from {whereCalledFrom}");
            await Task.Delay(5000);
            Console.WriteLine($"done {whereCalledFrom}");
            return $"result {whereCalledFrom}";
        }






        public static async Task<IEnumerable<Actor>> GetActors() =>
            await GetConnection().QueryAsync<Actor>(
                @"SELECT TOP 10 *
                FROM Actors");

        public static IDbConnection GetConnection()
        {
            var connection = new SqlConnection(ConnectionString);
            // don't need to connection.Open as Dapper will manage. 
            // https://stackoverflow.com/a/12629170/26086
            return connection;
        }


        public static Task<IEnumerable<Actor>> GetActorsB()
        {
            using (var connection = GetConnectionB())
            {
                // trying not to await this as don't need to? 
                // https://stackoverflow.com/a/15503860/26086
                var result = connection.QueryAsync<Actor>(
                    @"SELECT TOP 20 *
                    FROM Actors");
                return result;
            }
        }
        public static IDbConnection GetConnectionB()
        {
            var connection = new SqlConnection(ConnectionString);
            // don't need to connection.Open as Dapper will manage. 
            // https://stackoverflow.com/a/12629170/26086
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
    }
}
