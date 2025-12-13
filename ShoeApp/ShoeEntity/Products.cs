namespace ShoeApp
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Products
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Products()
        {
            OrderItems = new HashSet<OrderItems>();
        }

        public int Id { get; set; }

        [StringLength(50)]
        public string Article { get; set; }

        [StringLength(200)]
        public string Name { get; set; }

        public int? UnitId { get; set; }

        public decimal? Price { get; set; }

        public int? SupplierId { get; set; }

        public int? ManufacturerId { get; set; }

        public int? CategoryId { get; set; }

        public int? PromotionId { get; set; }

        public int? StockQty { get; set; }

        public string Description { get; set; }

        public int Discount { get; set; }

        [StringLength(100)]
        public string Photo { get; set; }

        public virtual Categories Categories { get; set; }

        public virtual Manufacturers Manufacturers { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderItems> OrderItems { get; set; }

        public virtual Suppliers Suppliers { get; set; }
    }
}
