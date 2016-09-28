using AzureRoadshow.VideoStore.Models.EF;

namespace AzureRoadshow.VideoStore.Services
{
    public class CustomerService
    {
        private VideoStoreContext _db;
        public CustomerService()
        {
            _db = new VideoStoreContext();
        }

        public void AddCustomer(string id, string email)
        {
            _db.Customer.Add(new Customer
            {
                UserId = id,
                Email = email
            });

            _db.SaveChanges();
        }
    }
}
