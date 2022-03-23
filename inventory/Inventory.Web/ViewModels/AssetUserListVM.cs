using System;
using System.ComponentModel.DataAnnotations;

namespace Inventory.Web.ViewModels
{
    public class AssetUserListVM
    {
        public int? Id { get; set; }

        public int? AssetId { get; set; }

        public AssetVM Asset { get; set; }

        public int? UserId { get; set; }

        public UserVM User { get; set; }

        public int? AssetListId { get; set; }

        public AssetListVM AssetList { get; set; }

        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
    }
}