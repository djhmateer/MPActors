using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace MPActorsConsole
{
    class ProgramWC
    {
        public static void MainWC()
        {
            Console.WriteLine("Experimenting with a HOF for db connection and using statements");
            Console.WriteLine("so don't have code duplication");
            var actors = GetActors(ConnectionString);

            foreach (var actor in actors) Console.WriteLine(actor);
        }

        // Function 
        public static IEnumerable<Actor> GetActors(string connectionString)
            => WithConnection<IEnumerable<Actor>>(connectionString, conn =>
            {
               var result = conn.Query<Actor>(
                   @"SELECT TOP 10 *
                   FROM Actors");
               return result;
            });

        // Wrapper returns a generic T eg IEnumerable<Actor>
        // It takes as arguments: A Func takes an IDbConnection (which is what we make here) and returns a T of the same type
        public static T WithConnection<T>(
            string connectionString,
            Func<IDbConnection, T> func)
        {
            using var conn = new SqlConnection(connectionString);

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

        private const string ConnectionString = "Server=(localdb)\\mssqllocaldb;Database=IMDBChallenge;Trusted_Connection=True;MultipleActiveResultSets=true";

    }
}
