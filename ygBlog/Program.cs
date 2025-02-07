using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using ygBlog.Managment;
using ygBlog.WebApi.Query;

namespace ygBlog
{
    public class Program
    {
        public static Database.Database db;
        public static void Main(string[] args)
        {
            Console.WriteLine("ygBlog v1.0");
            Console.WriteLine("Database init.");
            db = new Database.Database(Path.Combine(FileDir.Data, "data.db"));
            Settings.SetDB(db);

            //ASP.NET init

            var builder = WebApplication.CreateBuilder(new WebApplicationOptions { 
                WebRootPath = "wwwroot",
            });

            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            builder.Services.AddControllersWithViews(x => {
                x.Filters.Add<WebApiExceptionFilter>();
            });
            builder.Services.Configure<ApiBehaviorOptions>(x => {
                x.SuppressModelStateInvalidFilter = true;
            });
            builder.Services.Configure<FormOptions>(x => {
                x.ValueLengthLimit = 1024 * 1024 * 10;
                x.MultipartBodyLengthLimit = 1024 * 1024 * 10;
                x.MultipartHeadersLengthLimit = 1024 * 1024 * 5;
            });
            builder.Services.AddControllers().AddNewtonsoftJson();

            // Add services to the container.
            builder.Services.AddRazorPages();



            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(o =>
                {
                    o.LoginPath = "/login";
                    o.Events.OnValidatePrincipal = async (c) => {
                        var ticket = c.Principal.Claims.ToList().Where(x => x.Type == "ticket");
                        if (ticket.Count() <= 0) {
                            c.RejectPrincipal();
                            return;
                        }
                        if (!SessionManager.TicketAvailable(ticket.First().Value)) {
                            c.RejectPrincipal();
                            return;
                        }
                    };
                    o.ReturnUrlParameter = "ret";
                });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/DevError");
            }
            else {
                app.UseExceptionHandler("/Error");
            }

            app.UseForwardedHeaders();

            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(FileDir.PageResource),
                RequestPath = "/pageres"
            });

            app.UseStatusCodePages(new StatusCodePagesOptions { 
                HandleAsync = new Func<Microsoft.AspNetCore.Diagnostics.StatusCodeContext, Task>(async context =>
                {
                    switch (context.HttpContext.Response.StatusCode)
                    {
                        case 404:
                            context.HttpContext.Response.Redirect("/UserError?code=" + context.HttpContext.Response.StatusCode);
                            break;
                    }
                })
            });

            app.UseRouting();

            app.MapControllers();

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
