//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;
//using Villla.Application.Interfaces.CommonRepos;
//using Villla.Domain.Entities;
//using Villla.Infrastructure.Data;
//using Villla.Web.ViewModels;
//using Villla.Web.ViewModels.VillaNumberVM;

//namespace Villla.Web.Controllers
//{
//    [Authorize]
//    public class VillaNumberController : Controller
//    {
//        private readonly IUnitOfWork _uow;
//        public VillaNumberController(IUnitOfWork uow)
//        {
//            _uow = uow;
//        }

//        public IActionResult Index()
//        {
//            var item = _uow.VillaNumbers.GetAll(
//                include: q => q.Include(v => v.Villa)
//                ).ToList();

//            return View(item);
//        }
//        public IActionResult Create()
//        {
//            #region with ViewBag Before ViewModel
//            //var v1 = _db.Villas.ToList();
//            //ViewBag.VillaList = new SelectList(v1, "Id", "Name");
//            #endregion
//            var vm = new VillaNumberVM()
//            {
//                VillaList = _uow.Villas.GetAll().Select(u => new SelectListItem
//                {
//                    Text = u.Name,
//                    Value = u.Id.ToString()
//                }),
//                VillaNumber = new VillaNumber()
//            };

//            return View(vm);

//        }
//        [HttpPost]
//        public IActionResult Create(VillaNumberVM vmm)
//        {
//            ModelState.Remove("VillaNumber.Villa"); // Remove the validation for the navigation property
//            bool exists = _uow.VillaNumbers.GetAll()
//            .Any(v => v.Villa_Number == vmm.VillaNumber.Villa_Number);


//            if (ModelState.IsValid && !exists)
//            {
//                #region with ViewBag Before ViewModel
//                //_db.VillaNumbers.Add(villa);
//                //_db.SaveChanges();
//                //TempData["Success"] = "Villa created successfully.";
//                //return RedirectToAction("Index");
//                #endregion
//                _uow.VillaNumbers.Create(vmm.VillaNumber);
//                _uow.Save();
//                TempData["Success"] = "Villa created successfully.";
//                return RedirectToAction("Index");
//            }
//            var vm = new VillaNumberVM()
//            {
//                VillaList = _uow.Villas.GetAll().Select(u => new SelectListItem
//                {
//                    Text = u.Name,
//                    Value = u.Id.ToString()
//                }),
//                VillaNumber = new VillaNumber()
//            };
//            if (exists)
//            {
//                ModelState.AddModelError("VillaNumber.Villa_Number", "Villa Number already exists!");
//            }
//            TempData["Error"] = "Please correct the errors and try again.";
//            return View(vm);
//            #region with ViewBag Before ViewModel
//            //var v1 = _db.Villas.ToList();
//            //ViewBag.VillaList = new SelectList(v1, "Id", "Name");
//            //return View(villa);
//            #endregion
//        }
//        public IActionResult Edit(int id)
//        {
//            var v1 = _uow.VillaNumbers.Get(u => u.Villa_Number == id);


//            if (v1 == null)
//            {
//                return NotFound();
//            }
//            var vm=new VillaNumberVM()
//            {
//                VillaList= _uow.Villas.GetAll().Select(u => new SelectListItem
//                {
//                    Text = u.Name,
//                    Value = u.Id.ToString()
//                }),
//                VillaNumber =  v1

//            };
//            return View(vm);
//            #region with ViewBag Before ViewModel
//            //var v2 = _db.Villas.ToList();
//            //ViewBag.VillaList = new SelectList(v2, "Id", "Name");
//            //return View(v1);
//            #endregion
//        }
//        [HttpPost]
//        public IActionResult Edit(VillaNumberVM vm)
//        {
//            ModelState.Remove("VillaNumber.Villa");
//            #region with ViewBag Before ViewModel
//            //var existingVilla = _db.VillaNumbers.Find(villa.Villa_Number);

//            //if (existingVilla == null)
//            //{
//            //    var v1=_db.Villas.ToList();
//            //    ViewBag.VillaList = new SelectList(v1, "Id", "Name");
//            //    TempData["Error"] = "Villa not found.";
//            //    return View(villa);
//            //}

//            ////_db.VillaNumbers.Update(villa);

//            //existingVilla.SpecialDetails = villa.SpecialDetails;
//            //existingVilla.VillaId = villa.VillaId;

//            //_db.SaveChanges();

//            //TempData["Success"] = "Villa updated successfully.";
//            //return RedirectToAction("Index");
//            #endregion
//            if (!ModelState.IsValid)
//            {
//                vm.VillaList = _uow.Villas.GetAll().Select(u => new SelectListItem
//                {
//                    Text = u.Name,
//                    Value = u.Id.ToString()
//                });

//                return View(vm);
//            }

//            var existingVilla = _uow.VillaNumbers.Get(u => u.Villa_Number == vm.VillaNumber.Villa_Number);

//            if (existingVilla == null)
//            {
//                return NotFound();
//            }


//            existingVilla.VillaId = vm.VillaNumber.VillaId;
//            existingVilla.SpecialDetails = vm.VillaNumber.SpecialDetails;

//            //_uow.VillaNumbers.UpdateVillaNumber(existingVilla);
//            _uow.Save();

//            TempData["Success"] = "Updated successfully";
//            return RedirectToAction("Index");
//        }
//        public IActionResult Delete(int id)
//        {

//            var v1 = _uow.VillaNumbers.Get(u => u.Villa_Number == id);


//            if (v1 == null)
//            {
//                return NotFound();
//            }
//            var vm = new VillaNumberVM()
//            {
//                VillaList = _uow.Villas.GetAll().Select(u => new SelectListItem
//                {
//                    Text = u.Name,
//                    Value = u.Id.ToString()
//                }),
//                VillaNumber = v1

//            };
//            return View(vm);
//            #region with ViewBag Before ViewModel
//            //var item = _db.VillaNumbers.Find(id);
//            //if (item == null)
//            //{
//            //    return NotFound();
//            //}
//            //return View(item);
//            #endregion
//        }
//        [HttpPost, ActionName("Delete")]
//        public IActionResult DeleteConfirmed(int id)
//        {
//            ModelState.Remove("VillaNumber.Villa");
//            var item = _uow.VillaNumbers.Get(u => u.Villa_Number == id);
//            if (item == null)
//            {
//                TempData["Error"] = "Villa not found.";
//                return NotFound();
//            }
//            _uow.VillaNumbers.Delete(item);
//            _uow.Save();
//            TempData["Success"] = "Villa deleted successfully.";
//            return RedirectToAction(nameof(Index));

//            #region with ViewBag Before ViewModel
//            //var item = _db.VillaNumbers.Find(id);
//            //if (item == null)
//            //{
//            //    TempData["Error"] = "Villa not found.";
//            //    return NotFound();
//            //}
//            //_db.VillaNumbers.Remove(item);
//            //_db.SaveChanges();
//            //TempData["Success"] = "Villa deleted successfully.";
//            //return RedirectToAction("Index");
//            #endregion


//        }
//    }
//}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Villla.Application.Services.Interface;
using Villla.Application.Dtos;

namespace Villla.Web.Controllers
{
    [Authorize]
    public class VillaNumberController : Controller
    {
        private readonly IVillaNumberService _villaNumberService;

        public VillaNumberController(IVillaNumberService villaNumberService)
        {
            _villaNumberService = villaNumberService;
        }

        // ✅ Index
        public async Task<IActionResult> Index()
        {
            var data = await _villaNumberService.GetAllAsync();
            return View(data);
        }

        // ✅ Create (GET)
        public async Task<IActionResult> Create()
        {
            var dto = new VillaNumberDto
            {
                VillaList = await _villaNumberService.GetVillaListAsync()
            };

            return View(dto);
        }

        // ✅ Create (POST)
        [HttpPost]
        public async Task<IActionResult> Create(VillaNumberDto dto)
        {
            if (!ModelState.IsValid)
            {
                dto.VillaList = await _villaNumberService.GetVillaListAsync();
                return View(dto);
            }

            var created = await _villaNumberService.CreateAsync(dto);

            if (!created)
            {
                ModelState.AddModelError("VillaNumber", "Villa Number already exists!");
                dto.VillaList = await _villaNumberService.GetVillaListAsync();
                return View(dto);
            }

            TempData["Success"] = "Villa created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // ✅ Edit (GET)
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _villaNumberService.GetByIdAsync(id);

            if (dto == null) return NotFound();

            dto.VillaList = await _villaNumberService.GetVillaListAsync();

            return View(dto);
        }

        // ✅ Edit (POST)
        [HttpPost]
        public async Task<IActionResult> Edit(VillaNumberDto dto)
        {
            if (!ModelState.IsValid)
            {
                dto.VillaList = await _villaNumberService.GetVillaListAsync();
                return View(dto);
            }

            await _villaNumberService.UpdateAsync(dto);

            TempData["Success"] = "Updated successfully";
            return RedirectToAction(nameof(Index));
        }

        // ✅ Delete (GET)
        public async Task<IActionResult> Delete(int id)
        {
            var dto = await _villaNumberService.GetByIdAsync(id);

            if (dto == null) return NotFound();

            return View(dto);
        }

        // ✅ Delete (POST)
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _villaNumberService.DeleteAsync(id);

            TempData["Success"] = "Villa deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}