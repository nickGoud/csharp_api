using Casestudy.DAL;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;


namespace Casestudy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        readonly AppDbContext? _ctx;
        public DataController(AppDbContext context) // injected here
        {
            _ctx = context;
        }

        private async Task<String> GetMenuItemJsonFromWebAsync()
        {
            
            string jsonString = "";

            using (StreamReader r = new StreamReader("Controllers\\product_catalog.json"))
            {
                jsonString = await r.ReadToEndAsync();
            }

            return jsonString;
        }

        [HttpGet]
        public async Task<ActionResult<String>> Index()
        {
            DataUtility util = new(_ctx!);
            string payload = "";
            var json = await GetMenuItemJsonFromWebAsync();
            try
            {
                payload = (await util.LoadNutritionInfoFromWebToDb(json)) ? "tables loaded" : "problem loading tables";
            }
            catch (Exception ex)
            {
                payload = ex.Message;
            }
            return JsonSerializer.Serialize(payload);
        }
    }
}
