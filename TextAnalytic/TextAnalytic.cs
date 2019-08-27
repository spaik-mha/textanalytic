using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using Microsoft.Rest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TextAnalytic
{
    public class TextAnalytic
    {
        private readonly string _filePath = "";
        private TextAnalyticsClient _client;
        private readonly string _apiKey;
        private readonly string _endpoint;
        private readonly string connectionString;

        public TextAnalytic(string apikey, string endpoint, string connection, string filePath = "")
        {
            _filePath = filePath;
            _apiKey = apikey;
            _endpoint = endpoint;
            connectionString = connection;
            InitializeClient();
        }

        private void InitializeClient()
        {
            var credentials = new ApiKeyServiceClientCredentials(_apiKey);
            _client = new TextAnalyticsClient(credentials)
            {
                Endpoint = _endpoint
            };

        }

        public string Run()
        {
            return String.IsNullOrWhiteSpace(_filePath) ? RunDemo() : ReadFileSentiment();
        }

        private string RunDemo()
        {
            MultiLanguageBatchInput batchInputs = new MultiLanguageBatchInput()
            {
                Documents = FetchCommentsFromDB(connectionString)
            };
            var sentimentPrediction = _client.SentimentBatch(batchInputs);
            string analysis = JsonConvert.SerializeObject(sentimentPrediction);

            return analysis;
        }

        private string ReadFileSentiment()
        {
            string analysis = "";
            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(_filePath))
                {
                    // Read the stream to a string, and write the string to the console.
                    String line = sr.ReadToEnd();
                    var sentiment = _client.Sentiment(line);
                    analysis = JsonConvert.SerializeObject(sentiment);
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            return analysis;
        }

        private List<MultiLanguageInput> FetchCommentsFromDB(string ConnectionString)
        {
            List<MultiLanguageInput> comments = new List<MultiLanguageInput>();
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                    using (var fetchComments = new SqlCommand("SELECT TOP 100 [AP_summary] AS [Text], [Ap_ApId] AS [ID] FROM [dbo].[IM_Appointments] WHERE [AP_summary] IS NOT NULL", connection))
                    {
                        using (var commentReader = fetchComments.ExecuteReader())
                        {
                            while (commentReader.Read())
                            {
                                var comment = new MultiLanguageInput()
                                {
                                    Id = commentReader["ID"].ToString(),
                                    Text = commentReader["Text"].ToString()
                                };
                                comments.Add(comment);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Couldn't read from DB");
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
            return comments;
        }

        private List<MultiLanguageInput> FetchCommentsFromDB(SqlConnection connection)
        {
            List<MultiLanguageInput> comments = new List<MultiLanguageInput>();
            try
            {
                connection.Open();
                using (var fetchComments = new SqlCommand("SELECT [AP_summary] AS [Text], [Ap_ApId] AS [ID] FROM [dbo].[IM_Appointments] WHERE [AP_summary] IS NOT NULL", connection))
                {
                    using (var commentReader = fetchComments.ExecuteReader())
                    {
                        while (commentReader.Read())
                        {
                            var comment = new MultiLanguageInput()
                            {
                                Id = commentReader["ID"].ToString(),
                                Text = commentReader["Text"].ToString()
                            };
                            comments.Add(comment);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Couldn't read from DB");
                Console.WriteLine(ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return comments;
        }

    }

    //static void SentimentAnalysis(TextAnalyticsClient client)
    //{
    //    var result = client.Sentiment("I had the best day of my life.", "en");
    //    Console.WriteLine($"Sentiment Score: {result.Score:0.00}");
    //}

    class ApiKeyServiceClientCredentials : ServiceClientCredentials
    {
        private readonly string apiKey;

        public ApiKeyServiceClientCredentials(string apiKey)
        {
            this.apiKey = apiKey;
        }

        public override Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }
            request.Headers.Add("Ocp-Apim-Subscription-Key", this.apiKey);
            return base.ProcessHttpRequestAsync(request, cancellationToken);
        }
    }
}


