using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Week5.Models;
using Week5.Utilities; // Using the Utils class from the Helpers/Utilities folder
using System;
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

        // For JSON export: Selected columns from the table header.
        // The selected columns will be received as a comma-separated string and then split into a list.
        [BindProperty]
        public string? SelectedExportColumns { get; set; }

        public void OnGet()
        {
            if (!ClassList.Any())
            {
                // Creating 100 sample records
                for (int i = 1; i <= 100; i++)
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

            // Filtering
            var query = ClassList.AsQueryable();
            if (!string.IsNullOrWhiteSpace(FilterClassName))
            {
                query = query.Where(c => c.ClassName.Contains(FilterClassName));
            }

            // Calculating total pages
            int totalRecords = query.Count();
            TotalPages = (int)Math.Ceiling(totalRecords / (double)PageSize);

            // Pagination logic
            var paginatedData = query
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            // Converting to data for the table display
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

        
        // This method creating by ChatGPT, OpenAI, April 10, 2025;
        // Prompt used: 
        // "Implement a Razor Pages backend export feature that allows exporting either the full list or 
        // a filtered list of class data to JSON format. The user should be able to select which columns 
        // to include in the export. The exported JSON should be downloaded as a .json file with a dynamic filename 
        // based on the current timestamp."
        public IActionResult OnPostExport()
        {
        // Exporting the filtered data based on the current filter applied in the table.
        var query = ClassList.AsQueryable();
        if (!string.IsNullOrWhiteSpace(FilterClassName))
        {
            query = query.Where(c => c.ClassName.Contains(FilterClassName));
        }
        var exportData = query.ToList();

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

        // Convert JSON string to byte array for file download
        var fileBytes = System.Text.Encoding.UTF8.GetBytes(jsonResult);
        var fileName = $"ExportedData_{DateTime.Now:yyyyMMdd_HHmmss}.json";

        
        return File(fileBytes, "application/json", fileName);
        }

    }
}
