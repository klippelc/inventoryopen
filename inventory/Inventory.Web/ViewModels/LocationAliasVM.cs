
namespace Inventory.Web.ViewModels
{
    public class LocationAliasVM
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int? LocationId { get; set; }

        public LocationVM Location { get; set; }
    }
}