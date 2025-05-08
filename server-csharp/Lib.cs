using SpacetimeDB;

public static partial class Module
{
    // We're using this table as a singleton, so in this table
    // there can only be one element where the 'id' is 0
    [Table(Name = "config", Public = true)]
    public partial struct Config
    {
        [PrimaryKey]
        public uint id;
        public ulong world_size;
    }

    // This allows us to store 2D points in tables.
    [SpacetimeDB.Type]
    public partial struct DbVector2 
    {
        public float x;
        public float y;

        public DbVector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }

    [Table(Name = "entity", Public = true)]
    public partial struct Entity 
    {
        [PrimaryKey, AutoInc]
        public uint entity_id;
        public DbVector2 position;
        public uint mass;
    }

    [Table(Name = "circle", Public = true)]
    public partial struct Circle
    {
        [PrimaryKey]
        public uint entity_id;
        [SpacetimeDB.Index.BTree]
        public uint player_id;
        public DbVector2 direction;
        public float speed;
        public SpacetimeDB.Timestamp last_split_time;
    }

    [Table(Name = "food", Public = true)]
    public partial struct Food
    {
        [PrimaryKey]
        public uint entity_id;
    }

    [Table(Name = "player", Public = true)]
    public partial struct Player
    {
        [PrimaryKey]
        public Identity identity;
        [Unique, AutoInc]
        public uint player_id;
        public string name;
    }

    [Reducer(ReducerKind.ClientConnected)]
    public static void Connect(ReducerContext ctx)
    {
        Log.Info($"{ctx.Sender} just connected.");
    }

    // Note the `init` parameter passed to the reducer macro.
    // That indicates to SpacetimeDB that it should be called
    // once upon database creation.
    [Reducer(ReducerKind.Init)]
    public static void Init(ReducerContext ctx) {
        Log.Info($"Initializing...");
        ctx.db.config.Insert(new Config { world_size = 1000});
    }


    const uint FOOD_MASS_MIN = 2;
    const uint FOOD_MASS_MAX = 4;
    const uint TARGET_FOOD_COUNT = 600;

    public static float MassToRadius(uint mass) => MathF.Sqrt(mass);

    [Reducer]
    public static void SpawnFood(ReducerContext ctx) {
        if (ctx.Db.player.Count == 0) //Are there no players yet?
        {
            return;
        }

        var world_size = (ctx.Db.config.id.Find(0) ?? throw new Exception("Config not found")).world_size;
        var rng = ctx.Rng;
        var food_count = ctx.Db.food.Count;
        while (food_count < TARGET_FOOD_COUNT)
        {
            var food_mass = rng.Range(FOOD_MASS_MIN, FOOD_MASS_MAX);
            
        }
    }
}
