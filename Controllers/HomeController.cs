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
    public IActionResult Analizar(string opinion, string userId)
    {
        var resultado = _service.Predict(opinion);
        ViewBag.Texto = opinion;
        ViewBag.Resultado = resultado.Prediction ? "Positivo" : "Negativo";
        ViewBag.Score = resultado.Probability;
        ViewBag.UserId = userId;
        return View("Resultado");
    }

    [HttpGet]
    public IActionResult Recomendacion(string userId) 
    {
        ViewBag.UserId = userId; 
        return View();
    }

    [HttpPost]
    public IActionResult RecomendacionPost(string userId)
    {
        var servicio = new RecommendationService();
        var productos = servicio.GetRecommendations(userId);
        ViewBag.UserId = userId;
        ViewBag.Productos = productos;
        return View("Recomendacion"); 

    }
}
