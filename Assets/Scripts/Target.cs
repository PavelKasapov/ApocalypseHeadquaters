public class Target
{
    private readonly SightSystem sightSystem;

    public ITargetInfo targetInfo;
    public float Distance { get; private set; }
    public bool IsDirectVision;

    public Target(ITargetInfo targetInfo, SightSystem sightSystem)
    {
        this.targetInfo = targetInfo;
        this.sightSystem = sightSystem;
    }

    public void UpdateDistance()
    {
        Distance = sightSystem.GetDistance(targetInfo.Transform.position);
    }
}
