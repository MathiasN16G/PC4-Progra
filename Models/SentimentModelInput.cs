using Microsoft.ML.Data;

public class SentimentModelInput
{
    [LoadColumn(0)]
    public bool Label { get; set; }

    [LoadColumn(1)]
    public string Text { get; set; }
}
