using Microsoft.EntityFrameworkCore;
using Repository.Models;

namespace Repository.Impl
{
    public class CustomerRepository : RepositoryBase<Customer>, ICustomerRepository
    {
        private readonly HotelManagementContext context;
        private readonly DbSet<Customer> _customers;
        public CustomerRepository(HotelManagementContext context) : base(context)
        {
            this.context = context;
            _customers = context.Customers;
        }

        public Customer Create(Customer customer)
        {
            customer.Id = GetLatestId() + 1;
            Add(customer);
            return customer;
        }

        public void Delete(int id)
        {
            Customer? customer = GetByCustomerId(id);
            if (customer == null)
            {
                return;
            }
            customer.Status = true;
            Commit();
        }

        public Customer? GetByCustomerId(int id)
        {
            return _customers.SingleOrDefault(c => c.Id == id);
        }

        List<Customer> ICustomerRepository.GetAll()
        {
            return this.GetAll().ToList();
        }

        Customer ICustomerRepository.Update(Customer customer)
        {
            // detach old entity
            var existedCus = _customers.Local.FirstOrDefault(e => e.Id == customer.Id);
            if (existedCus != null)
            {
                context.Entry(existedCus).State = EntityState.Detached;
            }
            // attach new entity
            context.Attach(customer);
            context.Entry(customer).State = EntityState.Modified;
            context.SaveChanges();
            return customer;
        }

        private int GetLatestId()
        {
            return GetAll()
                .OrderByDescending(p => p.Id).FirstOrDefault()
                ?.Id ?? 0;
        }

        public Customer? Login(string email, string password)
        {
            return _customers.FirstOrDefault(c => c.Email == email && c.Password == password);
        }
    }
}
