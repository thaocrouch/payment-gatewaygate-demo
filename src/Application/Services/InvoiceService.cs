using Application.Interfaces;

namespace Application.Services;

public class InvoiceService : IInvoiceService
{
    public Task<bool> PrintInvoiceAsync(Order order)
    {
        // Handler here
        return Task.FromResult(true);
    }
}