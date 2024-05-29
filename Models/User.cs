using Microsoft.AspNetCore.Identity;

namespace LabelSite.Models
{
    public class User : IdentityUser
    {
        public byte[] UserPhoto { get; set; }

        public ICollection<SalesData> SalesDatas { get; set; }
    }
}
