using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Website.Controllers;
using UmbracoCMS.Services; // EmailService namespace

namespace UmbracoCMS.Controllers
{
    public class TestEmailController : SurfaceController
    {
        private readonly EmailService _email;

        public TestEmailController(
            Umbraco.Cms.Core.Web.IUmbracoContextAccessor umbracoContextAccessor,
            Umbraco.Cms.Infrastructure.Persistence.IUmbracoDatabaseFactory databaseFactory,
            Umbraco.Cms.Core.Services.ServiceContext services,
            Umbraco.Cms.Core.Cache.AppCaches appCaches,
            Umbraco.Cms.Core.Logging.IProfilingLogger logger,
            Umbraco.Cms.Core.Routing.IPublishedUrlProvider publishedUrlProvider,
            EmailService email)
            : base(umbracoContextAccessor, databaseFactory, services, appCaches, logger, publishedUrlProvider)
        {
            _email = email;
        }

        [HttpGet]
        public async Task<IActionResult> SendTest()
        {
            var result = await _email.SendEmailAsync(
                "kevinswwardh@gmail.com",
                "Umbraco Gmail Test",
                "Detta är ett testmail från din Umbraco-installation."
            );

            if (result)
                return Content("Testmail skickat ✔");
            else
                return Content("Misslyckades ❌ – kolla loggar eller SMTP-inställningar");
        }
    }
}
