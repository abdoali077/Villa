using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Villla.Application.Interfaces.CommonRepos;
using Villla.Application.Utility;
using Villla.Domain.Entities;
using Villla.Web.ViewModels;
using Villla.Web.ViewModels.VillaNumberVM;

namespace Villla.Web.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]
    public class AmenityController : Controller
    {
        private readonly IUnitOfWork _uow;
        public AmenityController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public IActionResult Index()
        {
            var amenities = _uow.Amenities.GetAll(
                include: q => q.Include(v => v.Villa)
            ).ToList();

            var result = amenities.Select(a => new AmenityVM
            {
                Amenity = a
            }).ToList();

            return View(result);
        }
        public IActionResult Create()
        {
            var vm = new AmenityVM
            {
                VillaList = _uow.Villas.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })
            };

            return View(vm);
        }
        
        [HttpPost]
        public IActionResult Create(AmenityVM vm)
        {
            if (ModelState.IsValid)
            {
                _uow.Amenities.Create(vm.Amenity);
                _uow.Save();

                TempData["Success"] = "Amenity created successfully.";

                return RedirectToAction("Index");
            }

          
            vm.VillaList = _uow.Villas.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });

            TempData["Error"] = "Please correct the errors and try again.";

            return View(vm);
        }
        public IActionResult Edit(int id)
        {
            var v1 = _uow.Amenities.Get(u => u.Id == id);


            if (v1 == null)
            {
                return NotFound();
            }
            var vm = new AmenityVM()
            {
                VillaList = _uow.Villas.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Amenity = v1

            };
            return View(vm);
          
        }
        [HttpPost]
        public IActionResult Edit(AmenityVM vm)
        {
            //ModelState.Remove("VillaNumber.Villa");
          
            if (!ModelState.IsValid)
            {
                vm.VillaList = _uow.Villas.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });

                return View(vm);
            }

            var existingVilla = _uow.Amenities.Get(u => u.Id == vm.Amenity.Id);

            if (existingVilla == null)
            {
                return NotFound();
            }


            existingVilla.VillaId = vm.Amenity.VillaId;
            existingVilla.Name=vm.Amenity.Name;
            existingVilla.Description = vm.Amenity.Description;
            _uow.Amenities.UpdateAmenity(existingVilla);

            _uow.Save();

            TempData["Success"] = "Updated successfully";
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        {

            var v1 = _uow.Amenities.Get(u => u.Id == id);


            if (v1 == null)
            {
                return NotFound();
            }


            AmenityVM vm = new AmenityVM()
            {
                Amenity = v1
            };
            return View(vm);
           
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            //ModelState.Remove("VillaNumber.Villa");
            var item = _uow.Amenities.Get(u => u.Id == id);
            if (item == null)
            {
                TempData["Error"] = "Amenity not found.";
                return NotFound();
            }
            
            _uow.Amenities.Delete(item);
            _uow.Save();
            TempData["Success"] = "Amenity deleted successfully.";
            return RedirectToAction(nameof(Index));

        }
    }
}