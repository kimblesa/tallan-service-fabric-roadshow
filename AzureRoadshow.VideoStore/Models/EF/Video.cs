using System;
using System.Collections.Generic;

namespace AzureRoadshow.VideoStore.Models.EF
{
    public partial class Video
    {
        public Video()
        {
            ActorToVideo = new HashSet<ActorToVideo>();
            FormatToVideo = new HashSet<FormatToVideo>();
            ProducerToVideo = new HashSet<ProducerToVideo>();
            TagToVideo = new HashSet<TagToVideo>();
        }

        public int VideoId { get; set; }
        public string Title { get; set; }
        public int? Length { get; set; }
        public string Description { get; set; }

        public virtual ICollection<ActorToVideo> ActorToVideo { get; set; }
        public virtual ICollection<FormatToVideo> FormatToVideo { get; set; }
        public virtual ICollection<ProducerToVideo> ProducerToVideo { get; set; }
        public virtual ICollection<TagToVideo> TagToVideo { get; set; }
    }
}
