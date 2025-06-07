using Microsoft.ML;
using Microsoft.ML.Trainers;
using System.Collections.Generic;
using System.Linq;

public class RecommendationService
{
    private readonly string _modelPath = "Data/recommendation-model.zip";
    private readonly MLContext _mlContext;
    private ITransformer _model;

    public RecommendationService()
    {
        _mlContext = new MLContext();
        TrainModel();
    }

    private void TrainModel()
    {
        var data = _mlContext.Data.LoadFromTextFile<RatingData>(
            "Data/ratings-data.csv", hasHeader: true, separatorChar: ',');

        var dataView = _mlContext.Transforms.Conversion
            .MapValueToKey("UserId")
            .Append(_mlContext.Transforms.Conversion.MapValueToKey("ProductId"))
            .Fit(data)
            .Transform(data);

        var options = new MatrixFactorizationTrainer.Options
        {
            MatrixColumnIndexColumnName = "UserId",
            MatrixRowIndexColumnName = "ProductId",
            LabelColumnName = "Label",
            NumberOfIterations = 20,
            ApproximationRank = 100
        };

        var estimator = _mlContext.Recommendation().Trainers.MatrixFactorization(options);
        _model = estimator.Fit(dataView);
    }

    public List<string> GetRecommendations(string userId)
    {
        var products = new[] { "P1", "P2", "P3", "P4", "P5" };
        var predictions = new List<(string productId, float score)>();

        foreach (var productId in products)
        {
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<RatingData, ProductRatingPrediction>(_model);
            var prediction = predictionEngine.Predict(new RatingData { UserId = userId, ProductId = productId });
            predictions.Add((productId, prediction.Score));
        }

        return predictions
            .OrderByDescending(p => p.score)
            .Take(5)
            .Select(p => p.productId)
            .ToList();
    }
}
