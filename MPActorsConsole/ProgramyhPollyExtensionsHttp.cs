using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Resources;
using System.Threading.Tasks;
using Dapper;
using MPActorsConsole.Polly;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using Xunit;
using Xunit.Abstractions;

namespace MPActorsConsole
{
    public class HttpPollyExtensionsHttp
    {
        // test the Http polly
        // copying the code in the Polly-Samples project
        // and using their webapi test server
        // https://localhost:44307

        public static async Task Main()
        {
            //await NoPolicy();
            //await RetryNTimes();
            //await WaitAndRetryNTimes();
            //await WaitAndRetryNTimesWithEnoughRetries();
            //await WaitAndRetryNTimesWithExponentialBackOff();
            await PollyExtensions();
        }

        private static async Task PollyExtensions()
        {
            var httpClient = new HttpClient();
            var url = "https://localhost:44307/api/values";

            // Handles HttpRequestException, Http status codes >= 500 (server errors) and status code 408 (request timeout)
            // A very simple retry-3-times. See https://github.com/App-vNext/Polly for the many richer Policy configuration options available.
            var policy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .RetryAsync(3);

            // Define our policy:
            //var policy = Policy.Handle<Exception>().WaitAndRetryAsync(
            //    6, // Could do forever ie miss this out, but good to do this ie max of 6400ms then it will fail
            //    attempt => TimeSpan.FromSeconds(0.1 * Math.Pow(2,
            //                                             attempt)), // Back off! 200,400,800,1600ms etc.. 
            //    (exception, calculatedWaitDuration) => 
            //{
            //    Console.WriteLine($"{exception.Message} : Auto delay for {calculatedWaitDuration.TotalMilliseconds}ms");
            //});

            while (true)
            {
                try
                {
                    await policy.ExecuteAsync(async () =>
                    {
                        var httpResponseMessage = await httpClient.GetAsync(url);
                        return httpResponseMessage;

                        // throws if .IsSuccessStatusCode property is false
                        httpResponseMessage.EnsureSuccessStatusCode();

                        var sc = httpResponseMessage.StatusCode;
                        Console.WriteLine($"Status code is {(int)sc}{sc}");

                        var content = await httpResponseMessage.Content.ReadAsStringAsync();
                        Console.WriteLine($"Content is {content}");
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Request eventually failed with {ex.Message}");
                }

                await Task.Delay(500);
            }
        }


        private static async Task WaitAndRetryNTimesWithExponentialBackOff()
        {
            var httpClient = new HttpClient();
            var url = "https://localhost:44307/api/values";

            // Define our policy:
            var policy = Policy.Handle<Exception>().WaitAndRetryAsync(
                6, // Could do forever ie miss this out, but good to do this ie max of 6400ms then it will fail
                attempt => TimeSpan.FromSeconds(0.1 * Math.Pow(2,
                                                         attempt)), // Back off! 200,400,800,1600ms etc.. 
                (exception, calculatedWaitDuration) =>
            {
                Console.WriteLine($"{exception.Message} : Auto delay for {calculatedWaitDuration.TotalMilliseconds}ms");
            });

            while (true)
            {
                try
                {
                    await policy.ExecuteAsync(async () =>
                    {
                        var httpResponseMessage = await httpClient.GetAsync(url);

                        // throws if .IsSuccessStatusCode property is false
                        httpResponseMessage.EnsureSuccessStatusCode();

                        var sc = httpResponseMessage.StatusCode;
                        Console.WriteLine($"Status code is {(int)sc}{sc}");

                        var content = await httpResponseMessage.Content.ReadAsStringAsync();
                        Console.WriteLine($"Content is {content}");
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Request eventually failed with {ex.Message}");
                }

                await Task.Delay(500);
            }
        }

        private static async Task WaitAndRetryNTimesWithEnoughRetries()
        {
            var httpClient = new HttpClient();
            var url = "https://localhost:44307/api/values";

            // Define our policy:
            var policy = Policy.Handle<Exception>().WaitAndRetryAsync(
                20, // Retry up to 20 times - should be enough that we eventually succeed
                attempt => TimeSpan.FromMilliseconds(500), // Wait 200ms between each retry
                (exception, calculationWithDuration) => // capture info for logging
            {
                Console.WriteLine($"Retrying after 200ms: {exception.Message} : {calculationWithDuration}");
            });

            while (true)
            {
                try
                {
                    await policy.ExecuteAsync(async () =>
                    {
                        var httpResponseMessage = await httpClient.GetAsync(url);
                        // GetStringAsync throws if not a success
                        //var content = await httpClient.GetStringAsync(url);

                        // throws if .IsSuccessStatusCode property is false
                        httpResponseMessage.EnsureSuccessStatusCode();

                        var sc = httpResponseMessage.StatusCode;
                        Console.WriteLine($"Status code is {(int)sc}{sc}");

                        var content = await httpResponseMessage.Content.ReadAsStringAsync();
                        Console.WriteLine($"Content is {content}");
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Request eventually failed with {ex.Message}");
                }

                await Task.Delay(500);
            }
        }


        private static async Task WaitAndRetryNTimes()
        {
            var httpClient = new HttpClient();
            var url = "https://localhost:44307/api/values";

            // Define our policy:
            var policy = Policy.Handle<Exception>().WaitAndRetryAsync(
                3, // Retry 3 times
                attempt => TimeSpan.FromMilliseconds(500), // Wait 200ms between each retry
                (exception, calculationWithDuration) => // capture info for logging
            {
                Console.WriteLine($"Retrying after 200ms: {exception.Message} : {calculationWithDuration}");
            });

            while (true)
            {
                try
                {
                    await policy.ExecuteAsync(async () =>
                    {
                        var httpResponseMessage = await httpClient.GetAsync(url);
                        // GetStringAsync throws if not a success
                        //var content = await httpClient.GetStringAsync(url);

                        // throws if .IsSuccessStatusCode property is false
                        httpResponseMessage.EnsureSuccessStatusCode();

                        var sc = httpResponseMessage.StatusCode;
                        Console.WriteLine($"Status code is {(int)sc}{sc}");

                        var content = await httpResponseMessage.Content.ReadAsStringAsync();
                        Console.WriteLine($"Content is {content}");
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Request eventually failed with {ex.Message}");
                }

                await Task.Delay(500);
            }
        }

        private static async Task RetryNTimes()
        {
            var httpClient = new HttpClient();
            var url = "https://localhost:44307/api/values";

            // Define our policy:
            var policy = Policy.Handle<Exception>().RetryAsync(3, (exception, attempt) =>
            {
                // This is your new exception handler! 
                Console.WriteLine($"Retrying immediately: {exception.Message} attempt: {attempt}");
            });

            while (true)
            {
                try
                {
                    await policy.ExecuteAsync(async () =>
                    {
                        var httpResponseMessage = await httpClient.GetAsync(url);
                        // GetStringAsync throws if not a success
                        //var content = await httpClient.GetStringAsync(url);

                        Console.WriteLine($"{httpResponseMessage.IsSuccessStatusCode}");

                        // throws if .IsSuccessStatusCode property is false
                        httpResponseMessage.EnsureSuccessStatusCode();

                        var sc = httpResponseMessage.StatusCode;
                        Console.WriteLine($"Status code is {(int)sc}{sc}");

                        var content = await httpResponseMessage.Content.ReadAsStringAsync();
                        Console.WriteLine($"Content is {content}");
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Request eventually failed with {ex.Message}");
                }

                Console.WriteLine("here");

                await Task.Delay(500);
            }
        }

        private static async Task NoPolicy()
        {
            var httpClient = new HttpClient();

            var url = "https://localhost:44307/api/values";

            var totalRequests = 0;
            while (true)
            {
                var httpResponseMessage = await httpClient.GetAsync(url);
                totalRequests++;

                var sc = httpResponseMessage.StatusCode;
                Console.WriteLine($"Status code is {(int)sc}{sc} {totalRequests}");

                var content = await httpResponseMessage.Content.ReadAsStringAsync();
                Console.WriteLine($"Content is {content}");

                await Task.Delay(1000);
            }
        }
    }
}
