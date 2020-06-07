// Component
public class EntryPoint : Component
{

    public EntryPoint(Vector3 location, float angle, float size, int world, int interior)
    {
        Position = location;
        Angle = angle;
        Interior = interior;
        VirtualWorld = world;
    }  

    public Vector3 Position { get; }

    public int Interior { get; }

    public int VirtualWorld { get; }
    public float Angle { get; }

    public EntryPoint Entry { get; set; }
}

// Service
public class ServiceEntry : IServiceEntry
{
    [EntityType]
    private static readonly Guid AreaLinkType = new Guid("04B52E0D-B8CF-18C6-9D21-C8B3AF646720");

    private static EntityId GetAreaLinkId(int areaId)
    {
        return new EntityId(AreaLinkType, areaId);
    }

    public ServiceEntry(IEntityManager entityManager, IStreamerService streamerService)
    {
        _entityManager = entityManager;
		_streamerService = streamerService;
    }
    
    public EntryPoint CreateEntryPoint(Vector3 location, float angle, float size, int world, int interior)
    {
		var entity = GetAreaLinkId(_entityManager.GetComponents<EntryPoint>().Length);
		_entityManager.Create(entity);
			
		var id = _entityManager.AddComponent<EntryPoint>(entity, location, angle, size, world, interior);
			
		_streamerService.CreateSphere(location, size, world, interior, parent:entity);
		return id;
    }
}

// System
public class EntrySystem : ISystem
{
    [Event]
    private void OnPlayerEnterDynamicArea(Player player, DynamicArea area)
    {
        var entry = area.GetComponentInParent<EntryPoint>();

        player.Position = entry.Entry.Position;
        player.Interior = entry.Entry.Interior;
        player.VirtualWorld = entry.Entry.VirtualWorld;
    }
}
