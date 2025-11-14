using CmsWin._24.Services;
using CmsWin._24.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Website.Controllers;

namespace UmbracoCMS.Controllers
{
    public class FormController : SurfaceController
    {
        private readonly FormSubmissionsService _formSubmissions;

        public FormController(
            IUmbracoContextAccessor umbracoContextAccessor,
            IUmbracoDatabaseFactory databaseFactory,
            ServiceContext services,
            AppCaches appCaches,
            IProfilingLogger profilingLogger,
            IPublishedUrlProvider publishedUrlProvider,
            FormSubmissionsService formSubmissions)
            : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
            _formSubmissions = formSubmissions;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult HandleCallbackForm(CallbackFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["FormError"] = "Model state is wrong";
                return CurrentUmbracoPage();
            }

            var result = _formSubmissions.SaveCallbackRequest(model);
            if (!result)
            {
                TempData["FormError"] = "Something went wrong while submitting your request. Please try again later.";
                return RedirectToCurrentUmbracoPage();
            }

            TempData["FormSuccess"] = "Thank you! Your request has been received and we will get back to you soon.";
            return RedirectToCurrentUmbracoPage();

        }
    }
}
