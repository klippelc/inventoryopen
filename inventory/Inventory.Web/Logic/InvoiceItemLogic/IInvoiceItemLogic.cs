using Inventory.Web.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory.Web.Logic
{
    public interface IInvoiceItemLogic
    {
        Task<IEnumerable<PagedListVM>> GetInvoiceItems(int? invoiceId, int? invoiceItemId, string assetType = "", string assetCategory = "", string sortOrder = "", string searchString = "", int page = 1, int pageSize = 10000000);

        Task<InvoiceItemVM> GetBlankInvoiceItem();

        Task<InvoiceItemVM> CreateInvoiceItem(InvoiceItemVM vm);

        Task<InvoiceItemVM> GetInvoiceItem(int? invoiceItemId);

        Task SaveInvoiceItem(InvoiceItemVM vm);

        Task DeleteInvoiceItem(InvoiceItemVM vm);

        Task<int> GetMinInvoiceItemNumber(int invoiceId);

        Task<bool> CheckManuandProduct(int invoiceId, int manuId, int productId);
    }
}