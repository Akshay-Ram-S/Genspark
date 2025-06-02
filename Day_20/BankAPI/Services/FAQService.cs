using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.Text;
using System.Collections.Generic;
using System.Linq;
using BankAPI.Models;

namespace BankAPI.Services
{
    public class FAQService
    {
        private readonly MLContext _mlContext;
        private readonly Dictionary<string, string> _customData;
        private readonly PredictionEngine<TextData, TextVector> _predictionEngine;
        private List<ChatMessage> chatHistory;

        public FAQService()
        {
            _mlContext = new MLContext();
            chatHistory = new List<ChatMessage>();
            _customData = new Dictionary<string, string>
            {
                { "How can I open an account?", "You can open a bank account by visiting a branch or applying online." },
                { "What is the bank timings?", "We are open from 9 AM to 5 PM, Monday to Friday." },
                { "How to contact support?", "You can contact support via bank@gmail.com or call 100-200-3000." },
                { "Where is the office headquarters?", "Our headquarters is located at New Delhi." },
                { "How can I apply credit card?", "You can apply for a credit card through internet banking, mobile app, or by visiting a branch. Please feel free to check various cards in the cards section." },
                { "What is internet banking?", "Internet banking allows you to perform financial transactions online via your bank's website or app" },
                { "Thank", "Thank you, Have a nice day!" }
            };

            var pipeline = _mlContext.Transforms.Text
                            .NormalizeText("NormalizedText", nameof(TextData.Text))
                            .Append(_mlContext.Transforms.Text.TokenizeIntoWords("Tokens", "NormalizedText"))
                            .Append(_mlContext.Transforms.Text.ApplyWordEmbedding(
                            outputColumnName: "Features",
                            inputColumnName: "Tokens",
                            modelKind: WordEmbeddingEstimator.PretrainedModelKind.GloVe300D));


            var sampleData = _customData.Keys.Select(q => new TextData { Text = q }).ToList();
            var model = pipeline.Fit(_mlContext.Data.LoadFromEnumerable(sampleData));


            _predictionEngine = _mlContext.Model.CreatePredictionEngine<TextData, TextVector>(model);
        }


        public string GetResponse(string userQuestion)
        {
            var inputVector = _predictionEngine.Predict(new TextData { Text = userQuestion }).Features;

            var bestMatch = _customData.Keys
                .Select(q => new
                {
                    Question = q,
                    Vector = _predictionEngine.Predict(new TextData { Text = q }).Features,
                    Similarity = CosineSimilarity(_predictionEngine.Predict(new TextData { Text = q }).Features, inputVector)
                })
                .OrderByDescending(x => x.Similarity)
                .FirstOrDefault();

            string response;
            if (bestMatch != null && bestMatch.Similarity >= 0.80f)
            {
                response = _customData[bestMatch.Question];
            }
            else
            {
                response = "Sorry, I don't have the required knowledge to answer this query. Please contact us via bank@gmail.com or call 100-200-3000.";
            }

            chatHistory.Add(new ChatMessage { Sender = "User", Message = userQuestion });
            chatHistory.Add(new ChatMessage { Sender = "Bot", Message = response });
            return response;
        }

        private float CosineSimilarity(float[] vectorA, float[] vectorB)
        {
            float dotProduct = vectorA.Zip(vectorB, (a, b) => a * b).Sum();
            float magnitudeA = (float)MathF.Sqrt(vectorA.Select(a => a * a).Sum());
            float magnitudeB = (float)MathF.Sqrt(vectorB.Select(b => b * b).Sum());
            return dotProduct / (magnitudeA * magnitudeB);
        }

        public List<ChatMessage> GetChatHistory()
        {
            return chatHistory;
        }
    }

}
