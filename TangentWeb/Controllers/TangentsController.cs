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

        bool StoreTangent(MultipartFormDataStreamProvider provider, TangentItem tangent)
        {
            // If there's a photo, then upload it to Azure storage and set the photo url in the tangent
            if (provider.FileData.Count == 1)
            {
                var file = provider.FileData[0];

                try
                {
                    var fileName = Path.GetFileName(file.Headers.ContentDisposition.FileName.Trim('"'));
                    var fileNoExt = Path.GetFileNameWithoutExtension(fileName);
                    fileName = fileNoExt + "_" + DateTime.UtcNow.Ticks.ToString() + Path.GetExtension(fileName);

                    var blob = GetContainer().GetBlockBlobReference(fileName);

                    using (var stream = File.OpenRead(file.LocalFileName))
                    {
                        blob.UploadFromStream(stream);
                    }

                    File.Delete(file.LocalFileName);

                    tangent.ImageUrl = blob.Uri.AbsoluteUri;
                }
                catch (Exception e)
                {
                    // Log me
                }
            }
            
            if (provider.FileData.Count == 1 || provider.FileData.Count == 0)
            {
                db.TangentItems.Add(tangent);
                db.SaveChanges();
                return true;
            }

            return false;
        }

        // POST api/tangents
        public async Task<HttpResponseMessage> Post()
        {
            if (ModelState.IsValid && Request.Content.IsMimeMultipartContent())
            {
                var provider = new MultipartFormDataStreamProvider(Path.GetTempPath());

                try
                {
                    await Request.Content.ReadAsMultipartAsync(provider);

                    var title = provider.FormData["Title"];
                    var text = provider.FormData["Text"];

                    if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(text))
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                    }

                    var tangent = new TangentItem(title, text, System.DateTime.UtcNow, User.Identity.Name);

                    var storageTask = Task<bool>.Factory.StartNew(() => StoreTangent(provider, tangent));

                    await storageTask;

                    if (storageTask.Result == false)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ModelState);
                    }
                   
                    // Broadcast on the hub for all listeners
                    var context = GlobalHost.ConnectionManager.GetHubContext<Hubs.TangentHub>();
                    context.Clients.All.newTangentReceived(tangent);

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

        void DeleteTangentPhoto(string blobName)
        {
            var blob = GetContainer().GetBlockBlobReference(blobName);
            blob.Delete();   
        }

        // DELETE api/Tangents/5
        public async Task<HttpResponseMessage> DeleteTangentItem(int id)
        {
            TangentItem tangent = db.TangentItems.Find(id);
            if (tangent == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            if (tangent.PosterId != User.Identity.Name)
            {
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, new HttpException("You can only delete your own Tangents."));
            }

            db.TangentItems.Remove(tangent);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            if (!string.IsNullOrEmpty(tangent.ImageUrl))
            {
                int i = tangent.ImageUrl.LastIndexOf('/');
                if (i > 0 && i < tangent.ImageUrl.Length - 1)
                {
                    var blobName = tangent.ImageUrl.Substring(i + 1);
                    var deleteTask = Task.Factory.StartNew(() => DeleteTangentPhoto(blobName));
                    await deleteTask;
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, tangent);
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