using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TangentWeb.Storage;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.AspNet.SignalR;


namespace TangentWeb.Controllers
{
    public class StorageController : ApiController
    {
        readonly CloudBlobContainer storageContainer;

        public StorageController()
        {
            storageContainer = GetContainer();
        }

        // GET api/storage
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/storage/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/storage
        public void Post([FromBody]string value)
        {
        }

        // PUT api/storage/5
        public async void Put(int id, [FromBody]string value)
        {
            if (!Request.Content.IsMimeMultipartContent("form-data"))
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.UnsupportedMediaType));
            }

            var provider = new StorageHelper(storageContainer);
            await Request.Content.ReadAsMultipartAsync(provider);

            //var context = GlobalHost.ConnectionManager.GetHubContext<Hubs.TangentHub>();

            //context.Clients.All.newPhotosReceived(provider.Urls);
        }

        // DELETE api/storage/5
        public void Delete(int id)
        {
        }

        CloudBlobContainer GetContainer()
        {
            // Retrieve storage account from connection string.
            var storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("photos");

            // Create the container if it doesn't already exist.
            container.CreateIfNotExists();

            var permissions = container.GetPermissions();
            if (permissions.PublicAccess == BlobContainerPublicAccessType.Off)
            {
                permissions.PublicAccess = BlobContainerPublicAccessType.Blob;
                container.SetPermissions(permissions);
            }

            return container;
        }
    }
}
