using Inventory.Data.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Data.Models
{
    public class AssetList : BaseEntity
    {
        public int Id { get; set; }

        [MaxLength(150)]
        public string Name { get; set; }

        [MaxLength(300)]
        public string Description { get; set; }

        public bool? Shared { get; set; }

        public int? AssetTypeId { get; set; }

        [ForeignKey("AssetTypeId")]
        public AssetType AssetType { get; set; }

        public int? UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        public virtual ICollection<AssetUserList> AssetUserLists { get; set; }
    }
}
