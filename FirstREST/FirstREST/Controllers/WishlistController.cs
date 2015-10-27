using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FirstREST.Lib_Primavera.Model;

namespace FirstREST.Controllers
{
    public class WishlistController : ApiController
    {
        // GET api/Wishlist/ALCAD    
        public Wishlist Get(string id)
        {
            Lib_Primavera.Model.Wishlist wish = Lib_Primavera.PriIntegration.GetWishlistUser(id);
            if (wish == null)
            {
                throw new HttpResponseException(
                        Request.CreateResponse(HttpStatusCode.NotFound));

            }
            else
            {
                return wish;
            }
        }

    }
}
