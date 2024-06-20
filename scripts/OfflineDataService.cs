using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using WhackAMath;
using Godot;

public class OfflineDataService : IDataService
{
    private string FilePath = "user://savegame.json";
    private SceneTree Tree;

    public OfflineDataService(SceneTree tree)
    {
        Tree = tree;
    }

    public async Task SaveGameDataAsync(Dictionary<string, object> data)
    {
        string jsonData = JsonSerializer.Serialize(data);
        using StreamWriter file = new StreamWriter(FilePath);
        await file.WriteAsync(jsonData);
    }

    public async Task<Dictionary<string, object>> LoadGameDataAsync()
    {
        Dictionary<string, object> data = null;
        if (File.Exists(FilePath))
        {
            string jsonData = await File.ReadAllTextAsync(FilePath);
            data = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonData);
        }
        return data;
    }

    public void Logout()
    {
        Tree.Quit();
    }
}
