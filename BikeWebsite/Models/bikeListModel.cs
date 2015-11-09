using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BikeWebsite.Models
{
    public class bikeListModel : DBEntities
    {
        public bikeListModel(string ProdModel, string Desc, int ProdModelID)
        {
            ProductModel = ProdModel;
            Description = Desc;
            ProductModelID = ProdModelID;
        }

        public string ProductModel { get; set; }
        public string Description { get; set; }
        public int ProductModelID { get; set; }
    }
}