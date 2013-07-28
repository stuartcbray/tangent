
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using TheFactorM.Federation;
using Android.Widget;
using System.Linq;

namespace TheFactorM.Federation.Android.Adapters
{
    /// <summary>
    /// List adapter used to list identity providers.
    /// </summary>
    public class IdentityProviderListAdapter : BaseAdapter<IdentityProviderInformation>
    {
		private Context _context;
		private IEnumerable<IdentityProviderInformation> _items;

        /// <summary>
        /// Constructor taking the context and the items to list.
        /// </summary>
        /// <param name="context">Context in which the adapter is being used.</param>
        /// <param name="items">Items that are to be listed.</param>
        public IdentityProviderListAdapter(Context context,IEnumerable<IdentityProviderInformation> items)
            :base()
        {
			_items = items;
			_context = context;
        }

		#region implemented abstract members of BaseAdapter

		public override long GetItemId (int position)
		{
			return position;
		}

		public override int Count {
			get {
				return _items.Count();
			}
		}

		#endregion

		#region implemented abstract members of BaseAdapter

		public override IdentityProviderInformation this [int position] {
			get {
				return _items.ElementAt (position);
			}
		}

		#endregion

        /// <summary>
        /// Gets the view for an item in the data source.
        /// </summary>
        /// <param name="position">Position of the item to render.</param>
        /// <param name="convertView">Item view to convert.</param>
        /// <param name="parent">Parent of the item.</param>
        /// <returns>Returns the rendered view for the item.</returns>
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View itemView = convertView;

            if (itemView == null)
            {
				itemView = new TextView(_context);
            }
            
            var identityProvider = _items.ElementAt(position);
            ((TextView)itemView).Text = identityProvider.Name;

            return itemView;
        }
    }
}