namespace HeYuanERP.Api.Requests.Logistics;

public class DeliveryListQuery
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 20;

    public string? DeliveryNo { get; set; }
    public string? Status { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public string? OrderNo { get; set; }
}

