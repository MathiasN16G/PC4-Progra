using Microsoft.ML;
using System.IO;

public class SentimentService
{
    private readonly string _modelPath = "Data/sentiment-model.zip";
    private readonly PredictionEngine<SentimentModelInput, SentimentModelOutput> _predEngine;

    public SentimentService()
    {
        var mlContext = new MLContext();

        if (!File.Exists(_modelPath))
        {
            var data = mlContext.Data.LoadFromTextFile<SentimentModelInput>(
                "Data/sentiment-data.tsv", hasHeader: true);

            var pipeline = mlContext.Transforms.Text.FeaturizeText("Features", nameof(SentimentModelInput.Text))
                .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression());

            var model = pipeline.Fit(data);
            mlContext.Model.Save(model, data.Schema, _modelPath);
        }

        var loadedModel = mlContext.Model.Load(_modelPath, out _);
        _predEngine = mlContext.Model.CreatePredictionEngine<SentimentModelInput, SentimentModelOutput>(loadedModel);
    }

    public SentimentModelOutput Predict(string texto)
    {
        return _predEngine.Predict(new SentimentModelInput { Text = texto });
    }
}
