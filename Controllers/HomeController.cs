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
    [HttpPost]
public IActionResult Analizar(string opinion, string userId)
{
    var resultado = _service.Predict(opinion);
    ViewBag.Texto = opinion;

    
    ViewBag.Resultado = resultado.Prediction ? "Negativo" : "Positivo";
    
    ViewBag.Score = resultado.Probability;
    ViewBag.UserId = userId;
    return View("Resultado");
}

}
