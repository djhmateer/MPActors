using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;

namespace MPActorsConsole
{
    class Program
    {
        public static async Task Main()
        {
            Console.WriteLine("Testing a more traditional way with async");
            var actors = await GetActorsAsync();

            foreach (var actor in actors)  Console.WriteLine(actor); 
        }

        public static async Task<IEnumerable<Actor>> GetActorsAsync()
        {
            using var connection = await GetOpenConnectionAsync();
            return await connection.QueryAsync<Actor>(
                @"SELECT TOP 10 *
                FROM Actors");
        }

        public static async Task<IDbConnection> GetOpenConnectionAsync()
        {
            var connection = new SqlConnection(ConnectionString);
            // do I need this?
            //await connection.OpenAsync();
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
