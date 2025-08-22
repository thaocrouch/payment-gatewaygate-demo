using System.ComponentModel.DataAnnotations;

namespace WebClient.Models;

public class OrderRequest
{
    /// <summary>
    /// UserId - Guid or Ulid
    /// </summary>
    /// <example>D57B3468-5A64-4E6E-BBBA-606BFE11F22F</example>
    [Required]
    [MaxLength(50)]
    public string UserId { get; set; }
    
    /// <summary>
    /// Name of invoice
    /// </summary>
    /// <example>Invoid abc112233</example>
    [Required]
    [MaxLength(200)]
    [RegularExpression(@"^[A-Za-z0-9 ]*$", ErrorMessage = "Invalid name")]
    public string Name { get; set; }
    
    /// <summary>
    /// Payment amount
    /// </summary>
    /// <example>120000</example>
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Amount must be positive number")]
    public decimal Amount { get; set; }
    
    /// <summary>
    /// Payment method, ref payment method list.
    /// </summary>
    /// <example>1</example>
    [Required]
    [Range(0, 3, ErrorMessage = "Amount must be positive number")]
    public int PaymentMethod { get; set; }
    
    /// <summary>
    /// Type of transaction: one-time, recurring,...
    /// </summary>
    /// <example>0</example>
    [Required]
    [Range(0, 2, ErrorMessage = "Amount must be positive number")]
    public int PaymentSubType { get; set; }
}

public class OrderIpnRequest
{
    public string TransactionId { get; set; }
    public string RefId { get; set; }
    public int Status { get; set; }
    public int ErrorCode { get; set; }
    public string ErrorMessage { get; set; }
}

public class FilterOrderRequest
{
    /// <example>1</example>
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Amount must be positive number")]
    public int PageNumber { get; set; }
    
    /// <example>10</example>
    [Required]
    [Range(1, 500, ErrorMessage = "Amount must be positive number")]
    public int PageSize { get; set; }
    
    /// <summary>
    /// Invoice name
    /// </summary>
    /// <example></example>
    [RegularExpression(@"^[A-Za-z0-9 ]*$", ErrorMessage = "Invalid name")]
    public string Name { get; set; }
}

public class OrderPaging
{
    public int Total { get; set; }
    public IEnumerable<Order> data { get; set; } = new List<Order>();
}

public class Order
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public string Name { get; set; }
    public decimal Amount { get; set; }
    public string Note { get; set; }
    public int PaymentMethod { get; set; }
    public int PaymentSubType { get; set; }
    public int Status { get; set; }
    public string CallbackUrl { get; set; }
    public string IpnUrl { get; set; }
    public int ErrorCode { get; set; }
    public string ErrorMessage { get; set; }
    public DateTime ExpireDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}