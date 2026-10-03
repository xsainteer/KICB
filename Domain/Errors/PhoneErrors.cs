using Domain.Common;

namespace Domain.Errors;

public static class PhoneErrors
{
    public static Error NotFound(Guid phoneId) =>
        new("Phone.NotFound",
            $"Phone {phoneId} was not found for this client.",
            ErrorType.NotFound);
}
