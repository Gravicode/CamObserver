using CamObserver.Data;
using CamObserver.Services.Helpers;
using CamObserver.Services.IServices;
using CamObserver.Services.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace CamObserver.Services.Extension
{
    public static class ServiceExtensions
    {
        public static void InjectDbWorkspace(this IServiceCollection services)
        {
            // database context
            services.AddScoped<DbContext, CamObserverDB>();
            services.AddTransient<IWorkspace, Workspace>();

            services.AddTransient<AzureBlobHelper>();
        }

        public static void CustomErrorHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(error =>
            {
                error.Run(async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

                    if (contextFeature is not null)
                    {
                        Log.Error($"Something when wrong in the {contextFeature.Error}");

                        await context.Response.WriteAsync(new Models.Error
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = $"Internal server error, please try again"
                        }.ToString());
                    }
                });
            });
        }
    }
}
