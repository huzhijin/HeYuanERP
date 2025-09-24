using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeYuanERP.Domain.Entities;

[Table("Products")]
public class Product
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    [Required]
    [StringLength(50)]
    public string Code { get; set; } = string.Empty;   // 物料编码（唯一）

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;   // 物料名称

    [StringLength(100)]
    public string? Spec { get; set; }                  // 规格型号

    [StringLength(50)]
    public string? Unit { get; set; }                  // 计量单位

    [Column(TypeName = "decimal(18,2)")]
    public decimal? DefaultPrice { get; set; }         // 默认含税单价

    public bool Active { get; set; } = true;           // 是否启用

    // P1扩展字段 - 产品管理增强
    [StringLength(50)]
    public string BarCode { get; set; } = string.Empty;

    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    public int? CategoryId { get; set; }

    [ForeignKey("CategoryId")]
    public virtual ProductCategory? Category { get; set; }

    [StringLength(100)]
    public string Brand { get; set; } = string.Empty;

    [StringLength(100)]
    public string Model { get; set; } = string.Empty;

    [StringLength(50)]
    public string Status { get; set; } = "Active"; // Active, Inactive, Discontinued

    // 价格信息
    [Column(TypeName = "decimal(18,2)")]
    public decimal CostPrice { get; set; } // 成本价

    [Column(TypeName = "decimal(18,2)")]
    public decimal StandardPrice { get; set; } // 标准价格

    [Column(TypeName = "decimal(18,2)")]
    public decimal SalesPrice { get; set; } // 销售价格

    [Column(TypeName = "decimal(18,2)")]
    public decimal MinPrice { get; set; } // 最低价格

    [Column(TypeName = "decimal(18,2)")]
    public decimal MaxPrice { get; set; } // 最高价格

    // 库存信息
    [Column(TypeName = "decimal(10,2)")]
    public decimal CurrentStock { get; set; } // 当前库存

    [Column(TypeName = "decimal(10,2)")]
    public decimal SafetyStock { get; set; } // 安全库存

    [Column(TypeName = "decimal(10,2)")]
    public decimal ReorderPoint { get; set; } // 再订购点

    // 供应商信息
    public int? PrimarySupplierId { get; set; } // 主供应商

    [StringLength(100)]
    public string PrimarySupplierName { get; set; } = string.Empty;

    // 产品属性
    [Column(TypeName = "decimal(8,2)")]
    public decimal Weight { get; set; }

    [StringLength(50)]
    public string Color { get; set; } = string.Empty;

    [StringLength(50)]
    public string Size { get; set; } = string.Empty;

    [StringLength(100)]
    public string Material { get; set; } = string.Empty;

    // 图片和文件
    [StringLength(500)]
    public string MainImageUrl { get; set; } = string.Empty;

    [StringLength(2000)]
    public string ImageUrls { get; set; } = string.Empty; // JSON数组格式

    // 销售信息
    public bool IsOnSale { get; set; } = true;

    public bool AllowOnlineOrder { get; set; } = true;

    public bool RequireApproval { get; set; } = false;

    [Column(TypeName = "decimal(10,2)")]
    public decimal MinOrderQuantity { get; set; } = 1;

    [Column(TypeName = "decimal(10,2)")]
    public decimal MaxOrderQuantity { get; set; } = 999999;

    // 质量信息
    [StringLength(100)]
    public string QualityGrade { get; set; } = string.Empty;

    public bool RequireQualityCheck { get; set; } = false;

    [StringLength(100)]
    public string WarrantyPeriod { get; set; } = string.Empty; // 保修期

    // 税务信息
    [Column(TypeName = "decimal(5,2)")]
    public decimal TaxRate { get; set; }

    [StringLength(50)]
    public string TaxCategory { get; set; } = string.Empty;

    // 生产信息
    [StringLength(100)]
    public string Manufacturer { get; set; } = string.Empty;

    [StringLength(100)]
    public string OriginCountry { get; set; } = string.Empty;

    // 统计信息
    public int SalesCount { get; set; } // 销售数量

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalSalesAmount { get; set; } // 总销售金额

    public DateTime? LastSaleDate { get; set; } // 最后销售日期

    // SEO和搜索优化
    [StringLength(200)]
    public string Keywords { get; set; } = string.Empty;

    public int ViewCount { get; set; } // 查看次数

    public decimal Rating { get; set; } // 评分

    public int ReviewCount { get; set; } // 评价数量

    // 审计字段
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // 扩展属性 (JSON格式)
    [StringLength(4000)]
    public string ExtendedAttributes { get; set; } = string.Empty;

    // 计算属性
    public decimal GrossMargin => StandardPrice > 0 ? ((SalesPrice - CostPrice) / SalesPrice) * 100 : 0;

    public bool IsLowStock => CurrentStock <= SafetyStock;

    public bool IsOutOfStock => CurrentStock <= 0;
}

