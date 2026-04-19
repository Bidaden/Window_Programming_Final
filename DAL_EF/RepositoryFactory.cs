using MySellerApp.BL.Interfaces;
using MySellerApp.DAL;

namespace MySellerApp.DAL_EF
{
    public static class RepositoryFactory
    {
        public static IUserRepository CreateUserRepository()
        {
            return AppConfig.UseADO
                ? (IUserRepository)new UserRepository()
                : (IUserRepository)new UserRepositoryEF();
        }

        public static IProductRepository CreateProductRepository()
        {
            return AppConfig.UseADO
                ? (IProductRepository)new ProductRepository()
                : (IProductRepository)new ProductRepositoryEF();
        }
    }
}