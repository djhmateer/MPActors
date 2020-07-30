using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Dapper;

namespace MPActorsConsole
{
    class Program
    {
        public static async Task Main()
        {
            Console.WriteLine("Testing async more");
            //var actors = await GetActors();
            //foreach (var actor in actors) Console.WriteLine(actor);

            var actorsb = await GetActorsB();
            foreach (var actor in actorsb) Console.WriteLine(actor);

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


        public static async Task<IEnumerable<Actor>> GetActorsB()
        {
            using (var connection = GetConnectionB())
            {
                // trying not to await this as don't need to? 
                // https://stackoverflow.com/a/15503860/26086
                var result = await connection.QueryAsync<Actor>(
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
            //connection.Open();
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
