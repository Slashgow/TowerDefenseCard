public interface IOccluder
{
    float OcclusionRange { get; }
    float OcclusionFieldOfView { get; }
    bool CanOcclude { get; }

    void SetCanOcclude(bool enabled);
}
