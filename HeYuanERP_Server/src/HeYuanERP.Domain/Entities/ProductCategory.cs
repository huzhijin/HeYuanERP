using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeYuanERP.Domain.Entities;

[Table("ProductCategories")]
public class ProductCategory
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string CategoryName { get; set; } = string.Empty;

    [StringLength(50)]
    public string CategoryCode { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    public int? ParentCategoryId { get; set; }

    [ForeignKey("ParentCategoryId")]
    public virtual ProductCategory? ParentCategory { get; set; }

    public virtual ICollection<ProductCategory> SubCategories { get; set; } = new List<ProductCategory>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    [StringLength(50)]
    public string Status { get; set; } = "Active"; // Active, Inactive

    public int SortOrder { get; set; }

    [StringLength(200)]
    public string ImageUrl { get; set; } = string.Empty;

    [StringLength(100)]
    public string Manager { get; set; } = string.Empty; // 分类负责人

    // 佣金设置
    [Column(TypeName = "decimal(5,2)")]
    public decimal CommissionRate { get; set; }

    // 税率设置
    [Column(TypeName = "decimal(5,2)")]
    public decimal TaxRate { get; set; }

    // 分类属性模板
    [StringLength(2000)]
    public string AttributeTemplate { get; set; } = string.Empty; // JSON格式的属性模板

    // 销售相关设置
    public bool AllowOnlineOrder { get; set; } = true;
    public bool RequireApproval { get; set; } = false;

    [Column(TypeName = "decimal(10,2)")]
    public decimal MinOrderAmount { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal MaxOrderAmount { get; set; }

    // 库存管理设置
    public bool TrackInventory { get; set; } = true;
    public bool AllowNegativeStock { get; set; } = false;

    // 价格管理设置
    [Column(TypeName = "decimal(5,2)")]
    public decimal DefaultMarkupRate { get; set; } = 20.00m; // 默认加价率

    [Column(TypeName = "decimal(5,2)")]
    public decimal MaxDiscountRate { get; set; } = 10.00m; // 最大折扣率

    // 质量相关
    [StringLength(100)]
    public string QualityStandard { get; set; } = string.Empty; // 质量标准

    public bool RequireQualityCheck { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [StringLength(100)]
    public string CreatedBy { get; set; } = string.Empty;

    [StringLength(100)]
    public string? UpdatedBy { get; set; }

    // 统计信息
    public int ProductCount { get; set; } // 该分类下产品数量

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalSalesAmount { get; set; } // 总销售金额

    [Column(TypeName = "decimal(18,2)")]
    public decimal AveragePrice { get; set; } // 平均价格

    // 计算属性
    public string FullPath => GetCategoryPath();

    private string GetCategoryPath()
    {
        var path = CategoryName;
        var parent = ParentCategory;
        while (parent != null)
        {
            path = $"{parent.CategoryName} > {path}";
            parent = parent.ParentCategory;
        }
        return path;
    }
}