using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

public class HomeController : Controller
{
    private readonly SentimentService _service = new();

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Analizar(string opinion)
    {
        var resultado = _service.Predict(opinion);
        ViewBag.Texto = opinion;
        ViewBag.Resultado = resultado.Prediction ? "Positivo" : "Negativo";
        ViewBag.Score = resultado.Probability;
        return View("Resultado");
    }
}
