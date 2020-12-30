using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcServer
{
    public class CustomersService : Customer.CustomerBase
    {
        private readonly ILogger<CustomersService> _logger;

        public CustomersService(ILogger<CustomersService> logger)
        {
            _logger = logger;
        }

        public override Task<CustomerModel> GetCustomerInfo(CustomerLookupModelRequest request,
            ServerCallContext context)
        {
            CustomerModel customerModel = new CustomerModel();
            if (request.UserID == 1)
            {
                customerModel.FirstName = "Engin";
                customerModel.LastName = "Hazar";
            }
            else if (request.UserID == 2)
            {
                customerModel.FirstName = "Ebru";
                customerModel.LastName = "Hazar";
            }
            else if (request.UserID == 3)
            {
                customerModel.FirstName = "Defne";
                customerModel.LastName = "Hazar";
            }
            else if (request.UserID == 4)
            {
                customerModel.FirstName = "Demir";
                customerModel.LastName = "Hazar";
            }

            return Task.FromResult(customerModel);
        }

        public override async Task GetNewCustomers(NewCustomerRequest request, 
            IServerStreamWriter<CustomerModel> responseStream, 
            ServerCallContext context)
        {

            List<CustomerModel> list = new List<CustomerModel>();
            list.Add(new CustomerModel { FirstName = "engin" });
            list.Add(new CustomerModel { FirstName = "ebru" });
            list.Add(new CustomerModel { FirstName = "defne" });
            list.Add(new CustomerModel { FirstName = "demir" });
            foreach (var cust in list)
            {
                await responseStream.WriteAsync(cust);
                await Task.Delay(10000);
            }
        }
    }
}
