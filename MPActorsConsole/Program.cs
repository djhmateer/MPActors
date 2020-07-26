using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace MPActorsConsole
{
    class ProgramA
    {
        public static void MainA()
        {
            var actors = GetActors();

            foreach (var actor in actors)
            {
                Console.WriteLine(actor);
            }
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
            var connection = new SqlConnection("Server=(localdb)\\mssqllocaldb;Database=IMDBChallenge;Trusted_Connection=True;MultipleActiveResultSets=true");
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

    }
}
