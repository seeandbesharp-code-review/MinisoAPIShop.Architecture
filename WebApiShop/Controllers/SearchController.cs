
using DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;
using System.Net.Http.Json;

namespace WebApiShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly HttpClient _http;
        private readonly IProductService _products; // השירות הקיים שלך שמביא מוצרים מה-DB

        // הזרקת ה-HttpClient והשירות של המוצרים שלך
        public SearchController(IHttpClientFactory f, IProductService products)
        {
            _http = f.CreateClient();
            _products = products;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SearchQuery req)
        {
            // 1. משיכת כל המוצרים האמיתיים מה-DB שלך
            var products = await _products.GetProducts(position: 1, skip: 100, parameters: new ProductSearchParams());

            // 2. סינון הנתונים - שולחים לפייתון רק מה שהמודל צריך כדי לחסוך במקום (Payload קטן)
            var productList = products.Data.Select(p => new {
                p.ProductName,
                p.Price,
                p.Description,
                p.ImageUrl,
                p.ProductId
            }).ToList();

            // 3. שליחת השאילתה והמוצרים לשרת הפייתון שהרמנו הרגע (פורט 8001)
            var res = await _http.PostAsJsonAsync(
                "http://localhost:8001/search",
                new { query = req.Query, products = productList, top_k = 5 });

            // 4. קריאת התשובה שחזרה מפייתון והחזרתה לאנגולר
            var data = await res.Content.ReadFromJsonAsync<SearchResponse>();
            return Ok(data);
        }
    }

    // ה-Records המשמשים לקבלת ושליחת הנתונים
    public record SearchQuery(string Query);
    public record SearchResponse(List<object> Results);
}