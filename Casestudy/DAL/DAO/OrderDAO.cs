using Casestudy.DAL.DomainClasses;
using Casestudy.Helpers;
using CaseStudyAPI.DAL.DomainClasses;
using Microsoft.EntityFrameworkCore;

namespace Casestudy.DAL.DAO
{
    public class OrderDAO
    {
        private readonly AppDbContext _db;
        public OrderDAO(AppDbContext ctx)
        {
            _db = ctx;
        }
        public async Task<List<OrderDetailsHelper>> GetOrderDetails(int orderId, string email)
        {
            Customer? customer = _db.Customers!.FirstOrDefault(customer => customer.Email == email);

            List<OrderDetailsHelper> allDetails = new();
            // LINQ way of doing INNER JOINS
            var results = from order in _db.Orders
                          join orderItem in _db.OrderLineItems! on order.Id equals orderItem.OrderId
                          join product in _db.Products! on orderItem.ProductId equals product.Id
                          where (order.CustomerId == customer!.Id && order.Id == orderId)
                          select new OrderDetailsHelper
                          {
                              productName = product.ProductName,
                              orderId = orderId,
                              price = product.CostPrice * orderItem.QtyOrdered,
                              qtyS = product.QtyOnHand,
                              qtyO = orderItem.QtyOrdered,
                              qtyB = orderItem.QtyOrdered - product.QtyOnHand,
                              DateCreated = order.OrderDate.ToString("yyyy/MM/dd - hh:mm tt")
                          };
            allDetails = await results.ToListAsync();
            return allDetails;

        }
        public async Task<List<Order>> GetAll(int id)
        {
            return await _db.Orders!.Where(order => order.CustomerId == id).ToListAsync<Order>();
        }
        public async Task<int> AddOrder(int customerid, SelectionHelper[] selections)
        {
            int orderId = -1;

            using (var _trans = await _db.Database.BeginTransactionAsync())
            {
                try
                {
                    Order order = new Order();
                    order.OrderDate = DateTime.Now;
                    order.CustomerId = customerid;
                    //order.OrderAmount
                    foreach (SelectionHelper selection in selections)
                    {
                        order.OrderAmount += selection.Qty * selection.item!.CostPrice;
                    }

                    await _db.Orders!.AddAsync(order);
                    await _db.SaveChangesAsync();
                    
                    //orderLineItem
                    foreach(SelectionHelper selection in selections)
                    {

                        OrderLineItem orderLineItem = new();
                        orderLineItem.OrderId = order.Id;
                        orderLineItem.SellingPrice = selection.Qty * selection.item!.CostPrice;
                        orderLineItem.ProductId = selection.item!.Id;
                        //QtyValues
                        //Enough stock
                        if (selection.Qty <= selection.item!.QtyOnHand)
                        {
                            //update product
                            Product product = new();

                            product.Id = selection.item!.Id;
                            product.BrandId = selection.item!.BrandId;
                            product.Timer = selection.item!.Timer;
                            product.ProductName = selection.item!.ProductName;
                            product.GraphicName = selection.item!.GraphicName;
                            product.CostPrice = selection.item!.CostPrice;
                            product.MSRP = selection.item!.MSRP;
                            product.QtyOnHand = selection.item!.QtyOnHand - selection.Qty;
                            product.QtyOnBackOrder = selection.item!.QtyOnBackOrder;
                            product.Description = selection.item!.Description;

                            _db.Products!.Update(product);
                            await _db.SaveChangesAsync();

                            //update Qty values
                            orderLineItem.QtySold = selection.Qty;
                            orderLineItem.QtyOrdered = selection.Qty;
                            orderLineItem.QtyBackOrdered = 0;
                        }
                        //Not enough stock
                        else
                        {
                            //update product
                            Product product = new();

                            product.Id = selection.item!.Id;
                            product.BrandId = selection.item!.BrandId;
                            product.Timer = selection.item!.Timer;
                            product.ProductName = selection.item!.ProductName;
                            product.GraphicName = selection.item!.GraphicName;
                            product.CostPrice = selection.item!.CostPrice;
                            product.MSRP = selection.item!.MSRP;
                            product.QtyOnHand = 0;
                            product.QtyOnBackOrder = selection.Qty - selection.item!.QtyOnHand;
                            product.Description = selection.item!.Description;

                            _db.Products!.Update(product);
                            await _db.SaveChangesAsync();

                            //update Qty values
                            orderLineItem.QtySold = selection.item!.QtyOnHand;
                            orderLineItem.QtyOrdered = selection.Qty;
                            orderLineItem.QtyBackOrdered = selection.Qty - selection.item!.QtyOnHand;
                        }
                        await _db.OrderLineItems!.AddAsync(orderLineItem);
                        await _db.SaveChangesAsync();
                    }

                    await _trans.CommitAsync();
                    orderId = order.Id;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await _trans.RollbackAsync();
                }
            }

            return orderId;
        }
    }
}
