using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace TheFactorM.Federation
{
	public class IdentityProviderTableViewSource: UITableViewSource
	{
		private IEnumerable<IdentityProviderInformation> _items;
		static NSString kCellIdentifier = new NSString ("IdpIdentifier");
		private bool _useImages = false;

		/// <summary>
		/// Fires when the user selects an idp from the list.
		/// </summary>
		public event EventHandler<IdentityProviderSelectedEventArgs> IdentityProviderSelected;

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			// Notify the view controller which idp to navigate to.
			if (IdentityProviderSelected != null)
			{
				IdentityProviderSelected(this, new IdentityProviderSelectedEventArgs() { IdentityProvider = _items.ElementAt(indexPath.Row) });
			}
		}

		public override float GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			return _useImages ? 55 : 44;
		}
		
		public IdentityProviderTableViewSource(IEnumerable<IdentityProviderInformation> items, bool useImages)
		{
			_items = items;
			_useImages = useImages;
		}
		
		public override int RowsInSection (UITableView tableView, int section)
		{
			return _items.Count();
		}
		
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = new UITableViewCell (UITableViewCellStyle.Default, kCellIdentifier);
			cell.BackgroundColor = _useImages ? UIColor.White : UIColor.Clear;
			var idp = _items.ElementAt(indexPath.Row);
			if (!string.IsNullOrEmpty(idp.ImageUrl) && _useImages)
			{
				cell.ImageView.Image = UIImage.LoadFromData(NSData.FromUrl(new NSUrl(idp.ImageUrl)));
			}
			else
			{
				// Windows Live ID replaces (tm) with ???
				cell.TextLabel.Text = _items.ElementAt(indexPath.Row).Name.Replace("???", "");
			}
			return cell;
		}
	}
}

