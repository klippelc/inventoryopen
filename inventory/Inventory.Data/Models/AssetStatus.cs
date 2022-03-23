using Inventory.Data.Common;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Inventory.Data.Models
{
    public class AssetStatus : BaseEntity
    {
        public int Id { get; set; }

        [MaxLength(150)]
        public string Name { get; set; }

        [MaxLength(150)]
        public string Group { get; set; }

        public int? Sequence { get; set; }

        [MaxLength(300)]
        public string Description { get; set; }

        [DefaultValue("true")]
        public bool Active { get; set; }

        [MaxLength(50)]
        public string IconCss { get; set; }

        [MaxLength(50)]
        public string ColorCss { get; set; }

        public virtual ICollection<Asset> Assets { get; set; }
        public virtual ICollection<AssetStatusType> AssetStatusTypes { get; set; }
    }
}