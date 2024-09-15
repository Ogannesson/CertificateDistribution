var builder = WebApplication.CreateBuilder(args);

// ��ӿ�����֧��
builder.Services.AddControllers();

var app = builder.Build();

// �м������
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();