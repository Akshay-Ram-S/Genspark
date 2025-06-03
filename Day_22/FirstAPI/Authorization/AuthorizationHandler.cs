using FirstAPI.Authorization;
using Microsoft.AspNetCore.Authorization;

public class ExperienceAuthorizationHandler : AuthorizationHandler<ExperienceRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ExperienceRequirement requirement)
    {
        var experienceClaim = context.User.FindFirst(c => c.Type == "ExperienceYears");

        if (experienceClaim != null && float.TryParse(experienceClaim.Value, out float years) && years >= requirement.RequiredYears)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
