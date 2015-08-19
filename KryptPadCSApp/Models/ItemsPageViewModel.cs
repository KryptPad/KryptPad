using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KryptPadCSApp.Models
{
    class ItemsPageViewModel : BaseModel
    {
        #region Properties
        /// <summary>
        /// Gets the collection of categories
        /// </summary>
        //public CategoryCollection Categories
        //{
        //    get { App.Current.Document.Categories; }
        //}
        public CategoryCollection Categories { get; private set; } = new CategoryCollection();
        #endregion


        public ItemsPageViewModel()
        {
            //add some fake categories
            Categories.Add(new Category() { Name = "Category 1" });
            Categories.Add(new Category() { Name = "Category 2" });
            Categories.Add(new Category() { Name = "Category 3" });
            Categories.Add(new Category() { Name = "Category 4" });

            Categories[0].Items.Add(new Profile() { Name = "Profile 1" });
            Categories[0].Items.Add(new Note() { Name = "Note 1" });
            Categories[0].Items.Add(new Profile() { Name = "Profile 2" });

            Categories[1].Items.Add(new Note() { Name = "Note 2" });
            Categories[1].Items.Add(new Note() { Name = "Profile 3" });
            Categories[1].Items.Add(new Profile() { Name = "Profile 3" });
        }

    }
}
