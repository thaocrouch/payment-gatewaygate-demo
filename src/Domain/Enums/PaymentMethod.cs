namespace Domain.Enums;

public enum PaymentMethod
{
    STRIPE = 0,
    MOMO = 1,
    ZALOPAY = 2,
    VNPAY = 3,
    ALL = 3 // Config range value
    // any more
}

public enum PaymentSubType
{
    ONETIME = 0,
    RECURRING = 1,
    ALL = 2 // Config range value
}