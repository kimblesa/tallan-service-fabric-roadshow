using System;
using System.Collections.Generic;

namespace AzureRoadshow.VideoStore.Models.EF
{
    public partial class TagToVideo
    {
        public int RelationId { get; set; }
        public int TagId { get; set; }
        public int VideoId { get; set; }

        public virtual Tag Tag { get; set; }
        public virtual Video Video { get; set; }
    }
}
