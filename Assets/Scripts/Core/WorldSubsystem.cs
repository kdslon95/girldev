namespace Core
{
    public abstract class WorldSubsystem
    {
        public virtual void InitializeSubsystem()
        {
        }

        public virtual void PreTickSubsystem(float deltaTime)
        {
        }

        public virtual void TickSubsystem(float deltaTime)
        {
        }

        public virtual void LateTickSubsystem(float deltaTime)
        {
        }

        public virtual void DisposeSubsystem()
        {
        }
    }
}