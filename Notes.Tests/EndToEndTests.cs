using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Playwright.NUnit;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.NetworkInformation;

namespace Notes.Tests
{
    public class EndToEndTests
    {
        [Test]
        public async Task TestCreateAndUpdateNote()
        {
            using var playwright = await Playwright.CreateAsync();
            var chrome = playwright.Chromium;
            var browser = await chrome.LaunchAsync(new() { Headless = false });
            var page = await browser.NewPageAsync();
            await page.GotoAsync("https://localhost:7209/");

            // Create a new note
            await page.FillAsync("#title", "Test Title");
            await page.FillAsync("#content", "Test Content");
            await page.ClickAsync(".btn-success");  // Click on the "Create" button

            // Wait for the note to appear in the table
            await page.WaitForSelectorAsync("td:has-text('Test Title')");

            // Search for the note by its title
            await page.FillAsync(".form-control[placeholder='Search by Title or Content']", "Test Title");

            // Wait for the dropdown-content to display the note
            await page.WaitForSelectorAsync("ul.dropdown-content li:has-text('Test Title')");

            // Click on the searched note from the dropdown
            await page.ClickAsync("ul.dropdown-content li:has-text('Test Title')");

            // Click on the last "Delete Note" button on the page
            await page.ClickAsync("button:has-text('Delete Note')");

            // Verify the note is deleted (waiting for it to disappear from the table)
            await page.WaitForSelectorAsync("td:has-text('Test Title')", new() { State = WaitForSelectorState.Detached });

            await browser.CloseAsync();
        }

        [Test]
        public async Task TestEditNote()
        {
            using var playwright = await Playwright.CreateAsync();
            var chrome = playwright.Chromium;
            var browser = await chrome.LaunchAsync(new() { Headless = false });
            var page = await browser.NewPageAsync();
            await page.GotoAsync("https://localhost:7209/");

            // Create a new note for the purpose of this test
            await page.FillAsync("#title", "Edit Test Title");
            await page.FillAsync("#content", "Edit Test Content");
            await page.ClickAsync(".btn-success");  // Click on the "Create" button

            // Wait for the note to appear in the table
            await page.WaitForSelectorAsync("td:has-text('Edit Test Title')");

            // Click on the "Edit Note" button
            await page.ClickAsync("td:has-text('Edit Test Title') + td + td button:has-text('Edit Note')");

            // Fill in the updated details
            await page.FillAsync("td input[id='editTitle']", "Updated Test Title");
            await page.FillAsync("td textarea[id='editContent']", "Updated Test Content");

            // Click on "Apply Changes" button
            await page.ClickAsync("td:has-text('Updated Test Title') + td + td button:has-text('Apply Changes')");

            // Validate that the changes have been applied
            await page.WaitForSelectorAsync("td:has-text('Updated Test Title')");

            // Clean up by deleting the note we just edited
            await page.ClickAsync("td:has-text('Updated Test Title') + td + td button:has-text('Delete Note')");

            // Verify the note is deleted
            await page.WaitForSelectorAsync("td:has-text('Updated Test Title')", new() { State = WaitForSelectorState.Detached });

            await browser.CloseAsync();
        }
    }
}
