using System;
using System.Collections.Generic;

namespace AzureRoadshow.VideoStore.Models.EF
{
    public partial class Producer
    {
        public Producer()
        {
            ProducerToVideo = new HashSet<ProducerToVideo>();
        }

        public int ProducerId { get; set; }
        public string ProducerName { get; set; }

        public virtual ICollection<ProducerToVideo> ProducerToVideo { get; set; }
    }
}
