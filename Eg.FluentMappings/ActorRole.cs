namespace Eg.FluentMappings
{
    public class ActorRole : Entity
    {
        public virtual string Actor { get; set; }
        public virtual string Role { get; set; }
        public virtual int Version { get; set; }
    }
}
