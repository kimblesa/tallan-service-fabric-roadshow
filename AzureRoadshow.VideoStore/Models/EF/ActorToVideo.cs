using System;
using System.Collections.Generic;

namespace AzureRoadshow.VideoStore.Models.EF
{
    public partial class ActorToVideo
    {
        public int RelationId { get; set; }
        public int ActorId { get; set; }
        public int VideoId { get; set; }

        public virtual Actor Actor { get; set; }
        public virtual Video Video { get; set; }
    }
}
