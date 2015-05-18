using System.IO;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Net.Http.Headers;

namespace Panther.Mail.Mvc
{
    public class EmailResult : ActionResult
    {
        private readonly Email email;
        private readonly SmtpClient smtpClient;
        public EmailResult(Email model)
        {
            email = model;
            EmailViewDirectoryName = "Emails";
            smtpClient = new SmtpClient("127.0.0.1", 25);
        }

        public int? StatusCode { get; set; }
        public string EmailViewDirectoryName { get; set; }

        public IViewEngine ViewEngine { get; set; }

        public IEmailParser EmailParser { get; set; }

        public MediaTypeHeaderValue ContentType { get; set; }

        public override async Task ExecuteResultAsync(ActionContext context)
        {
            var viewEngine = ViewEngine ?? context.HttpContext.RequestServices.GetRequiredService<ICompositeViewEngine>();
            var emailParser = EmailParser ?? context.HttpContext.RequestServices.GetRequiredService<IEmailParser>();
            var viewName = email.ViewName ?? context.ActionDescriptor.Name;

            var result = string.Empty;

            context.RouteData.Values["controller"] = EmailViewDirectoryName;

            var viewEngineResult = viewEngine.FindView(context, viewName);
            var view = viewEngineResult.EnsureSuccessful().View;
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            {
                var viewContext = new ViewContext(context, view, email.ViewData, email.TempData, writer);
                await view.RenderAsync(viewContext);
                writer.Flush();
                result = Encoding.UTF8.GetString(stream.ToArray());
            }

            var mail = emailParser.Parse(result, email);
            smtpClient.SendAsync(mail);
        }
    }
}
