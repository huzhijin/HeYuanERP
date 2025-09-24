using HeYuanERP.Domain.Entities;
using HeYuanERP.Api.Common;

namespace HeYuanERP.Api.Services.ProductPrice;

public interface IProductPriceService
{
    // 产品分类管理
    Task<ApiResponse<ProductCategory>> CreateProductCategoryAsync(ProductCategory category);
    Task<ApiResponse<ProductCategory>> UpdateProductCategoryAsync(ProductCategory category);
    Task<ApiResponse<bool>> DeleteProductCategoryAsync(int id);
    Task<ApiResponse<ProductCategory?>> GetProductCategoryByIdAsync(int id);
    Task<ApiResponse<List<ProductCategory>>> GetProductCategoriesAsync();
    Task<ApiResponse<List<ProductCategory>>> GetProductCategoryTreeAsync();
    Task<ApiResponse<List<ProductCategory>>> GetSubCategoriesAsync(int parentId);

    // 产品管理
    Task<ApiResponse<Product>> CreateProductAsync(Product product);
    Task<ApiResponse<Product>> UpdateProductAsync(Product product);
    Task<ApiResponse<bool>> DeleteProductAsync(string id);
    Task<ApiResponse<Product?>> GetProductByIdAsync(string id);
    Task<ApiResponse<Product?>> GetProductByCodeAsync(string code);
    Task<ApiResponse<List<Product>>> GetProductsByCategoryAsync(int categoryId);
    Task<ApiResponse<List<Product>>> SearchProductsAsync(string searchTerm);
    Task<ApiResponse<List<Product>>> GetProductsAsync(int pageNumber = 1, int pageSize = 50);
    Task<ApiResponse<bool>> UpdateProductStockAsync(string productId, decimal newStock);
    Task<ApiResponse<List<Product>>> GetLowStockProductsAsync();
    Task<ApiResponse<List<Product>>> GetOutOfStockProductsAsync();

    // 价格策略管理
    Task<ApiResponse<PriceStrategy>> CreatePriceStrategyAsync(PriceStrategy strategy);
    Task<ApiResponse<PriceStrategy>> UpdatePriceStrategyAsync(PriceStrategy strategy);
    Task<ApiResponse<bool>> DeletePriceStrategyAsync(int id);
    Task<ApiResponse<PriceStrategy?>> GetPriceStrategyByIdAsync(int id);
    Task<ApiResponse<List<PriceStrategy>>> GetPriceStrategiesAsync();
    Task<ApiResponse<List<PriceStrategy>>> GetActivePriceStrategiesAsync();
    Task<ApiResponse<decimal>> CalculatePriceWithStrategyAsync(string productId, int customerId, decimal quantity, int? strategyId = null);
    Task<ApiResponse<List<PriceStrategy>>> GetApplicablePriceStrategiesAsync(string productId, int customerId, decimal quantity);

    // 特价申请管理
    Task<ApiResponse<PriceRequest>> CreatePriceRequestAsync(PriceRequest request);
    Task<ApiResponse<PriceRequest>> UpdatePriceRequestAsync(PriceRequest request);
    Task<ApiResponse<bool>> DeletePriceRequestAsync(int id);
    Task<ApiResponse<PriceRequest?>> GetPriceRequestByIdAsync(int id);
    Task<ApiResponse<List<PriceRequest>>> GetPriceRequestsByRequesterAsync(int requesterId);
    Task<ApiResponse<List<PriceRequest>>> GetPriceRequestsByStatusAsync(string status);
    Task<ApiResponse<List<PriceRequest>>> GetPendingPriceRequestsAsync();
    Task<ApiResponse<PriceRequest>> ApprovePriceRequestAsync(int id, decimal approvedPrice, string comments, int approverId);
    Task<ApiResponse<PriceRequest>> RejectPriceRequestAsync(int id, string reason, int approverId);

    // 报价管理
    Task<ApiResponse<Quotation>> CreateQuotationAsync(Quotation quotation);
    Task<ApiResponse<Quotation>> UpdateQuotationAsync(Quotation quotation);
    Task<ApiResponse<bool>> DeleteQuotationAsync(int id);
    Task<ApiResponse<Quotation?>> GetQuotationByIdAsync(int id);
    Task<ApiResponse<Quotation?>> GetQuotationByNumberAsync(string quotationNumber);
    Task<ApiResponse<List<Quotation>>> GetQuotationsByCustomerAsync(int customerId);
    Task<ApiResponse<List<Quotation>>> GetQuotationsBySalespersonAsync(int salespersonId);
    Task<ApiResponse<List<Quotation>>> GetQuotationsByStatusAsync(string status);
    Task<ApiResponse<List<Quotation>>> GetExpiringQuotationsAsync(int days = 7);
    Task<ApiResponse<Quotation>> SendQuotationAsync(int id);
    Task<ApiResponse<Quotation>> AcceptQuotationAsync(int id);
    Task<ApiResponse<Quotation>> RejectQuotationAsync(int id, string reason);

    // 报价单项管理
    Task<ApiResponse<QuotationItem>> AddQuotationItemAsync(QuotationItem item);
    Task<ApiResponse<QuotationItem>> UpdateQuotationItemAsync(QuotationItem item);
    Task<ApiResponse<bool>> RemoveQuotationItemAsync(int id);
    Task<ApiResponse<List<QuotationItem>>> GetQuotationItemsAsync(int quotationId);
    Task<ApiResponse<bool>> RecalculateQuotationTotalsAsync(int quotationId);

    // 价格计算和分析
    Task<ApiResponse<decimal>> CalculateOptimalPriceAsync(string productId, int customerId, decimal quantity);
    Task<ApiResponse<object>> GetPriceAnalysisAsync(string productId, DateTime startDate, DateTime endDate);
    Task<ApiResponse<object>> GetProfitMarginAnalysisAsync(string productId);
    Task<ApiResponse<object>> GetCompetitivePriceAnalysisAsync(string productId);

    // 统计和报告
    Task<ApiResponse<object>> GetProductSalesStatsAsync(DateTime startDate, DateTime endDate);
    Task<ApiResponse<object>> GetPriceRequestStatsAsync(DateTime startDate, DateTime endDate);
    Task<ApiResponse<object>> GetQuotationStatsAsync(DateTime startDate, DateTime endDate);
    Task<ApiResponse<object>> GetProfitabilityReportAsync(DateTime startDate, DateTime endDate);
    Task<ApiResponse<List<object>>> GetTopSellingProductsAsync(int topN = 10);
    Task<ApiResponse<List<object>>> GetTopProfitableProductsAsync(int topN = 10);

    // 库存管理相关
    Task<ApiResponse<object>> GetInventoryValueReportAsync();
    Task<ApiResponse<List<Product>>> GetReorderRequiredProductsAsync();
    Task<ApiResponse<bool>> UpdateMultipleProductStockAsync(List<(string productId, decimal newStock)> updates);

    // 价格历史和趋势
    Task<ApiResponse<List<object>>> GetProductPriceHistoryAsync(string productId);
    Task<ApiResponse<object>> GetPriceTrendAnalysisAsync(string productId, int months = 12);

    // 批量操作
    Task<ApiResponse<bool>> BulkUpdateProductPricesAsync(List<(string productId, decimal newPrice)> updates);
    Task<ApiResponse<bool>> BulkApplyPriceStrategyAsync(int strategyId, List<string> productIds);

    // 导入导出
    Task<ApiResponse<bool>> ImportProductsAsync(byte[] fileData, string fileName);
    Task<ApiResponse<byte[]>> ExportProductsAsync(List<string> productIds);
    Task<ApiResponse<byte[]>> ExportQuotationAsync(int quotationId, string format = "PDF");
}