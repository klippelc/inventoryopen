namespace Inventory.Web.ViewModels
{
    public class AssetStatusVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }
        public int? Sequence { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public string IconCss { get; set; }
        public string ColorCss { get; set; }
    }
}