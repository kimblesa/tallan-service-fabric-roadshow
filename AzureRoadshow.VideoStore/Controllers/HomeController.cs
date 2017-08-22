using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AzureRoadshow.VideoStore.Models.EF;
using AzureRoadshow.VideoStore.Models;

namespace AzureRoadshow.VideoStore.Controllers
{
    public class HomeController : Controller
    {
        VideoStoreContext _db;
        public HomeController(VideoStoreContext context)
        {
            _db = context;
        }

        /// <summary>
        /// Main index, displays all kinds of video agnostic of format
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            // redirect to pull video list with default action (select all videos)
            return RedirectToAction("VideoList", new { format = "All" });
        }

        /// <summary>
        /// return a list of videos to the view, depending on the selected format and sorting characteristics, typically coming from an action link
        /// in the navbar. 
        /// Videos should be selected in two lists: special videos and recommendations.
        /// For now, these lists are: all videos matching the format, and star wars videos (obviously).
        /// 
        /// TODO push filtering, sorting functionality out of method?
        /// </summary>
        /// <param name="format">The format of the videolist items, i.e. all, DVD, etc.</param>
        /// <param name="sortBy">The attribute to order the list by</param>
        /// <returns></returns>
        public ActionResult VideoList(string format, string sortBy, string filterText = "")
        {
            VideoListModel model = new VideoListModel();
            List<VideoWrapperModel> specials = new List<VideoWrapperModel>();
            List<VideoWrapperModel> recommendations = new List<VideoWrapperModel>();
            VideoWrapperModel item;

            var formatResult = _db.FormatToVideo.AsQueryable();

            if (format != "All")
            {
                formatResult = formatResult.Where(x => x.Format.FormatName == format);
            }

            try
            {
                switch (sortBy)
                {
                    case "Title":
                        formatResult = formatResult.OrderBy(m => m.Video.Title);
                        break;
                    case "Length":
                        formatResult = formatResult.OrderBy(m => m.Video.Length);
                        break;
                    case "Actor":
                        formatResult = formatResult.OrderBy(m => m.Video.ActorToVideo.First().Actor.ActorName);
                        break;
                    case "Producer":
                        formatResult = formatResult.OrderBy(m => m.Video.ProducerToVideo.First().Producer.ProducerName);
                        break;
                    case "Description":
                        formatResult = formatResult.OrderBy(m => m.Video.Description);
                        break;
                    default:
                        break;
                }
            }
            catch (NullReferenceException)
            {
                //TODO do something 
            }

            if (!string.IsNullOrWhiteSpace(filterText))
            {
                formatResult = formatResult.Where(m => m.Video.Title.Contains(filterText));
            }

            // from each video, pull out the video, price, inventory id, and image address
            foreach (var i in formatResult
                .Include(x => x.Inventory)
                .Include(x => x.Video)
                .Take(16))
            {
                item = new VideoWrapperModel();
                item.image = i.Inventory.Image;
                item.price = i.Inventory.Price;
                item.video = i.Video;
                item.inventoryId = i.InventoryId;
                specials.Add(item);
            }
            model.specials = specials;

            // re-query result choosing only recommendations
            // currently, recommendations are always movies containing "Star" in name.
            // TODO allow for new types of recommendations
            model.recommendations = model.specials.Where(m => m.video.Title.Contains("Star")).ToList();
            model.selectedFormat = format;

            return View(model);
        }

        /// <summary>
        /// Display the page for information on a single video, regardless of format
        /// </summary>
        /// <param name="inventoryId">the inventoryId of the video to display</param>
        /// <returns></returns>
        public ActionResult SingleVideo(int inventoryId)
        {
            //TODO make this page* not*format - agnostic, so items can also be populated to list here
            VideoWrapperModel item = new VideoWrapperModel();

            Inventory inventoryItem = _db.Inventory
                .Include(inv => inv.VideoReview)
                    .ThenInclude(review => review.Customer)
                .Include(inv => inv.FormatToVideo)
                    .ThenInclude(ftov => ftov.Video)
                        .ThenInclude(video => video.ProducerToVideo)
                            .ThenInclude(ptov => ptov.Producer)
                .Include(inv => inv.FormatToVideo)
                    .ThenInclude(ftov => ftov.Video)
                        .ThenInclude(video => video.ActorToVideo)
                            .ThenInclude(atov => atov.Actor)
                .Include(inv => inv.FormatToVideo)
                    .ThenInclude(ftov => ftov.Video)
                        .ThenInclude(video => video.TagToVideo)
                            .ThenInclude(ttov => ttov.Tag)
                .Include(inv => inv.FormatToVideo)
                    .ThenInclude(ftov => ftov.Format)
                .First(x => x.InventoryId == inventoryId);

            item.video = inventoryItem.FormatToVideo.FirstOrDefault(m => m.InventoryId == inventoryId).Video;
            item.image = inventoryItem.Image;
            item.inventoryId = inventoryId;
            item.price = inventoryItem.Price;

            item.Reviews = _db.VideoReview.Where(m => m.InventoryId == inventoryId).ToList();

            var customer = _db.Customer.FirstOrDefault(m => m.Email == User.Identity.Name);
            
            if (customer != null)
            {
                // select the review made by the current user, if any
                VideoReview userReview = item.Reviews.FirstOrDefault(m => m.CustomerId == customer.CustomerId);
                // if not, then create a new one to modify
                if (userReview != null)
                {
                    item.userReview = userReview;
                }
                else
                {
                    item.userReview = new VideoReview();
                    item.userReview.CustomerId = customer.CustomerId;
                }
            }
            else
            {
                item.userReview = new VideoReview();
            }

            return View(item);
        }

        /// <summary>
        /// submit a user review to the database, or update an existing one
        /// </summary>
        /// <param name="model">The review to submit</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitReview(VideoReview model)
        {
            // either update or add new review entry to model, depending on whether it exists already.
            // a user can only have one review for a video.
            if (model.ReviewId == 0)
            {
                _db.VideoReview.Add(model);
            }
            else
            {
                _db.VideoReview.Attach(model);
                var entry = _db.Entry(model);
                entry.Property(i => i.Review).IsModified = true;
                entry.Property(i => i.Rating).IsModified = true;
            }

            _db.SaveChanges();
            return RedirectToAction("SingleVideo", "Home", new { inventoryId = model.InventoryId });
        }
    }
}