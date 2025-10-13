using backend.Data;
using Shop.Shared.Results;

namespace backend.Services;

public class BaseService
{
    protected readonly AppDbContext shopContext;

    protected BaseService(AppDbContext shopContext)
    {
        this.shopContext = shopContext;
    }

    protected static Result<T> ValidateId<T>(long id, string parameterName = "id")
    {
        if (id <= 0)
        {
            return Result<T>.Failure($"Invalid {parameterName}. Must be a positive number greater than 0. Received: {id}");
        }
        return Result<T>.Success(default(T)!); // Success, value doesn't matter for validation
    }

    protected static Result ValidateId(long id, string parameterName = "id")
    {
        if (id <= 0)
        {
            return Result.Failure($"Invalid {parameterName}. Must be a positive number greater than 0. Received: {id}");
        }
        return Result.Success();
    }

    protected static Result ValidateIds(params (long id, string name)[] idValidations)
    {
        foreach (var (id, name) in idValidations)
        {
            var validation = ValidateId(id, name);
            if (!validation.IsSuccess)
                return validation;
        }
        return Result.Success();
    }
}
