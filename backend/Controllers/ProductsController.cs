using Microsoft.AspNetCore.Mvc;
using backend.Interface;

namespace backend.Controllers
{
    [ApiController]
    [Route("products")]
    public class ProductsController : ControllerBase
    {
        private readonly IShopDao _dao;
        public ProductsController(IShopDao dao) => _dao = dao;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _dao.GetAllProductsAsync());
    }
}
