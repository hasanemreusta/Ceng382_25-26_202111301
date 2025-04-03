using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Week5.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Week5.Pages
{
    public class IndexModel : PageModel
    {
        private static readonly List<ClassInformationModel> _classList = GenerateSampleData();
        private static int _nextId = 101;

        [BindProperty]
        public ClassInformationModel NewClass { get; set; } = new ClassInformationModel();

        [BindProperty(SupportsGet = true)]
        public string FilterClassName { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public int? FilterStudentCount { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }
        public int TotalClassCount => _classList.Count;

        public List<ClassInformationTable> DisplayClasses { get; set; } = new List<ClassInformationTable>();

// Creating by ChatGPT,OpenAI,04.04.2025 
 // PROMPT USED: "Create a Razor Pages pagination system with 100 sample records using C#"
// HUMAN EDITS: Filter implementation, model binding adjustments-->
        private static List<ClassInformationModel> GenerateSampleData()
        {
            var rnd = new Random();
            return Enumerable.Range(1, 100).Select(i => new ClassInformationModel
            {
                Id = i,
                ClassName = $"Class {i}",
                StudentCount = rnd.Next(1, 101),
                Description = $"Description for Class {i}"
            }).ToList();
        }

// Prompt: "Implement LINQ pagination with filters", ChatGPT, OpenAI
        public void OnGet()
        {
            var query = _classList.AsQueryable();

            if (!string.IsNullOrWhiteSpace(FilterClassName))
            {
                query = query.Where(c => c.ClassName.Contains(FilterClassName, StringComparison.OrdinalIgnoreCase));
            }
            if (FilterStudentCount.HasValue)
            {
                query = query.Where(c => c.StudentCount == FilterStudentCount.Value);
            }

            int totalItems = query.Count();
            TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);
            PageNumber = Math.Clamp(PageNumber, 1, Math.Max(1, TotalPages));

            DisplayClasses = query
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .Select(c => new ClassInformationTable
                {
                    Id = c.Id,
                    ClassName = c.ClassName,
                    StudentCount = c.StudentCount,
                    Description = c.Description
                }).ToList();
        }
    }
}