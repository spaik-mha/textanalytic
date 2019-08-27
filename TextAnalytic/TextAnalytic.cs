using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TextAnalytic
{
    public class TextAnalytic
    {
        private bool _defaultProcedure = true;
        private string _filePath = "";
        private TextAnalyticsClient _client;
        private string _apiKey;
        private string _endpoint;

        public TextAnalytic( string apikey, string endpoint, bool isDemo = true, string filePath = "")
        {
            _defaultProcedure = isDemo;
            _filePath = filePath;
            _apiKey = apikey;
            _endpoint = endpoint;
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

        public void Run()
        {
            if (_defaultProcedure)
            {
                RunDemo();
            }
            else
            {
                ReadFileSentiment();
            }
        }

        private void RunDemo()
        {
            MultiLanguageBatchInput batchInputs = new MultiLanguageBatchInput()
            {
                Documents = new List<MultiLanguageInput>()
                    {
                            new MultiLanguageInput()
                        {
                            Id = "Positive Sentence",
                            Text = "I had the best day of my life."
                        },
                        new MultiLanguageInput()
                        {
                            Id = "Negative Sentence",
                            Text = "This was the worst pie."
                        },
                        new MultiLanguageInput()
                        {
                            Id = "Neutral Sentence",
                            Text = "It rains in Spain."
                        },
                        new MultiLanguageInput()
                        {
                            Id = "Positive Text",
                            Text = "I went to the circus the other day. The train was late. I stubbed my toe. But, overall, it wasn't that bad."
                        },
                        new MultiLanguageInput()
                        {
                            Id = "Bad Grammar",
                            Text = "If I had before known that what was truthfully stated was not I would not have trusted thee."
                        },
                        new MultiLanguageInput()
                        {
                            Id = "Sarcasm",
                            Text = "Wow, the circus was really crowded and hot. Someone stepped on my foot. Best day ever."
                        },
                    }
            };

            var sentimentPrediction = _client.SentimentBatch(batchInputs);
            foreach (var prediction in sentimentPrediction.Documents)
            {
                var sentiment = prediction.Score;
                var id = prediction.Id;
                var text = batchInputs.Documents.Where(x => x.Id == id).Select(x => x.Text).FirstOrDefault();
                Console.WriteLine($"Id: {id}");
                Console.WriteLine($"Text: {text}");
                Console.WriteLine($"Sentiment: {sentiment}");
                Console.WriteLine("-------------------------------------------------------------------");
            }
        }

        private void ReadFileSentiment()
        {
            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(_filePath))
                {
                    // Read the stream to a string, and write the string to the console.
                    String line = sr.ReadToEnd();
                    var sentiment = _client.Sentiment(line);
                    Console.WriteLine("-------------------------------------------------------------------");
                    Console.WriteLine($"Text: {line}");
                    Console.WriteLine($"Sentiment: {sentiment.Score}");
                    Console.WriteLine("-------------------------------------------------------------------");
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
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


