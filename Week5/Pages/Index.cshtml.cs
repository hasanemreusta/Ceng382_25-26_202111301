using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Week5.Data;
using Week5.Models;
using Week5.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Week5.Pages
{
    public class IndexModel : PageModel
    {
        private readonly SchoolDbContext _context;

        public IndexModel(SchoolDbContext context)
        {
            _context = context;
        }

        public IList<Class>? ClassList { get; set; }

        [BindProperty]
        public Class NewClass { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? FilterClassName { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }

        public List<Class> FilteredTableData { get; set; } = new();

        [BindProperty]
        public bool IsEditMode { get; set; }

        [BindProperty]
        public int EditId { get; set; }

        [BindProperty]
        public string? SelectedExportColumns { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var sessionUsername = HttpContext.Session.GetString("username");
            var sessionToken = HttpContext.Session.GetString("token");
            var sessionId = HttpContext.Session.GetString("session_id");

            var cookieUsername = Request.Cookies["username"];
            var cookieToken = Request.Cookies["token"];
            var cookieSessionId = Request.Cookies["session_id"];

            if (string.IsNullOrEmpty(sessionUsername) ||
                string.IsNullOrEmpty(sessionToken) ||
                string.IsNullOrEmpty(sessionId) ||
                cookieUsername != sessionUsername ||
                cookieToken != sessionToken ||
                cookieSessionId != sessionId)
            {
                TempData["LoginError"] = "You must be logged in to view this page.";
                return RedirectToPage("/Login");
            }

            var query = _context.Classes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(FilterClassName))
            {
                query = query.Where(c => c.Name.Contains(FilterClassName));
            }

            int totalRecords = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(totalRecords / (double)PageSize);

            FilteredTableData = await query
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAddAsync()
        {
            if (!ModelState.IsValid) return Page();

            _context.Classes.Add(NewClass);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var item = await _context.Classes.FindAsync(id);
            if (item != null)
            {
                _context.Classes.Remove(item);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditAsync(int id)
        {
            var item = await _context.Classes.FindAsync(id);
            if (item != null)
            {
                NewClass = new Class
                {
                    Name = item.Name,
                    PersonCount = item.PersonCount,
                    Description = item.Description,
                    IsActive = item.IsActive
                };
                EditId = item.Id;
                IsEditMode = true;
            }

            await OnGetAsync(); // Refresh table
            return Page();
        }

        public async Task<IActionResult> OnPostUpdateAsync()
        {
            if (!ModelState.IsValid) return Page();

            var existing = await _context.Classes.FindAsync(EditId);
            if (existing != null)
            {
                existing.Name = NewClass.Name;
                existing.PersonCount = NewClass.PersonCount;
                existing.Description = NewClass.Description;
                existing.IsActive = NewClass.IsActive;

                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostExportAsync()
        {
            var query = _context.Classes.AsQueryable();
            if (!string.IsNullOrWhiteSpace(FilterClassName))
            {
                query = query.Where(c => c.Name.Contains(FilterClassName));
            }

            var exportData = await query.ToListAsync();

            string jsonResult;
            var selectedColumnsList = string.IsNullOrWhiteSpace(SelectedExportColumns)
                ? new List<string>()
                : SelectedExportColumns.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                        .Select(s => s.Trim())
                                        .ToList();

            if (selectedColumnsList.Any())
            {
                jsonResult = Utils.Instance.ExportToJsonSelected(exportData, selectedColumnsList);
            }
            else
            {
                jsonResult = Utils.Instance.ExportToJson(exportData);
            }

            var fileBytes = System.Text.Encoding.UTF8.GetBytes(jsonResult);
            var fileName = $"ExportedData_{DateTime.Now:yyyyMMdd_HHmmss}.json";

            return File(fileBytes, "application/json", fileName);
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete("username");
            Response.Cookies.Delete("token");
            Response.Cookies.Delete("session_id");

            return RedirectToPage("/Login");
        }
    }
}
