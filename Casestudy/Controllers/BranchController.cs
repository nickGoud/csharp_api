using Casestudy.DAL;
using Casestudy.DAL.DAO;
using CaseStudyAPI.DAL.DomainClasses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Casestudy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class BranchController : ControllerBase
    {
        readonly AppDbContext _db;
        public BranchController(AppDbContext context)
        {
            _db = context;
        }
        [AllowAnonymous]
        [HttpGet("{lat}/{lon}")]
        public async Task<ActionResult<List<Branch>?>> Index(float lat, float lon)
        {
            BranchDAO dao = new(_db);
            return await dao.GetThreeClosestBranches(lat, lon);
        }
    }
}
