namespace EchoProject.Domain.Common
{
    public abstract class Entity //TODO ACTIVE/INACTIVE
    {
        public Guid Id { get; protected set; } = Guid.NewGuid();
    }
}