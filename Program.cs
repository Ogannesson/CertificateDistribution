var builder = WebApplication.CreateBuilder(args);

// 添加控制器支持
builder.Services.AddControllers();

var app = builder.Build();

// 中间件配置
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();