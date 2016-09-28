using System;
using System.Collections.Generic;

namespace AzureRoadshow.VideoStore.Models.EF
{
    public partial class VideoFormat
    {
        public VideoFormat()
        {
            FormatToVideo = new HashSet<FormatToVideo>();
        }

        public int FormatId { get; set; }
        public string FormatName { get; set; }

        public virtual ICollection<FormatToVideo> FormatToVideo { get; set; }
    }
}
