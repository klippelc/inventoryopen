namespace Inventory.Web.ViewModels
{
    public class RoleVM
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int Sequence { get; set; }

        public bool Active { get; set; }

        //
        public bool IsChecked { get; set; }

        public string Checked { get; set; }
        //
    }
}