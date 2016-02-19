using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KryptPadCSApp.Collections
{
    class RefreshableCollection<T> : ObservableCollection<T>
    {
        public void RefreshItem(T item)
        {
            // Get current item index
            var index = IndexOf(item);

            // Remove item
            Remove(item);

            // Add item
            Insert(index, item);
        }
    }
}
