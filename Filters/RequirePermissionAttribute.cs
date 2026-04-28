using AvyyanBackend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AvyyanBackend.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class RequirePermissionAttribute : TypeFilterAttribute
    {
        public RequirePermissionAttribute(string pageName, string action = "View") 
            : base(typeof(RequirePermissionFilter))
        {
            Arguments = new object[] { pageName, action };
        }
    }

    public class RequirePermissionFilter : IAsyncAuthorizationFilter
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly string _pageName;
        private readonly string _action;

        public RequirePermissionFilter(ApplicationDbContext dbContext, string pageName, string action)
        {
            _dbContext = dbContext;
            _pageName = pageName;
            _action = action;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Get role from claims
            var roleClaim = user.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(roleClaim))
            {
                context.Result = new ForbidResult();
                return;
            }

            // Admins bypass all restrictions
            if (roleClaim.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Look up role permissions
            var role = await _dbContext.RoleMasters
                .Include(r => r.PageAccesses)
                .FirstOrDefaultAsync(r => r.RoleName == roleClaim);

            if (role == null)
            {
                context.Result = new ForbidResult();
                return;
            }

            var pageAccess = role.PageAccesses
                .FirstOrDefault(p => string.Equals(p.PageName, _pageName, StringComparison.OrdinalIgnoreCase));

            if (pageAccess == null)
            {
                context.Result = new ForbidResult();
                return;
            }

            bool hasAccess = _action switch
            {
                "View" => pageAccess.IsView,
                "Add" => pageAccess.IsAdd,
                "Edit" => pageAccess.IsEdit,
                "Delete" => pageAccess.IsDelete,
                _ => false
            };

            if (!hasAccess)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
