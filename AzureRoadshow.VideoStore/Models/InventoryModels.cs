using AzureRoadshow.VideoStore.Models.EF;
using System.Collections.Generic;

namespace AzureRoadshow.VideoStore.Models
{
    public class VideoWrapperModel
    {
        public Video video { get; set; }
        public string image { get; set; }
        public int inventoryId { get; set; }
        public decimal price { get; set; }
        public List<VideoReview> Reviews { get; set; }
        public VideoReview userReview { get; set; }
    }

    public class VideoListModel
    {
        public IEnumerable<VideoWrapperModel> specials { get; set; }
        public IEnumerable<VideoWrapperModel> recommendations { get; set; }
        public string selectedFormat { get; set; }
    }
}