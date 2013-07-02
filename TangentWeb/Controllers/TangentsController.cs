using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.SignalR;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using TangentWeb.Models;

namespace TangentWeb.Controllers
{

    public class TangentsController : ApiController
    {
        readonly TangentWebContext db;

        public TangentsController()
        {
            db = new TangentWebContext();
        }


        // GET api/Tangents
        [AllowAnonymous]
        public IEnumerable<TangentItem> GetTangentItems()
        {
            return db.TangentItems.AsEnumerable();
        }

        // GET api/Tangents/5
        [AllowAnonymous]
        public TangentItem GetTangentItem(int id)
        {
            TangentItem tangentitem = db.TangentItems.Find(id);
            if (tangentitem == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return tangentitem;
        }

        // POST api/tangents
        public async Task<HttpResponseMessage> Post()
        {
            if (ModelState.IsValid && Request.Content.IsMimeMultipartContent())
            {
                string root = HttpContext.Current.Server.MapPath("~/App_Data");
                var provider = new MultipartFormDataStreamProvider(root);

                try
                {
                    await Request.Content.ReadAsMultipartAsync(provider);

                    var text = new StringBuilder();
                    var title = new StringBuilder();

                    foreach (var key in provider.FormData.AllKeys)
                    {
                        foreach (var val in provider.FormData.GetValues(key))
                        {
                            switch (key)
                            {
                                case "Text":
                                    text.Append(val);
                                    break;
                                case "Title":
                                    title.Append(val);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                    var tangent = new TangentItem(title.ToString(), text.ToString(), System.DateTime.UtcNow, User.Identity.Name);
                    db.TangentItems.Add(tangent);
                    db.SaveChanges();

                    // This illustrates how to get the file names for uploaded files.
                    // TODO: perform this in an async / await routine
                    foreach (var file in provider.FileData)
                    {
                        FileInfo fileInfo = new FileInfo(file.LocalFileName);
                       
                        string fileName = Path.GetFileName(file.Headers.ContentDisposition.FileName.Trim('"'));
                        var blob = GetContainer().GetBlockBlobReference(fileName);

                        using (var stream = File.OpenRead(file.LocalFileName))
                        {
                            blob.UploadFromStream(stream);
                        }

                        File.Delete(file.LocalFileName);
                    }

                    // Broadcast on the hub for all listeners
                    var context = GlobalHost.ConnectionManager.GetHubContext<Hubs.TangentHub>();
                    context.Clients.All.newTangentReceived(tangent.id);

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, tangent);
                    response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = tangent.id }));
                    return response;
                }
                catch (System.Exception e)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
                }
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/Tangents/5
        public HttpResponseMessage DeleteTangentItem(int id)
        {
            TangentItem tangentitem = db.TangentItems.Find(id);
            if (tangentitem == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.TangentItems.Remove(tangentitem);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, tangentitem);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
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