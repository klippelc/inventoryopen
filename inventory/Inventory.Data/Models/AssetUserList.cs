using Inventory.Data.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Data.Models
{
    public class AssetUserList : BaseEntity
    {
        public int Id { get; set; }

        public int? AssetListId { get; set; }

        [ForeignKey("AssetListId")]
        public AssetList AssetList { get; set; }

        public int? AssetId { get; set; }

        [ForeignKey("AssetId")]
        public Asset Asset { get; set; }

        public int? UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

    }
}
