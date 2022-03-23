using Inventory.Data.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Data.Models
{
    public class UserAssetConfirmation : BaseEntity
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        public int? AssetId { get; set; }

        [ForeignKey("AssetId")]
        public Asset Asset { get; set; }

        public DateTime? DateConfirmed { get; set; }

        [MaxLength(500)]
        public string Notes { get; set; }
    }
}
