using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using assignment2.Data;
using assignment2.Models;
using assignment2.Models.ViewModels;

namespace assignment2.Controllers
{
    public class StudentsController : Controller
    {
        private readonly SchoolCommunityContext _context;

        public StudentsController(SchoolCommunityContext context)
        {
            _context = context;
        }

        // GET: 
        public async Task<IActionResult> Index(int? id)
        {
            var viewModel = new CommunityViewModel();
            viewModel.Students = await _context.Students
                  .Include(c => c.Membership)
                  .ThenInclude(s => s.Community)
                  .AsNoTracking()
                  .OrderBy(s => s.FirstName)
                  .ToListAsync();



            if (id != null)
            {
                ViewData["StudentId"] = id;
                viewModel.CommunityMemberships = viewModel.Students.Where(
                    c => c.Id == id).Single().Membership;
            }

            return View(viewModel);
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LastName,FirstName,EnrollmentDate")] Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LastName,FirstName,EnrollmentDate")] Student student)
        {
            if (id != student.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        public async Task<IActionResult> EditMemberships(int? id)
        {
            var viewModel = new StudentMembershipViewModel();

            viewModel.Student = await _context.Students
                  .Include(Student => Student.Membership)
                  .ThenInclude(Membership => Membership.Community)
                  .AsNoTracking()
                  .FirstOrDefaultAsync(Student => Student.Id == id);

            var allCommunities = await _context.Communities
                              .Include(c => c.Membership)
                              .ThenInclude(m => m.Student)
                              .AsNoTracking()
                              .OrderBy(c => c.Title)
                              .ToListAsync();
            var Memberships = new List<CommunityMembershipViewModel>();
            foreach (Community c in allCommunities)
            {
                var Membership = new CommunityMembershipViewModel();
                Membership.CommunityId = c.Id;
                Membership.Title = c.Title;
                bool found = false;
                foreach (var membership in c.Membership)
                {
                    if (membership.StudentId == id)
                    {
                        found = true;
                        break;
                    }
                }
                Membership.IsMember = found;
                Memberships.Add(Membership);
            }

            viewModel.Memberships = Memberships;
            return View(viewModel);
        }

        public async Task<IActionResult> addMembership(int? studentId, string communityId)
        {
            var viewModel = new StudentMembershipViewModel();

            if (studentId == null || communityId == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var membership = new CommunityMembership();
                    membership.CommunityId = communityId;
                    membership.StudentId = studentId.Value;
                    _context.CommunityMemberships.Add(membership);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(studentId.Value))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(EditMemberships), new { id = studentId });
            }
            return View(studentId);
        }

        public async Task<IActionResult> deleteMembership(int? studentId, string communityId)
        {
            if (studentId == null || communityId == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var membership = new CommunityMembership();
                    membership.CommunityId = communityId;
                    membership.StudentId = studentId.Value;
                    _context.CommunityMemberships.Remove(membership);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(studentId.Value))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(EditMemberships), new { id = studentId });
            }
            return View(studentId);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.Id == id);
        }
    }
}
