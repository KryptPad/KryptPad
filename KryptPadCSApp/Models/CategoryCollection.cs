using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KryptPadCSApp.Models
{
    class CategoryCollection : ObservableCollection<Category>
    {
        protected override void InsertItem(int index, Category item)
        {
            base.InsertItem(index, item);
        }
    }
}
