using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using TangentWeb.Filters;
using TangentWeb.Models;

namespace TangentWeb.Controllers
{

    public class TangentsController : ApiController
    {
        private TangentWebContext db = new TangentWebContext();

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

        // PUT api/Tangents/5
        public HttpResponseMessage PutTangentItem(int id, TangentItem tangentitem)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != tangentitem.id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(tangentitem).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST api/Tangents
        public HttpResponseMessage PostTangentItem(TangentItem tangentitem)
        {
            if (ModelState.IsValid)
            {
                db.TangentItems.Add(tangentitem);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, tangentitem);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = tangentitem.id }));
                return response;
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
    }
}