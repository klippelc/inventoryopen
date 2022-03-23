namespace Inventory.Web.ViewModels
{
    public class LicenseTypeVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? Sequence { get; set; }
        public string Description { get; set; }
        public string IconCss { get; set; }
        public bool Active { get; set; }
    }
}