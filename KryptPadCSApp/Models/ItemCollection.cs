using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;


namespace KryptPadCSApp.Models
{
    class ItemCollection : ObservableCollection<ItemBase>
    {
        protected override void InsertItem(int index, ItemBase item)
        {
            //before we insert the add item, make sure it does not exist in the list already.
            //check collection for an existing AddItem
            if (item is AddItem
                && this.Any((i) => i is AddItem))
            {
                return;
            }

            //base method
            base.InsertItem(index, item);
        }
    }
}
