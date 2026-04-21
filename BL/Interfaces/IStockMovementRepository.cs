using System.Collections.Generic;
using MySellerApp.Models;

namespace MySellerApp.BL.Interfaces
{
    public interface IStockMovementRepository
    {
        List<StockMovement> GetAll();
    }
}