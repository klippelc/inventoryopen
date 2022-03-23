using System;

namespace Inventory.Web.ViewModels
{
    public class UserAssetConfirmationVM
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        public UserVM User { get; set; }

        public int? AssetId { get; set; }

        public AssetVM Asset { get; set; }

        public DateTime? DateConfirmed { get; set; }

        public string Notes { get; set; }
    }
}