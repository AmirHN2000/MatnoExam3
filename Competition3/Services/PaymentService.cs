using System.Linq;
using System.Threading.Tasks;
using Competition3.Data;
using Microsoft.EntityFrameworkCore;

namespace Competition3.Services
{
    public interface IPaymentService
    {
        public Task AddPaymentAsync(Payment payment);

        public Task<Payment> GetPaymentAsync(string authority);

        public IQueryable<Payment> GetAll();

        public Task<Payment> FindByIdAsync(int payId);
    }

    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _dbContext;

        public PaymentService(AppDbContext context)
        {
            _dbContext = context;
        }

        public async Task AddPaymentAsync(Payment payment)
        {
            await _dbContext.Payments.AddAsync(payment);
        }

        public async Task<Payment> GetPaymentAsync(string authority)
        {
            return await _dbContext.Payments.FirstOrDefaultAsync(x => x.SystemCode == authority);
        }

        public IQueryable<Payment> GetAll()
        {
            return _dbContext.Payments.AsQueryable();
        }

        public async Task<Payment> FindByIdAsync(int payId)
        {
            return await _dbContext.Payments.FirstOrDefaultAsync(x => x.Id == payId);
        }
    }
}