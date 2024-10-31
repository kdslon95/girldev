namespace Core
{
    public abstract class WorldSubsystem
    {
        public abstract void InitializeSubsystem();

        public virtual void TickSubsystem(float deltaTime)
        {
        }

        public abstract void DisposeSubsystem();
    }
}