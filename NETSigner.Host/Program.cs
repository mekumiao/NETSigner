var builder = WebApplication.CreateBuilder(args);

builder.Services.AddNETSigner(options => options.TimestampValidationOffset = TimeSpan.FromDays(5));

var app = builder.Build();

app.UseNETSigner();

app.Map("sign", context => context.Response.WriteAsync("> success"));

await app.RunAsync();

#pragma warning disable CA1050 // 在命名空间中声明类型
public partial class Program { }
