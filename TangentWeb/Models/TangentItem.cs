using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Web;

namespace TangentWeb.Models
{
  public class TangentItem
  {
    public TangentItem()
    {
    }

    public int id { get; set; }
   
    public string ImageUrl { get; set; }

    public string Title { get; set; }

    public string Text { get; set; }

    public string Date { get; set; }

    public bool Complete { get; set; }

    public string PosterId { get; set; }

    public string PosterImageUrl { get; set; }

    public string DeviceToken { get; set; }

    public string OriginalUrl { get; set; }

  }
}