public class Layer
{
    public const string UI = "UI";
    public const string POST_PROCESSING = "PostProcessing";
    public const string FRIENDLY = "Friendly";
    public const string ENEMY = "Enemy";
    public const string STRUCTURES = "Structures";
    public const string TERRAIN = "Terrain";
    public const string DEFAULT = "Unknown";

    public string Name;
    public int Priority;

    public static Layer[] GetLayers()
    {
        var ui = new Layer() { Name = UI, Priority = 5 };
        var postProcess = new Layer() { Name = POST_PROCESSING, Priority = 8 };
        var friendly = new Layer() { Name = FRIENDLY, Priority = 9 };
        var enemy = new Layer() { Name = ENEMY, Priority = 10 }; 
        var structures = new Layer() { Name = STRUCTURES, Priority = 11 };
        var terrain = new Layer() { Name = TERRAIN, Priority = 12 };
        var unknown = new Layer() { Name = DEFAULT, Priority = 0 };

        return new Layer[] { unknown, ui, postProcess, friendly, enemy, structures, terrain };
    }
}

