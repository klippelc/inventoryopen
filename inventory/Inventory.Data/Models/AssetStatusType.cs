
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Data.Models
{
    public class AssetStatusType
    {
        public int Id { get; set; }

        public int? AssetStatusId { get; set; }

        [ForeignKey("AssetStatusId")]
        public AssetStatus AssetStatus { get; set; }

        public int? AssetTypeId { get; set; }

        [ForeignKey("AssetTypeId")]
        public AssetType AssetType { get; set; }

        public int? LicenseTypeId { get; set; }

        [ForeignKey("LicenseTypeId")]
        public LicenseType LicenseType { get; set; }
    }
}
