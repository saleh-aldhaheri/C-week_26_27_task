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
            context.Response.Redirect("/Admin/Home/Index");
            return;
        }

        await _next(context);
    } 
}
