using GrainInterfaces;
using Grains;


await Host.CreateDefaultBuilder(args)
    .UseOrleans((ctx, siloBuilder) =>
    {
        if (ctx.HostingEnvironment.IsDevelopment())
        {
            siloBuilder.UseLocalhostClustering();
            siloBuilder.AddMemoryStreams("chat");
            siloBuilder.AddMemoryGrainStorage("PubSubStore");
        }
        else
        {
            /*// In Kubernetes, we use environment variables and the pod manifest
            siloBuilder.UseKubernetesHosting();

            // Use Redis for clustering & persistence
            var redisConnectionString = $"{Environment.GetEnvironmentVariable("REDIS")}:6379";
            siloBuilder.UseRedisClustering(options => options.ConnectionString = redisConnectionString);
            siloBuilder.AddRedisGrainStorage("definitions", options => options.ConnectionString = redisConnectionString);*/
        }
    })
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.ConfigureServices(services => services.AddControllers());
        webBuilder.Configure((ctx, app) =>
        {
            if (ctx.HostingEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        });
    })
    .ConfigureServices(services =>
    {
        services.AddSingleton<IUser, User>();
        services.AddSingleton<IChatRoom, ChatRoom>();
    })
    .RunConsoleAsync();


/*
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
*/