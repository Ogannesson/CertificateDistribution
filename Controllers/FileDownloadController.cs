using Microsoft.AspNetCore.Mvc;

namespace CertificateDistribution.Controllers
{
    /// <summary>
    /// 文件下载控制器，用于处理文件下载请求。
    /// </summary>
    [ApiController]
    [Route("/")]
    public class FileDownloadController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// 初始化 <see cref="FileDownloadController"/> 的新实例。
        /// </summary>
        /// <param name="configuration">注入的配置对象，用于获取API密钥。</param>
        public FileDownloadController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// 处理GET请求以下载指定路径和文件名的文件。
        /// </summary>
        /// <param name="path">文件所在的相对路径。</param>
        /// <param name="file">要下载的文件名。</param>
        /// <param name="apiKey">请求头中的API密钥，用于验证请求。</param>
        /// <returns>返回文件作为下载或错误响应。</returns>
        [HttpGet]
        public IActionResult GetFile(
            [FromQuery] string path,
            [FromQuery] string file,
            [FromHeader(Name = "Authorization")] string apiKey)
        {
            // 获取配置中的API密钥
            var configuredApiKey = _configuration["ApiKey"];

            // 检查请求中的API密钥是否有效
            if (apiKey != configuredApiKey)
            {
                return Unauthorized(new { error = "Unauthorized access" });
            }

            // 文件根目录
            string rootDirectory = "/etc/letsencrypt/live";

            // 组合文件的完整路径
            string fullPath = Path.Combine(rootDirectory, path, file);

            // 检查文件是否存在
            if (!System.IO.File.Exists(fullPath))
            {
                return NotFound(new { error = "File not found" });
            }

            // 读取文件并返回
            var fileBytes = System.IO.File.ReadAllBytes(fullPath);
            return File(fileBytes, "application/octet-stream", file);
        }
    }
}