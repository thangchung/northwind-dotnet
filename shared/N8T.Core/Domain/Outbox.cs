using System;

namespace N8T.Core.Domain
{
    public abstract class Outbox : EntityRootBase
    {
        protected Outbox(Guid id, string type, string aggregateType, Guid aggregateId, byte[] payload)
        {
            Id = id;
            Type = type;
            AggregateType = aggregateType;
            AggregateId = aggregateId;
            Payload = payload;
        }

        public new Guid Id { get; private set; }
        public string Type { get; private set; }
        public string AggregateType { get; private set; }
        public Guid AggregateId { get; private set; }
        public byte[] Payload { get; private set; }
    }
}
