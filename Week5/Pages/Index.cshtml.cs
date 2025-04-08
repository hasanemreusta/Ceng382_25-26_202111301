using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Week5.Models;
using System.Collections.Generic;
using System.Linq;

namespace Week5.Pages
{
            // Created by ChatGPT, OpenAI, April 8, 2025;
            // using prompt: "Create a C# Razor Page with a form to add, edit, and delete class information.
            // The class should have properties like ClassName, StudentCount, and Description. 
            // Include validation for the properties. Implement filtering and pagination for the list of classes displayed on the page."
    public class IndexModel : PageModel
    {
        private static List<ClassInformationModel> ClassList = new List<ClassInformationModel>();
        private static int NextId = 1;

        // For the input form
        [BindProperty]
        public ClassInformationModel NewClass { get; set; } = new ClassInformationModel();

        // Filtering parameter
        [BindProperty(SupportsGet = true)]
        public string? FilterClassName { get; set; }

        // Pagination parameters
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }

        // Table data to be displayed
        public List<ClassInformationTable> FilteredTableData { get; set; } = new();

        [BindProperty]
        public bool IsEditMode { get; set; }

        [BindProperty]
        public int EditId { get; set; }

        public void OnGet()
        {

            
            if (!ClassList.Any())
            {
                
                for (int i = 1; i < 100; i++)
                {
                    ClassList.Add(new ClassInformationModel
                    {
                        Id = NextId++,
                        ClassName = $"Class {i}",
                        StudentCount = 20 + (i % 5),
                        Description = $"Description for class {i}"
                    });
                }
            }

            // Filtreleme
            var query = ClassList.AsQueryable();
            if (!string.IsNullOrWhiteSpace(FilterClassName))
            {
                query = query.Where(c => c.ClassName.Contains(FilterClassName));
            }

            // Toplam sayfa sayısı
            int totalRecords = query.Count();
            TotalPages = (int)System.Math.Ceiling(totalRecords / (double)PageSize);

            // Sayfalama
            var paginatedData = query
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            // Convert to the table to be displayed
            FilteredTableData = paginatedData
                .Select(c => new ClassInformationTable
                {
                    Id = c.Id,
                    ClassName = c.ClassName,
                    StudentCount = c.StudentCount,
                    Description = c.Description
                })
                .ToList();
        }

        public IActionResult OnPostAdd()
        {
            if (!ModelState.IsValid) return Page();

            NewClass.Id = NextId++;
            ClassList.Add(NewClass);
            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int id)
        {
            var item = ClassList.FirstOrDefault(c => c.Id == id);
            if (item != null) ClassList.Remove(item);
            return RedirectToPage();
        }

        public IActionResult OnPostEdit(int id)
        {
            var item = ClassList.FirstOrDefault(c => c.Id == id);
            if (item != null)
            {
                NewClass = new ClassInformationModel
                {
                    ClassName = item.ClassName,
                    StudentCount = item.StudentCount,
                    Description = item.Description
                };
                EditId = item.Id;
                IsEditMode = true;
            }
            return Page();
        }

        public IActionResult OnPostUpdate()
        {
            if (!ModelState.IsValid) return Page();

            var existing = ClassList.FirstOrDefault(c => c.Id == EditId);
            if (existing != null)
            {
                existing.ClassName = NewClass.ClassName;
                existing.StudentCount = NewClass.StudentCount;
                existing.Description = NewClass.Description;
            }

            return RedirectToPage();
        }
    }
}
