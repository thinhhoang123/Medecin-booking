namespace Doctor.Domain.Events
{
    public interface IDomainEvent
    {
        DateTime OccurredOn { get; }
        string EventType { get; }
        string AggregateId { get; }
    }

    public abstract class DomainEvent : IDomainEvent
    {
        public DateTime OccurredOn { get; protected set; }
        public string EventType { get; protected set; }
        public string AggregateId { get; protected set; }

        protected DomainEvent(string aggregateId)
        {
            AggregateId = aggregateId;
            OccurredOn = DateTime.UtcNow;
            EventType = GetType().Name;
        }

        protected DomainEvent(string aggregateId, DateTime occurredOn)
        {
            AggregateId = aggregateId;
            OccurredOn = occurredOn;
            EventType = GetType().Name;
        }
    }
}