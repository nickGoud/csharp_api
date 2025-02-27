using Casestudy.DAL;
using Casestudy.DAL.DAO;
using Casestudy.DAL.DomainClasses;
using Casestudy.Helpers;
using CaseStudyAPI.DAL.DomainClasses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Casestudy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        readonly AppDbContext _ctx;
        public OrderController(AppDbContext ctx)
        {
            _ctx = ctx;
        }
        [Route("{orderid}/{email}")]
        [HttpGet]
        public async Task<ActionResult<List<OrderDetailsHelper>>> GetTrayDetails(int orderid, string email)
        {
            OrderDAO orderDAO = new(_ctx);
            return await orderDAO.GetOrderDetails(orderid, email);
        }

        [Route("{email}")]
        [HttpGet]
        public async Task<ActionResult<List<Order>>> List(string email)
        {
            List<Order> orders;  ;
            CustomerDAO customerDAO = new(_ctx);
            Customer? customerOrder = await customerDAO.GetByEmail(email);
            OrderDAO orderDAO = new(_ctx);
            orders = await orderDAO.GetAll(customerOrder!.Id);
            return orders;
        }
        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<string>> Index(OrderHelper helper)
        {
            string retVal = "";

            Console.WriteLine(helper);
            try
            {
                CustomerDAO customerDao = new(_ctx);
                Customer? orderCustomer = await customerDao.GetByEmail(helper.Email);
                OrderDAO orderDAO = new(_ctx);
                int orderId = await orderDAO.AddOrder(orderCustomer!.Id, helper.Selections!);

                bool backOrder = false;

                foreach (SelectionHelper selection in helper.Selections!)
                {
                    if (selection.Qty > selection.item!.QtyOnHand) { backOrder = true; }
                }
                
                if (backOrder)
                {
                    retVal = orderId > 0
                        ? "Order " + orderId + " saved! Goods backordered!"
                        : "Order not saved";
                }
                else
                {
                    retVal = orderId > 0
                        ? "Order " + orderId + " saved!"
                        : "Order not saved";
                }
            }
            catch (Exception ex)
            {
                retVal = "Order not saved: " + ex.Message;
            }


            return retVal;
        }
    }
}
