var builder = WebApplication.CreateBuilder(args);
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolver = null;
});
var app = builder.Build();

// ��ȡ���ö���
var configuration = builder.Configuration;

// ���������ļ��� API ·��
app.MapGet("/", (string path, string file, HttpRequest request) =>
{
    // �������л�ȡ API ��Կ
    var configuredApiKey = configuration["ApiKey"];

    // ������ͷ�л�ȡ API ��Կ
    var apiKey = request.Headers.Authorization.ToString();
    // ��� API ��Կ�Ƿ�ƥ��
    if (apiKey != configuredApiKey)
    {
        return Results.Problem(
            detail: "Unauthorized access",
            statusCode: StatusCodes.Status401Unauthorized
        );
    }

    // �ļ���Ŀ¼
    string rootDirectory = "/etc/letsencrypt/live";

    // ����ļ�������·��
    string fullPath = Path.GetFullPath(Path.Combine(rootDirectory, path, file));

    // ��֤�ļ�·���Ƿ�������ĸ�Ŀ¼��
    if (!fullPath.StartsWith(rootDirectory))
    {
        return Results.Problem(
            detail: "Invalid file path",
            statusCode: StatusCodes.Status400BadRequest
        );
    }

    // ����ļ��Ƿ����
    if (!System.IO.File.Exists(fullPath))
    {
        return Results.Problem(
            detail: "File not found",
            statusCode: StatusCodes.Status404NotFound
            );
    }

    // ��ȡ�ļ����ݲ�������Ϊ����
    var fileBytes = System.IO.File.ReadAllBytes(fullPath);

    // �̶� MIME ����Ϊ application/x-pem-file
    string mimeType = "application/x-pem-file";

    return Results.File(fileBytes, mimeType, file);
});

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.Run();