using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using backend.Models.Enums;

namespace backend.Controllers;

[ApiController]
public abstract class BaseController : ControllerBase
{
    protected long GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return long.TryParse(userIdClaim, out var userId) ? userId : 0;
    }

    protected bool IsAdmin()
    {
        var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
        return roleClaim == UserRole.Admin.ToString();
    }

    protected bool CanAccessUser(long userId)
    {
        // Admin can access any user, regular users can only access themselves
        return IsAdmin() || GetCurrentUserId() == userId;
    }

    protected IActionResult ValidateId(long id, string parameterName = "id")
    {
        if (id <= 0)
        {
            return BadRequest(new { 
                error = $"Invalid {parameterName}. Must be a positive number.",
                field = parameterName,
                value = id
            });
        }
        return null!; // null means validation passed
    }

    protected IActionResult ValidateIds(params (long id, string name)[] idValidations)
    {
        foreach (var (id, name) in idValidations)
        {
            var validation = ValidateId(id, name);
            if (validation != null)
                return validation;
        }
        return null!; // null means all validations passed
    }

    protected IActionResult HandleServiceError(string errorMessage)
    {
        // Check for common error patterns and return appropriate status codes
        if (errorMessage.Contains("not found", StringComparison.OrdinalIgnoreCase))
        {
            return NotFound(new { error = errorMessage });
        }
        
        if (errorMessage.Contains("unauthorized", StringComparison.OrdinalIgnoreCase) ||
            errorMessage.Contains("permission", StringComparison.OrdinalIgnoreCase) ||
            errorMessage.Contains("not allowed", StringComparison.OrdinalIgnoreCase))
        {
            return Forbid(errorMessage);
        }

        if (errorMessage.Contains("invalid", StringComparison.OrdinalIgnoreCase) ||
            errorMessage.Contains("validation", StringComparison.OrdinalIgnoreCase) ||
            errorMessage.Contains("required", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new { error = errorMessage });
        }

        // Default to BadRequest for other errors
        return BadRequest(new { error = errorMessage });
    }
}