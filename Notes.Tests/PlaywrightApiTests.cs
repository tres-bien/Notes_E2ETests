using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Notes_WebApp.Shared;
using System.Text.Json;

namespace Notes.Tests
{
    internal class PlaywrightApiTests
    {
        private IPlaywright _playwright;
        private IAPIRequestContext _requestContext;

        [SetUp]
        public async Task InitializeTests()
        {
            _playwright = await Playwright.CreateAsync();
            _requestContext = await _playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions()
            {
                BaseURL = "https://localhost:7209/",
                IgnoreHTTPSErrors = true,
            });
        }

        [Test]
        [Order(1)]
        public async Task GetAllNotesReturnsData()
        {
            var responce = await _requestContext.GetAsync("api/Notes");
            var data = await responce.JsonAsync();

            Assert.IsNotNull(data);
        }

        [Test]
        [Order(2)]
        public async Task GetSingleNoteReturnsData()
        {
            var responce = await _requestContext.GetAsync("api/Notes/1");

            var data = await responce.JsonAsync();

            Assert.IsNotNull(data);
        }

        [Test]
        [Order(3)]
        public async Task CanCreateNewNote()
        {
            var newNoteContent = new Note
            {
                Title = "E2E Test",
                Content = "This is an E2E test content",
                Date = DateTime.Now,
                IsExpanded = false,
                IsEditing = false
            };

            var responce = await _requestContext.PostAsync("api/Notes", new APIRequestContextOptions()
            {
                DataObject = newNoteContent
            });

            Assert.That(responce.Status, Is.EqualTo(200));
        }

        [Test]
        [Order(4)]
        public async Task CanUpdateNoteTitle()
        {
            var get = await _requestContext.GetAsync("api/Notes");
            var jsonString = await get.TextAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            List<Note> notes = JsonSerializer.Deserialize<List<Note>>(jsonString, options)!;
            string targetTitle = "E2E Test";
            Note targetNote = notes.FirstOrDefault(n => n.Title == targetTitle)!;

            if (targetNote!.Title != targetTitle)
            {
                Assert.Fail("Note not found");
            }

            var updatedNote = new Note
            {
                Title = "Updated Title",
                Content = "Updated Content",
                Date = DateTime.UtcNow,
                IsExpanded = true,
                IsEditing = false
            };

            var responce = await _requestContext.PutAsync($"api/Notes/{targetNote.Id}", new APIRequestContextOptions()
            {
                DataObject = updatedNote
            });

            Assert.Positive(responce.Status);
        }


        [Test]
        [Order(5)]
        public async Task CanDeleteUpdatedNote()
        {
            var get = await _requestContext.GetAsync("api/Notes");
            var jsonString = await get.TextAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            List<Note> notes = JsonSerializer.Deserialize<List<Note>>(jsonString, options)!;
            string targetTitle = "Updated Title";
            Note targetNote = notes.FirstOrDefault(n => n.Title == targetTitle)!;

            if (targetNote!.Title != targetTitle)
            {
                Assert.Fail("Note not found");
            }
            Assert.That(targetNote.Title, Is.EqualTo(targetTitle));
            var responce = await _requestContext.DeleteAsync($"api/Notes/{targetNote.Id}");
            Assert.Positive(responce.Status);
        }
    }
}
