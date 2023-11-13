using System.Text.Json;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.Services;

namespace SearchService.Data;
public class DbInitializer
{
    public static async Task InitDb(WebApplication app)
    {
        await DB.InitAsync("Search", MongoClientSettings.FromConnectionString(app.Configuration.GetConnectionString("MongoDbConnection")));

        await DB.Index<Item>()
            .Key(x => x.Make, KeyType.Text)
            .Key(x => x.Model, KeyType.Text)
            .Key(x => x.Color, KeyType.Text)
            .Key(x => x.Status, KeyType.Text)
            .Key(x => x.Year, KeyType.Ascending)
            .Key(x => x.Mileage, KeyType.Ascending)
            .CreateAsync();

        var count = await DB.CountAsync<Item>();

        using var scope = app.Services.CreateScope();
        var httpClient = scope.ServiceProvider.GetRequiredService<AuctionSvcHttpClient>();
        var items = await httpClient.GetItemsForSearchDb();

        if (items.Count > 0)
        {
            await DB.SaveAsync(items);
        }

    }
}
