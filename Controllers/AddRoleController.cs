using Glowry.Data;
using Glowry.View_Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Glowry.Controllers
{
    public class AddRoleController : Controller
    {
        private RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        public AddRoleController(RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _roleManager = roleManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult AddRole()
        {
            return View();
        }
        [HttpPost]
        public async Task< IActionResult> AddRole(AddRoleViewModel addrole)
        {
            if (ModelState.IsValid) 
            { 
                IdentityRole role=new IdentityRole();
                role.Name=addrole.RoleName;
                IdentityResult result=   await _roleManager.CreateAsync(role);
                if (result.Succeeded == true)
                {
                    ViewBag.sucess = true;
                    return View("AddRole");
                }
                else 
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }
            }
            return View(addrole);
        }

        [HttpGet]
        public async Task<IActionResult> allroles()
        {
            var allroles = _context.Roles;
            
            return View(await allroles.ToListAsync());
        }

    }
}
