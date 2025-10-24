using DeviceRepoAspNetCore.Services;
using Microsoft.AspNetCore.Mvc;

namespace DeviceRepoAspNetCore.Controllers
{
    // For detailed API documentation, including endpoint descriptions, request/response formats, 
    // and examples, please refer to the `rest-api-documentation.md` file located in the project root.
    [ApiController]
    [Route("api/[controller]")]
    public class InfoController(VersionProvider versionProvider) : ControllerBase
    {
        [HttpGet("version")]
        public IActionResult GetVersion()
        {
            return Ok(new
            {
                releaseVersion = versionProvider.CodeVersion,
                lastCommitDate = versionProvider.LastCommitDate,
                runtime = versionProvider.Runtime
            });
        }
    }
}
