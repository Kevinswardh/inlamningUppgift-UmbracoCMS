using Umbraco.Cms.Core.Services;

namespace UmbracoCMS.Services
{
    public class ContactBoxSubmissionService
    {
        private readonly IContentService _contentService;

        public ContactBoxSubmissionService(IContentService contentService)
        {
            _contentService = contentService;
        }

        public bool SaveSubmission(string email, string page)
        {
            var parent = _contentService.GetRootContent()
        .FirstOrDefault(x => x.ContentType.Alias == "contactBoxFormSubmissions");


            if (parent == null) return false;

            var name = $"{DateTime.Now:yyyy-MM-dd HH:mm} - {email}";
            var node = _contentService.Create(name, parent, "contactBoxFormSubmission");

            node.SetValue("submissionEmail", email);
            node.SetValue("submissionPage", page);
            node.SetValue("submissionCreated", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));

            var saved = _contentService.Save(node);
            return saved.Success;
        }
    }
}
