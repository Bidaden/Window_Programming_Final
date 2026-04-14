using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySellerApp.DAL
{
    public static class DBConnection
    {
        public static string ConnStr =
            "Server=localhost;" +
            "Database=ElectronicShopDB;" +
            "Integrated Security=True;";
    }
}
