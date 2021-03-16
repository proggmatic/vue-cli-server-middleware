using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;


namespace Proggmatic.SpaServices.VueCli
{
    public static class FixSpaPathBaseBugMiddleware
    {
        /// <summary>
        /// Need only if you prefer to use non-automatic proxy like this <code>spa.UseProxyToSpaDevelopmentServer("http://localhost:8080/my-custom-path");</code>
        /// Because of bug in Microsoft's middleware (SpaProxy.cs line 65), we need to temporary clear PathBase, moving it to Path.
        /// Use it right before <code>spa.UseProxyToSpaDevelopmentServer("http://localhost:8080/my-custom-path");</code>
        /// </summary>
        public static void UseFixSpaPathBaseBugMiddleware(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                var oldPath = context.Request.Path;
                var oldPathBase = context.Request.PathBase;
                
                var newPath = oldPathBase + oldPath;
                var newPathBase = new PathString(string.Empty);
                
                // Set new paths to fix the bug
                context.Request.Path = newPath;
                context.Request.PathBase = newPathBase;

                try
                {
                    await next();
                }
                finally
                {
                    // Restore back only if any next middleware changed nothing
                    if (context.Request.PathBase == newPathBase && context.Request.Path == newPath)
                    {
                        context.Request.PathBase = oldPathBase;
                        context.Request.Path = oldPath;
                    }
                }
            });
        }
    }
}