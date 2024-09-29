using CarDealer.Models;
using CarDealer.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;

namespace CarDealer.Controllers
{
    public class CarItemController(CarDbContext context, IConfiguration config) : Controller
    {


        [HttpGet]
        public async Task<IActionResult> GetItem(int id)
        {
            var carItem = await context.CarItems.FindAsync(id);
            //var url = Url.Action("Index");
            return View(carItem);
        }
        [HttpGet]
        public async Task<IActionResult> Index(int productPage = 1)
        {
            int PageSize = 6;
            try
            {


                return View(new PageListView
                {
                    PageItem = new()
                    {
                        CurrentPage = productPage,
                        ItemPerPage = PageSize,
                        TotalItem = await context.CarItems.CountAsync()
                    },
                    CarItems = await context.CarItems
                    .OrderBy(p => p.Id)
                    .AsNoTracking()
                    .Skip((productPage - 1) * PageSize)
                    .Take(PageSize)
                    .ToListAsync()
                });
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return RedirectToAction("Index");
            }

        }
        [HttpGet]
        public async Task<IActionResult> DeleteItem(int id, string? name)
        {
            var carItem = await context.CarItems.FindAsync(id);
            if (carItem == null)
            {
                return RedirectToAction("Index");
            }
            return View(carItem);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var carItem = await context.CarItems.FindAsync(id);
            if (carItem == null)
            {
                return RedirectToAction("Index");
            }
            if (carItem.ImagePath != null )
            {
                FileRemover(carItem.ImagePath);
                FileRemover(carItem.ImagePath.Replace("original-images", "thumbnail-images"));
            }
            context.CarItems.Remove(carItem);
            await context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> EditItem(int id)
        {
            var carItem = await context.CarItems.FindAsync(id);
            if (carItem == null)
            {
                return RedirectToAction("Index");
            }
            CarListItem carListItem = new() { CarItem = carItem, Image = new() };
            return View(carListItem);
        }
        [HttpPost]
        
        public async Task<IActionResult> EditItem(int id, CarListItem? carListItem)
        {
            var carItem = await context.CarItems.FindAsync(id);
            if (carItem == null)
            {
                return RedirectToAction("Index");
            }
            if (carListItem != null)
            {
                CarItem carItem2 = carListItem.CarItem;
                carItem.Manufacturer = carItem2.Manufacturer;
                carItem.Model = carItem2.Model;
                carItem.Color= carItem2.Color;
                carItem.Year = carItem2.Year;
                carItem.Price = carItem2.Price;
                carItem.Features = carItem2.Features;
                carItem.WheelQuantity = carItem2.WheelQuantity;

                IFormFile? carImage = carListItem.Image?.Image;
                if (carImage != null && carImage.Length > 0 && carImage.FileName != null)
                {
                    var filePath = carItem.ImagePath!.Replace(carItem.ImagePath.Split("\\")[^1],
                        Guid.NewGuid().ToString() + carImage.FileName);
                    using (var streamObj = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                    {
                        await carImage.CopyToAsync(streamObj);
                    }
                    FileRemover(carItem.ImagePath);
                    FileRemover(carItem.ImagePath.Replace("original-images", "thumbnail-images"));
                    using (var imgStream = carImage.OpenReadStream())
                    {
                        ImageHelper.ResizeImage(imgStream, filePath.Replace("original-images", "thumbnail-images"), 150, 150);
                    }
                    carItem.ImagePath = filePath;

                }
                context.CarItems.Update(carItem);
                await context.SaveChangesAsync();
                return RedirectToAction("Index");


            }
            return RedirectToAction("EditItem");
        }



        [HttpGet("new")]
        public IActionResult PostCar() => View();

        [HttpPost("new")]
        public async Task<IActionResult> PostCar(CarListItem carListItem)
        {
            if (ModelState.IsValid)
            {
                var carImage = carListItem.Image.Image;
                if (carImage != null && carImage.Length > 0 && carImage.FileName != null)
                {
                    string imgName = Guid.NewGuid().ToString() + carImage.FileName;
                    var path = Path.Combine(Path.GetFullPath(Directory.GetCurrentDirectory()), "wwwroot", "uploaded-image");
                  
                    var originalPath = Path.Combine(path, "original-images");
                    string originalFilePath = Path.Combine(originalPath,imgName);

                    if (!Directory.Exists(originalPath))
                        Directory.CreateDirectory(originalPath);
                    if (!Directory.Exists(Path.Combine(path, "thumbnail-images")))
                        Directory.CreateDirectory(Path.Combine(path, "thumbnail-images"));

                    using (var streamObj = new FileStream(originalFilePath, FileMode.Create, FileAccess.ReadWrite))
                    {
                        await carImage.CopyToAsync(streamObj);
                    }
                    using (var imageStream = carImage.OpenReadStream())
                    {
                    var thumbnailFilePath = Path.Combine(path, "thumbnail-images", imgName);
                        ImageHelper.ResizeImage(imageStream, thumbnailFilePath, 150, 150);
                    }
                    carListItem.CarItem.ImagePath = originalFilePath;
                    await context.CarItems.AddAsync(carListItem.CarItem);
                    await context.SaveChangesAsync();
                }
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendUserInquiry(int id, [FromForm] string EmailAddress)
        {
            var carItem = await context.CarItems.FindAsync(id);
            string username = config.GetSection("Smtp")["Username"]!;
            if (carItem != null)
            {
                var mailMessage = new MailMessage()
                {
                    From = new(username),
                    Subject = $"Inquiry about {carItem.Model}",
                    Body = $"<div class='card' style='border-color:{carItem.Color}'> " +
                    $"<p class='card-header fs-3' style='border-color:{carItem.Color}'>{carItem.Manufacturer}</p>" +
                    $"<div class='card-body'>" +
                    $"<div class='d-flex flex-wrap gap-2 justify-content-between align-items-start mb-2'><p style=\"font-weight: 600\">{carItem.Model}</p> <button class=\"btn btn-success p-1\">{carItem.Price.ToString("c")}</button></div><div class=\"card-img img-top\"><img src=\"/uploaded-image/{carItem.ImagePath?.Split('\\')[^1]}\" alt='image' style=\"width: 300px; display: grid; margin: auto;\" /></div><p> Color:{carItem.Color}</p> <p>Wheel: {carItem.WheelQuantity}</p><p>Year: {carItem.Year}</p><p>Features: {carItem.Features}</p></div> " +
                    $"</div>" +
                    $"</div>",
                    IsBodyHtml = true
                };
                EmailService.SendMessage(config, mailMessage, EmailAddress);
                Console.WriteLine("\n\nSent\n\n");
            }
            return RedirectToAction("GetItem", new { id });
        }


        private static void FileRemover(string fileName)
        {
            FileInfo filePath;
            if (fileName != null && fileName != "")
            {
                filePath = new(fileName);
                if (filePath.Exists)
                    filePath.Delete();
                else
                {
                    Console.WriteLine($"Cannot delete: {fileName}");
                }
            }
        }
    }
}
