using System;
using System.Collections.Generic;

namespace AzureRoadshow.VideoStore.Models.EF
{
    public partial class ProducerToVideo
    {
        public int RelationId { get; set; }
        public int ProducerId { get; set; }
        public int VideoId { get; set; }

        public virtual Producer Producer { get; set; }
        public virtual Video Video { get; set; }
    }
}
