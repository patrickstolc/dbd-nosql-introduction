using MongoExample.Core.Repositories;
using MongoExample.Core.Services;

var builder = WebApplication.CreateBuilder(args);

// Register repositories
builder.Services.AddScoped<PostRepository>();
builder.Services.AddScoped<UserRepository>();

// Register services
builder.Services.AddScoped<PostService>();
builder.Services.AddScoped<UserService>();

// Add after other service registrations
builder.Services.AddSingleton<IRedisService>(sp => 
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetValue<string>("Redis:ConnectionString");
    return new RedisService(connectionString);
});

builder.Services.AddControllers();
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

app.UseAuthorization();

app.MapControllers();

app.Run();