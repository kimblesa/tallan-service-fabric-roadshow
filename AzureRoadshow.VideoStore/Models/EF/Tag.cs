using System;
using System.Collections.Generic;

namespace AzureRoadshow.VideoStore.Models.EF
{
    public partial class Tag
    {
        public Tag()
        {
            TagToVideo = new HashSet<TagToVideo>();
        }

        public int TagId { get; set; }
        public string TagDescription { get; set; }

        public virtual ICollection<TagToVideo> TagToVideo { get; set; }
    }
}
