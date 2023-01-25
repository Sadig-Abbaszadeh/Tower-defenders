namespace DartsGames.CUT
{
    /// <summary>
    /// Make the property an autoproperty and subscribe to its on start and destroy to get notifs
    /// </summary>
    public interface IPoolComponent
    {
        PoolObject poolObject { get; set; }
    }
}