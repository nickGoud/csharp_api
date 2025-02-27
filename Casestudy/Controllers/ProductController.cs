using Casestudy.DAL;
using Casestudy.DAL.DAO;
using Casestudy.DAL.DomainClasses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Casestudy.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        readonly AppDbContext _db;
        public ProductController(AppDbContext context)
        {
            _db = context;
        }
        [HttpGet]
        [Route("{branid}")]
        public async Task<ActionResult<List<Product>>> Index(int branid)
        {
            ProductDAO dao = new(_db);
            List<Product> itemsForBrand = await dao.GetAllByBrand(branid);
            return itemsForBrand;
        }
    }
}
