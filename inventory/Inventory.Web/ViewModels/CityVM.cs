using System.Collections.Generic;


namespace Inventory.Web.ViewModels
{
    public class CityVM
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int? StateId { get; set; }

        public StateVM State { get; set; }

        public bool Active { get; set; }

        public IEnumerable<StateVM> States { get; set; }
    }
}