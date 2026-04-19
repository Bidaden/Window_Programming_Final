using System;
using System.Collections.Generic;
using MySellerApp.DAL;
using MySellerApp.Models;

namespace MySellerApp.BL
{
    public class SaleOrderService
    {
        private readonly SaleOrderRepository _repo
            = new SaleOrderRepository();

        public int PlaceOrder(string customerName,
                              string customerPhone,
                              List<CartItem> cartItems)
        {
            // Validate
            if (string.IsNullOrWhiteSpace(customerName))
                throw new Exception("Vui lòng nhập tên khách hàng!");
            if (string.IsNullOrWhiteSpace(customerPhone))
                throw new Exception("Vui lòng nhập số điện thoại!");
            if (cartItems == null || cartItems.Count == 0)
                throw new Exception("Giỏ hàng đang trống!");

            // Kiểm tra số lượng tồn kho
            foreach (var item in cartItems)
            {
                if (item.Quantity <= 0)
                    throw new Exception(
                        string.Format("{0}: Số lượng không hợp lệ!", item.ProductName));
                if (item.Quantity > item.MaxStock)
                    throw new Exception(
                        string.Format("{0}: Chỉ còn {1} sản phẩm trong kho!",
                        item.ProductName, item.MaxStock));
            }

            // Tính tổng tiền
            decimal total = 0;
            foreach (var item in cartItems)
                total += item.SubTotal;

            var order = new SaleOrder
            {
                CustomerName = customerName,
                CustomerPhone = customerPhone,
                TotalAmount = total
            };

            var details = new List<SaleOrderDetail>();
            foreach (var item in cartItems)
            {
                details.Add(new SaleOrderDetail
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                });
            }

            return _repo.CreateOrder(order, details);
        }

        public List<SaleOrder> GetAllOrders()
        {
            return _repo.GetAll();
        }

        public List<SaleOrderDetail> GetOrderDetails(int orderId)
        {
            return _repo.GetDetailsByOrderId(orderId);
        }
    }
}