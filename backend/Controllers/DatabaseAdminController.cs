using backend.Interface;
using backend.Dto;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("db")]
    public class DatabaseAdminController : ControllerBase
    {
        private readonly IDatabaseAdminDao _dao;
        public DatabaseAdminController(IDatabaseAdminDao dao) => _dao = dao;

        [HttpPost("reset")]
        public async Task<ActionResult<DatabaseResetResultDto>> Reset([FromBody] DatabaseResetRequestDto body)
        {
            try
            {
                var result = await _dao.ResetAsync(body);
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
    }
}
