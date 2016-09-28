using System;
using System.Collections.Generic;

namespace AzureRoadshow.VideoStore.Models.EF
{
    public partial class Actor
    {
        public Actor()
        {
            ActorToVideo = new HashSet<ActorToVideo>();
        }

        public int ActorId { get; set; }
        public string ActorName { get; set; }

        public virtual ICollection<ActorToVideo> ActorToVideo { get; set; }
    }
}
