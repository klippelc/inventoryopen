namespace Inventory.Web.ViewModels
{
    public class AmenityTypeVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? Sequence { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
    }
}