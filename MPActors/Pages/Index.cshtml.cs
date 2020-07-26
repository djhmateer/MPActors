using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Dapper;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using StackExchange.Profiling;
using StackExchange.Profiling.Data;

namespace MPActors.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public IEnumerable<Actor> Actors { get; set; }

        public void OnGet()
        {
            var mp = MiniProfiler.Current;
            //using (mp.Step("Level 1"))
            using (var connection = GetConnection())
            {
                //connection.Query<long>("select 1");

                //using (mp.Step("Level 2"))
                //{
                //    connection.Query<long>("select 1");
                //}

                List<Actor> actors = connection.Query<Actor>("SELECT TOP 10 * FROM Actors").ToList();

                Actors = actors;
            }

        }
        public DbConnection GetConnection(MiniProfiler profiler = null)
        {
            using (profiler.Step(nameof(GetConnection)))
            {
                DbConnection cnn = new System.Data.SqlClient.SqlConnection("Server=(localdb)\\mssqllocaldb;Database=IMDBChallenge;Trusted_Connection=True;MultipleActiveResultSets=true");

                // to get profiling times, we have to wrap whatever connection we're using in a ProfiledDbConnection
                // when MiniProfiler.Current is null, this connection will not record any database timings
                if (MiniProfiler.Current != null)
                {
                    cnn = new ProfiledDbConnection(cnn, MiniProfiler.Current);
                }

                cnn.Open();
                return cnn;
            }
        }
    }

    public class Actor
    {
        public int actorid { get; set; }
        public string name { get; set; }
        public string sex { get; set; }

        public override string ToString() => $"{actorid} {name} {sex}";
    }
}
