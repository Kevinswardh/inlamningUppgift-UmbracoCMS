using Umbraco.Cms.Core.Services;

namespace UmbracoCMS.Services
{
    public class QuestionFormSubmissionService
    {
        private readonly IContentService _contentService;
        private readonly EmailService _emailService;

        public QuestionFormSubmissionService(IContentService contentService, EmailService emailService)
        {
            _contentService = contentService;
            _emailService = emailService;
        }

        public bool SaveSubmission(string name, string email, string message, string page)
        {
            // 🟦 Hämta rätt container (root node med alias: questionFormSubmissions)
            var parent = _contentService.GetRootContent()
                .FirstOrDefault(x => x.ContentType.Alias == "questionFormSubmissions");

            if (parent == null) return false;

            // 🟩 Skapa barn-node
            var title = $"{DateTime.Now:yyyy-MM-dd HH:mm} - {name}";
            var node = _contentService.Create(title, parent, "questionFormSubmission");

            node.SetValue("submissionName", name);
            node.SetValue("submissionEmail", email);
            node.SetValue("submissionMessage", message);
            node.SetValue("submissionPage", page);
            node.SetValue("submissionCreated", DateTime.Now);

            var saved = _contentService.Save(node);

            if (!saved.Success)
                return false;

            // 🟧 Skicka bekräftelsemail till användaren
            SendConfirmationEmail(name, email);

            return true;
        }

        private void SendConfirmationEmail(string name, string email)
        {
            var subject = "We have received your message ✔️";

            var body = $@"
                <h2>Thank you for contacting us, {name}!</h2>
                <p>We have received your message and our team will review it shortly.</p>
                <p>You will hear back from us as soon as possible.</p>
                <br/>
                <p>Regards,<br/>Onatrix Support</p>
            ";

            _emailService.SendEmailAsync(email, subject, body);
        }
    }
}
