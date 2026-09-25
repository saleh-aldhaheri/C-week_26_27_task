using week_26_27.Utilities;

namespace week_26_27.Middlewares;

public class GuestMiddleware
{
    private readonly RequestDelegate _next;

    public GuestMiddleware(RequestDelegate next)
    {
        _next = next; 
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            if(context.User.IsInRole(RoleConstants.CUSTOMER))
            {
                context.Response.Redirect("/Customer/Home/Index");
            }else
            {
                context.Response.Redirect("/Admin/Home/Index");
            }

            return;
        }

        await _next(context);
    } 
}
