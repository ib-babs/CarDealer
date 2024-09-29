using Microsoft.AspNetCore.Mvc;
using CarDealer.Models;
using CarDealer.Service;
using System.Net.Mail;
namespace CarDealer.Controllers
{
    public class ContactUsController(IConfiguration config) : Controller
    {
        [HttpGet]
        public IActionResult Index() => View();

        [HttpPost]
        public IActionResult Index(ContactItem contactItem)
        {
            if (ModelState.IsValid)
            {
                var username = config.GetSection("Smtp")["Username"]!;
                var mailMessage = new MailMessage()
                {
                    From = new(username),
                    Subject = contactItem.Subject,
                    Body = $"From: {contactItem.EmailAddress}\n{contactItem.Message}",
                    IsBodyHtml = false
                };

                EmailService.SendMessage(config, mailMessage, username);
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}