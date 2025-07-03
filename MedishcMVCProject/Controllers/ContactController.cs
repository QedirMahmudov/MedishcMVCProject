using MedishcMVCProject.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;

namespace MedishcMVCProject.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SubmitContactForm(ContactFormModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(model.Email, model.Name),
                        Subject = $"Vebsaytdan mesaj: {model.Subject}",
                        Body = $@"
                    <h3>Yeni mesaj vebsaytdan</h3>
                    <p><strong>Ad:</strong> {model.Name}</p>
                    <p><strong>Email:</strong> {model.Email}</p>
                    <p><strong>Telefon:</strong> {model.Phone}</p>
                    <p><strong>Mövzu:</strong> {model.Subject}</p>
                    <p><strong>Mesaj:</strong></p>
                    <p>{model.Message}</p>
                ",
                        IsBodyHtml = true,
                    };

                    mailMessage.To.Add("lapulos7@gmail.com");

                    using (var smtpClient = new SmtpClient("smtp.gmail.com")
                    {
                        Port = 587,
                        Credentials = new NetworkCredential("uvapebro9@gmail.com", "hjhy jbxx wfzp foxb"),
                        EnableSsl = true,
                    })
                    {
                        await smtpClient.SendMailAsync(mailMessage);
                    }

                    return Json(new { success = true, message = "Your message has been sent successfully!" });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "An error occurred. Please try again." });
                }
            }

            return Json(new { success = false, message = "Invalid data submitted." });
        }
    }
}
