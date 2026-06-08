using WorkTrack.Application;
using WorkTrack.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();

	app.UseSwaggerUI(options =>
	{
		options.SwaggerEndpoint(
			"/openapi/v1.json",
			"WorkTrack API v1");

		options.RoutePrefix = "swagger";
	});
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();