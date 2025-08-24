using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace PaymentGateway.Api.Models;

public class CreateOrderRequest
{
    /// <summary>
    ///     UserId - Guid or Ulid
    /// </summary>
    /// <example>D57B3468-5A64-4E6E-BBBA-606BFE11F22F</example>
    [Required]
    [MaxLength(50)]
    public string UserId { get; set; }

    /// <summary>
    ///     Name of invoice
    /// </summary>
    /// <example>Invoid abc112233</example>
    [Required]
    [MaxLength(200)]
    [RegularExpression(@"^[A-Za-z0-9 _\-]*$", ErrorMessage = "Invalid name")]
    public string Name { get; set; }

    /// <summary>
    ///     Payment amount
    /// </summary>
    /// <example>120000</example>
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Amount must be positive number")]
    public decimal Amount { get; set; }

    /// <summary>
    ///     Payment method, ref payment method list.
    /// </summary>
    /// <example>1</example>
    [Required]
    [Range(0, (int)Domain.Enums.PaymentMethod.ALL, ErrorMessage = "Amount must be positive number")]
    public int PaymentMethod { get; set; }

    /// <summary>
    ///     Type of transaction: one-time, recurring,...
    /// </summary>
    /// <example>0</example>
    [Required]
    [Range(0, (int)Domain.Enums.PaymentSubType.ALL, ErrorMessage = "Amount must be positive number")]
    public int PaymentSubType { get; set; }
}

public class UpdateOrderRequest
{
    /// <summary>
    ///     Status of transaction, ref Transaction status list.
    /// </summary>
    /// <example>1</example>
    [Required]
    [Range((int)TransactionStatus.FAIL, (int)TransactionStatus.SUCCESS, ErrorMessage = "Invalid transaction status")]
    public int Status { get; set; }

    /// <summary>
    ///     Process error code
    /// </summary>
    public int ErrorCode { get; set; }

    /// <summary>
    ///     Error message
    /// </summary>
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
    ///     Invoice name
    /// </summary>
    /// <example></example>
    [RegularExpression(@"^[A-Za-z0-9 ]*$", ErrorMessage = "Invalid name")]
    public string? Name { get; set; }
}

public class UserFilterOrderRequest
{
    /// <example>1</example>
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Amount must be positive number")]
    public int PageNumber { get; set; }

    /// <example>10</example>
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Amount must be positive number")]
    public int PageSize { get; set; }

    /// <example>D57B3468-5A64-4E6E-BBBA-606BFE11F22F</example>
    [RegularExpression(@"^[A-Za-z0-9-_ ]*$", ErrorMessage = "Invalid UserId")]
    public string UserId { get; set; }
}

public class OrderMessageRequest
{
    /// <example>Order-1111</example>
    public string Id { get; set; }

    /// <example>D57B3468-5A64-4E6E-BBBA-606BFE11F22F</example>
    public string UserId { get; set; }

    /// <example>1</example>
    public int Status { get; set; }
}

public class OrderPayRequest
{
    /// <example>Order-1111</example>
    public string Id { get; set; }

    /// <summary>
    ///     Success: 1, Fail: -1
    /// </summary>
    /// <example>1</example>
    public int Status { get; set; }
}