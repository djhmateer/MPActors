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

        private const string ConnectionString = "Server=(localdb)\\mssqllocaldb;Database=IMDBChallenge;Trusted_Connection=True;MultipleActiveResultSets=true";

    }
}
