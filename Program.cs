using MongoDB.Driver;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddSingleton<MongoClient>(_ => new MongoClient());
builder.Services.AddSingleton<IMongoDatabase>(
    provider => provider.GetRequiredService<MongoClient>().GetDatabase("room-temp-db"));
builder.Services.AddSingleton<IMongoCollection<RoomTemperature>>(
    provider => provider.GetRequiredService<IMongoDatabase>().GetCollection<RoomTemperature>("temperatures"));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();
app.MapGet("/",  () => {return TypedResults.Ok("API runs");});

app.MapGet("/rooms", async (IMongoCollection<RoomTemperature> collection)
    => TypedResults.Ok(await collection.Find(Builders<RoomTemperature>.Filter.Empty).ToListAsync()));

app.MapPost("/room", async (IMongoCollection<RoomTemperature> collection, RoomTemperature room)
    =>
{

    await collection.InsertOneAsync(room);
    return TypedResults.Ok(room);
});

app.MapGet("/rooms/{queryString}", async (IMongoCollection<RoomTemperature> collection, string queryString)
    =>
{
    var filter = Builders<RoomTemperature>.Filter.Eq(r => r.Roomname, queryString);
    var rooms = await collection.Find(filter).FirstOrDefaultAsync();

    if (rooms == null)
    {
        return Results.NotFound($"No room found with name {queryString}");
    }

    return Results.Ok(rooms);
});

app.Run();
