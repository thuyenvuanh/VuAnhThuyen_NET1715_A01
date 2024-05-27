using Repository.Models;

namespace Repository
{
    public interface ICustomerRepository
    {
        public Customer? Login(string email, string password);
        public Customer? GetByCustomerId(int id);
        public List<Customer> GetAll();
        public Customer Create(Customer customer);
        public Customer Update(Customer customer);
        public void Delete(int id);
    }
}
