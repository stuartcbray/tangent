using System;
using System.Runtime.Serialization;
using System.Globalization;

namespace Tangent
{
	public class TangentItem
	{
		public enum TangentVisibility { Private, Public, Friends, GeoLocation };

		public enum TangentStatus { Draft, Complete };

		public TangentItem() { }

		public TangentItem(string title, string text, DateTime date, string posterId)
		{
			Title = title;
			Text = text;
			Date = date;
			PosterId = posterId;
		}

		public int id { get; set; }

		public string ImageUrl { get; set; }

		public string Title { get; set; }

		public string Text { get; set; }

		public DateTime Date { get; set; }

		public TangentStatus Status { get; set; }

		public TangentVisibility Visibility { get; set; }

		public string PosterId { get; set; }

		public string PosterImageUrl { get; set; }

		public string DeviceToken { get; set; }

		public string OriginalUrl { get; set; }

	}

}

