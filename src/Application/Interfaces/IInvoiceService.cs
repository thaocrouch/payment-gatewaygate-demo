namespace Application.Interfaces;

public interface IInvoiceService
{
    Task<bool> PrintInvoiceAsync(Order order);
}