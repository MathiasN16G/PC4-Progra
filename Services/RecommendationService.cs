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
    try
    {
        var predictionEngine = _mlContext.Model.CreatePredictionEngine<RatingData, ProductRatingPrediction>(_model);

        var productos = new List<string>();

        for (int i = 1; i <= 5; i++)
{
    var productId = $"P{i}";
    var prediction = predictionEngine.Predict(new RatingData
    {
        UserId = userId,
        ProductId = productId
    });

    productos.Add($"{productId} (Score: {prediction.Score:F2})");
}
    

        return productos;
    }
    catch (Exception ex)
    {
        // log interno, o en tu vista puedes capturar esto
        return new List<string> { "Error al obtener recomendaciones", ex.Message };
    }
}





}
