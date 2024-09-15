var builder = WebApplication.CreateBuilder(args);
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolver = null;
});
var app = builder.Build();

// 获取配置对象
var configuration = builder.Configuration;

// 定义下载文件的 API 路由
app.MapGet("/", (string path, string file, HttpRequest request) =>
{
    // 从配置中获取 API 密钥
    var configuredApiKey = configuration["ApiKey"];

    // 从请求头中获取 API 密钥
    var apiKey = request.Headers.Authorization.ToString();
    // 检查 API 密钥是否匹配
    if (apiKey != configuredApiKey)
    {
        return Results.Problem(
            detail: "Unauthorized access",
            statusCode: StatusCodes.Status401Unauthorized
        );
    }

    // 文件根目录
    string rootDirectory = "/etc/letsencrypt/live";

    // 组合文件的完整路径
    string fullPath = Path.GetFullPath(Path.Combine(rootDirectory, path, file));

    // 验证文件路径是否在允许的根目录内
    if (!fullPath.StartsWith(rootDirectory))
    {
        return Results.Problem(
            detail: "Invalid file path",
            statusCode: StatusCodes.Status400BadRequest
        );
    }

    // 检查文件是否存在
    if (!System.IO.File.Exists(fullPath))
    {
        return Results.Problem(
            detail: "File not found",
            statusCode: StatusCodes.Status404NotFound
            );
    }

    // 读取文件内容并返回作为下载
    var fileBytes = System.IO.File.ReadAllBytes(fullPath);

    // 固定 MIME 类型为 application/x-pem-file
    string mimeType = "application/x-pem-file";

    return Results.File(fileBytes, mimeType, file);
});

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.Run();