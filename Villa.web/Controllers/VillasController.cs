using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Villla.Application.Interfaces.CommonRepos;
using Villla.Domain.Entities;
using Villla.Infrastructure.Data;
using Villla.Infrastructure.RepositoryImplementation;
using Villla.Web.ViewModels.VillaNumberVM;

namespace Villla.Web.Controllers
{
    [Authorize]
    public class VillasController : Controller
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IWebHostEnvironment _webHostEnvironment;

        public VillasController(IUnitOfWork uow, IWebHostEnvironment webHostEnvironment)
        {
            _uow = uow;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var item = _uow.Villas.GetAll();

            return View(item);
        }
        public IActionResult Create()
        {
            return View();

        }
        [HttpPost]
        public IActionResult Create(Villla.Domain.Entities.Villa villa)
        {
            if (villa.Image != null)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(villa.Image.FileName);

                string folderPath = Path.Combine(wwwRootPath, "images", "VillaImage");

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string filePath = Path.Combine(folderPath, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    villa.Image.CopyTo(fileStream);
                }

                villa.ImageUrl = "/images/VillaImage/" + fileName;
            }
            else
            {
                villa.ImageUrl = "https://images.unsplash.com/photo-1522708323590-d24dbb6b0267?auto=format&fit=crop&w=600&q=60";
            }
            _uow.Villas.Create(villa);
            _uow.Save();
            TempData["Success"] = "Villa created successfully.";
            return RedirectToAction("Index");
        }
        public IActionResult Edit(int id)
        {
            var item = _uow.Villas.Get(v => v.Id == id);

            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }
        [HttpPost]
        public IActionResult Edit(Villla.Domain.Entities.Villa villa)
        {
            var existingVilla = _uow.Villas.Get(v => v.Id == villa.Id);

            if (existingVilla == null)
            {
                return NotFound();
            }

            string oldImagePath = existingVilla.ImageUrl;

            if (villa.Image != null)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(villa.Image.FileName);

                string folderPath = Path.Combine(wwwRootPath, "images", "VillaImage");

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string filePath = Path.Combine(folderPath, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    villa.Image.CopyTo(fileStream);
                }

                existingVilla.ImageUrl = "/images/VillaImage/" + fileName;

             
                if (!string.IsNullOrEmpty(oldImagePath))
                {
                    var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, oldImagePath.TrimStart('/'));

                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
            }

            existingVilla.Name = villa.Name;
            existingVilla.Price = villa.Price;
            existingVilla.Description = villa.Description;
            existingVilla.Occupancy =villa.Occupancy;
            existingVilla.Sqft = villa.Sqft;


            _uow.Villas.UpdateVilla(existingVilla);
            _uow.Save();

            TempData["Success"] = "Villa updated successfully";

            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        {
            var item = _uow.Villas.Get(v => v.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var item = _uow.Villas.Get(v => v.Id == id);
            if(item.ImageUrl != null)
            {
                var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, item.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }
            else {
                TempData["Error"] = "Villa image not found.";
            }

            if (item == null)
            {
                TempData["Error"] = "Villa not found.";
                return NotFound();
            }
            _uow.Villas.Delete(item);
            _uow.Save();
            TempData["Success"] = "Villa deleted successfully.";
            return RedirectToAction("Index");
        }

        public IActionResult GetDetails(int id)
        {
            var villa = _uow.Villas.Get(
                v => v.Id == id,
                include: q => q.Include(v => v.Amenities)
            );

            if (villa == null)
                return NotFound();

            return PartialView("_VillaDetailsPartial", villa);
        }

    }
}
