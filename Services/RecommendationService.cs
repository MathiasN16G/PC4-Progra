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

        // Verifica si el modelo existe, si no, lo entrena
        if (!File.Exists(_modelPath))
        {
            TrainModel();
        }

        using var stream = new FileStream(_modelPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        _model = _mlContext.Model.Load(stream, out _);
    }

    private void TrainModel()
    {
        var data = _mlContext.Data.LoadFromTextFile<RatingData>(
            "Data/ratings-data.csv", hasHeader: true, separatorChar: ',');

        var pipeline = _mlContext.Transforms.Conversion
            .MapValueToKey("UserId")
            .Append(_mlContext.Transforms.Conversion.MapValueToKey("ProductId"))
            .Append(_mlContext.Recommendation().Trainers.MatrixFactorization(new MatrixFactorizationTrainer.Options
            {
                MatrixColumnIndexColumnName = "UserId",
                MatrixRowIndexColumnName = "ProductId",
                LabelColumnName = "Label",
                NumberOfIterations = 20,
                ApproximationRank = 100
            }));

        var model = pipeline.Fit(data);
        _mlContext.Model.Save(model, data.Schema, _modelPath);
    }

    public List<string> GetRecommendations(string userId)
{
    var products = new[] { "P1", "P2", "P3", "P4", "P5" };
    var predictionResults = new List<(string productId, float score)>();

    var inputData = products.Select(p => new RatingData
    {
        UserId = userId,
        ProductId = p
    });

    var inputView = _mlContext.Data.LoadFromEnumerable(inputData);
    var transformedData = _model.Transform(inputView);
    var scoredData = _mlContext.Data.CreateEnumerable<ProductRatingPrediction>(transformedData, reuseRowObject: false).ToList();

    for (int i = 0; i < products.Length; i++)
    {
        predictionResults.Add((products[i], scoredData[i].Score));
    }

    return predictionResults
        .OrderByDescending(p => p.score)
        .Take(5)
        .Select(p => p.productId)
        .ToList();
}




}
