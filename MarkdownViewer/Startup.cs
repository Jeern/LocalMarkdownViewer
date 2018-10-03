using System.IO;
using System.Reflection;
using Markdig.Syntax.Inlines;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace MarkdownViewer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var filepath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            var staticFilePaths = Configuration.GetValue<string>("StaticFilePaths");

            if (staticFilePaths != null)
            {
                var staticFilePathsArray = staticFilePaths.Split(',');
                foreach (var path in staticFilePathsArray)
                {
                    app.UseStaticFiles(new StaticFileOptions
                    {
                        FileProvider = new PhysicalFileProvider(Path.Combine(filepath, path)),
                        RequestPath = $"/{path}"
                    });
                }
            }

            app.Run(async context =>
            {
                var subPath = context?.Request?.Path.Value?.Substring(1);
                if (string.IsNullOrWhiteSpace(subPath))
                {
                    subPath = Configuration.GetValue<string>("StartPage") ?? "Readme.md";
                }

                var path = Path.Combine(filepath, subPath);
                if(!File.Exists(path))
                    return;

                await context.Response.WriteAsync(Markdig.Markdown.ToHtml(File.ReadAllText(path)));
            });
        }
    }
}
