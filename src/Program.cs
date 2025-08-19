var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();

var app = builder.Build();
app.MapOpenApi();

app.MapGet("/{input}", (string input) =>
{
    return TypedResults.Ok(new Echo(input));
})
.WithName("Echo");

await app.RunAsync();

public record Echo(string Message);