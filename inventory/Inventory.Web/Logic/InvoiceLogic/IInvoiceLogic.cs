using Inventory.Web.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory.Web.Logic
{
    public interface IInvoiceLogic
    {
        Task<IEnumerable<InvoiceVM>> GetInvoiceList(string sortOrder = "", string currentFilter = "", string searchString = "", int page = 1, int pageSize = 10000000);

        Task<IEnumerable<InvoiceVM>> GetInvoices();

        Task<InvoiceVM> GetBlankInvoice();

        Task<InvoiceVM> CreateInvoice(InvoiceVM vm);

        Task<InvoiceVM> GetInvoice(int invoiceId);

        Task<InvoiceVM> SaveInvoice(InvoiceVM vm);

        Task DeleteInvoice(InvoiceVM vm);

        Task<bool> CheckPONumber(string poNumber);
    }
}