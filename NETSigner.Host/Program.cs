var builder = WebApplication.CreateBuilder(args);

builder.Services.AddNETSigner(options => options.TimestampValidationOffset = TimeSpan.FromDays(5));

var app = builder.Build();

app.UseNETSigner();

app.Map("sign", context => context.Response.WriteAsync("> success"));

await app.RunAsync();

public partial class Program { }
