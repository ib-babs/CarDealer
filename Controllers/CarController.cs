using Azure.Storage.Blobs;
using CarDealer.Interfaces;
using CarDealer.Models;
using CarDealer.Service;
using CarDealer.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;


namespace CarDealer.Controllers
{
    public class CarController(ICarRepository carRepository, IConfiguration configuration, ILogger<CarController> logger, BlobServiceClient blobService) : Controller
    {
        private readonly ICarRepository _carRepository = carRepository;
        private readonly ILogger<CarController> _logger = logger;


        [HttpGet]
        public async Task<IActionResult> GetCar(int id)
        {
            if (!await _carRepository.CarExistsAsync(id)) return NotFound();
            return View(await _carRepository.GetCarAsync(id));
        }
        [HttpGet]
        public async Task<IActionResult> Index(int productPage = 1)
        {
            _logger.LogInformation(message: configuration.GetConnectionString("SqlServerConnectionString"));
            _logger.LogInformation(message: configuration.GetConnectionString("BlobConnectionString"));
            _logger.LogInformation(message: Environment.GetEnvironmentVariable("SmtpUsername"));
            _logger.LogInformation(message: Environment.GetEnvironmentVariable("SmtpPassword"));

            ICollection<CarItem> cars = await _carRepository.GetCarsAsync();
            int PageSize = 6;

            return View(new PageListView
            {
                PageItem = new()
                {
                    CurrentPage = productPage,
                    ItemPerPage = PageSize,
                    TotalItem = cars.Count
                },
                Cars = cars.Skip((productPage - 1) * PageSize)
                .Take(PageSize)
            });


        }
        [HttpGet]
        public async Task<IActionResult> DeleteCar(int id, string? _name)
        {
            if (!await _carRepository.CarExistsAsync(id)) return NotFound();
            var car = await _carRepository.GetCarAsync(id);
            return View(car);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteCar(int id)
        {
            if (!await _carRepository.CarExistsAsync(id)) return NotFound();
            var car = await _carRepository.GetCarAsync(id);
            var isImageDeleted = await AzureBlobContainer.DeleteImageAsync(blobService, car.ImagePath!);
            if (!isImageDeleted)
            {
                return BadRequest();
            }
            await _carRepository.DeleteCarAsync(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> EditCar(int id)
        {
            if (!await _carRepository.CarExistsAsync(id)) return NotFound();

            var car = await _carRepository.GetCarAsync(id);
            return View(car);
        }
        [HttpPost]

        public async Task<IActionResult> EditCar(int id, CarItem car)
        {
            if (car == null)
            {
                return BadRequest(ModelState);
            }
            if (!await _carRepository.CarExistsAsync(id)) return NotFound();

            if (!ModelState.IsValid)
            {
                return View();
            }
            var updatedCar = await _carRepository.UpdateCarAsync(id, car);
            return RedirectToAction(nameof(Index));
        }



        [HttpGet]
        public IActionResult CreateCar() => View();
        

        [HttpPost]
        public async Task<IActionResult> CreateCar(CarListItem carItem)
        {
            _logger.Log(LogLevel.Information, message: configuration.GetConnectionString("SqlServerConnectionString"));
            _logger.Log(LogLevel.Information, message: configuration.GetConnectionString("BlobConnectionString"));
            _logger.Log(LogLevel.Information, message: Environment.GetEnvironmentVariable("SmtpUsername"));
            _logger.Log(LogLevel.Information, message: Environment.GetEnvironmentVariable("SmtpPassword"));
            if (carItem == null || carItem.Car == null || carItem.Image == null)
            {
                Console.WriteLine("What is null?");
                return BadRequest(ModelState);
            }
            Console.WriteLine(carItem.Image.FileName);
            Console.WriteLine(carItem.Image.Length);
            if (carItem.Image.Length <= 0 || string.IsNullOrWhiteSpace(carItem.Image.FileName))
            {
                ModelState.AddModelError("", "Something is wrong with the image file");
                return BadRequest(ModelState);
            }

            (bool IsSuccess, string imageUrl) = await AzureBlobContainer.UploadImageAsync(blobService, carItem.Image);
            if (!IsSuccess) ModelState.AddModelError("", "Something went wrong while uplaoding the image. Try again!");
            if (!ModelState.IsValid) return View();
            carItem.Car.ImagePath = imageUrl;
            await _carRepository.CreateCarAsync(carItem.Car);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> SendUserInquiry(int id, string emailAddress)
        {
            var username = Environment.GetEnvironmentVariable("SmtpUsername");
            var carItem = await _carRepository.GetCarAsync(id);

            var mailMessage = new MailMessage()
            {
                From = new(username!),
                Subject = $"Inquiry about {carItem.Model}",
                Body = $"<div class='card' style='border-color:{carItem.Color}'> " +
                $"<p class='card-header fs-3' style='border-color:{carItem.Color}'>{carItem.Manufacturer}</p>" +
                $"<div class='card-body'>" +
                $"<div class='d-flex flex-wrap gap-2 justify-content-between align-items-start mb-2'><p style=\"font-weight: 600\">{carItem.Model}</p> <button class=\"btn btn-success p-1\">{carItem.Price}</button></div><div class=\"card-img img-top\"><img src=\"{carItem.ImagePath}\" alt='image' style=\"width: 300px; display: grid; margin: auto;\" /></div><p> Color:{carItem.Color}</p> <p>Wheel: {carItem.WheelQuantity}</p><p>Year: {carItem.Year}</p><p>Features: {carItem.Features}</p></div> " +
                $"</div>" +
                $"</div>",
                IsBodyHtml = true
            };
            EmailService.SendMessage(mailMessage, emailAddress);
            Console.WriteLine("\n\nSent\n\n");

            return RedirectToAction("GetCar", new { id });
        }



    }

}
