using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Umbraco.Cms.Web.Website.Controllers;
using UmbracoCMS.Services;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;

namespace UmbracoCMS.Controllers
{
    public class FormSubmissionController : SurfaceController
    {
        private readonly ContactBoxSubmissionService _contactBoxService;
        private readonly QuestionFormSubmissionService _questionService;
        private readonly EmailService _emailService;

        public FormSubmissionController(
            IUmbracoContextAccessor umbracoContextAccessor,
            IUmbracoDatabaseFactory databaseFactory,
            ServiceContext services,
            AppCaches appCaches,
            IProfilingLogger profilingLogger,
            IPublishedUrlProvider publishedUrlProvider,
            ContactBoxSubmissionService contactBoxService,
            QuestionFormSubmissionService questionService,
            EmailService emailService)
            : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
            _contactBoxService = contactBoxService;
            _questionService = questionService;
            _emailService = emailService;
        }

        // CONTACT BOX ------------------------------------------------------------
        public class ContactBoxInput
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitContactBox(ContactBoxInput form)
        {
            if (!ModelState.IsValid)
            {
                TempData["contactBoxError"] = "Invalid email.";
                return RedirectToCurrentUmbracoPage();
            }

            var pageName = UmbracoContext?.PublishedRequest?.PublishedContent?.Name ?? "Unknown Page";

            _contactBoxService.SaveSubmission(
                form.Email,
                pageName
            );

            // 📧 Send confirmation email
            _emailService.SendEmailAsync(
                form.Email,
                "Your email has been registered",
                $@"<p>Thank you for registering your email: <strong>{form.Email}</strong>.</p>
                   <p>We will contact you shortly.</p>"
            );

            TempData["contactBoxSuccess"] = "Thank you! We’ll contact you soon.";
            return RedirectToCurrentUmbracoPage();
        }


        // QUESTION FORM ------------------------------------------------------------
        public class QuestionFormInput
        {
            [Required]
            public string Name { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            public string Message { get; set; }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitQuestion(QuestionFormInput form)
        {
            if (!ModelState.IsValid)
            {
                TempData["questionError"] = "Please fill all fields.";
                return RedirectToCurrentUmbracoPage();
            }

            var pageName = UmbracoContext?.PublishedRequest?.PublishedContent?.Name ?? "Unknown Page";

            _questionService.SaveSubmission(
                form.Name,
                form.Email,
                form.Message,
                pageName
            );

            // 📧 Send confirmation email
            _emailService.SendEmailAsync(
                form.Email,
                "We have received your message",
                $@"<p>Hello {form.Name},</p>
                    <p>Thank you for contacting us. We have received your message:</p>
                    <blockquote>{form.Message}</blockquote>
                    <p>We will get back to you as soon as possible.</p>"
            );

            TempData["questionSuccess"] = "Your message has been sent!";
            return RedirectToCurrentUmbracoPage();
        }
    }
}
