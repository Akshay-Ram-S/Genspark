using Microsoft.AspNetCore.Authorization;

namespace FirstAPI.Authorization
{
    public class ExperienceRequirement : IAuthorizationRequirement
    {
        public int RequiredYears { get; }

        public ExperienceRequirement(int years)
        {
            RequiredYears = years;
        }
    }
}

