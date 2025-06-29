using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Utility;
using Web.Models;
using Web.Models.DTOs.Request;
using Web.Models.DTOs.Response;
using Web.Services.IServices;

namespace Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = Constant.Role_Admin)]
public class GenreController : Controller
{
    private readonly IGenreService _genreService;
    private readonly ILogger<GenreController> _logger;

    public GenreController(IGenreService genreService, ILogger<GenreController> logger)
    {
        _genreService = genreService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var genreResponse = await _genreService.GetAllGenresAsync<APIResponse>();
        if (genreResponse == null || !genreResponse.IsSuccess)
        {
            _logger.LogError("Failed to load Genre from API");
            TempData["error"] = genreResponse?.ErrorMessages?.FirstOrDefault() ?? "Unable to load screen list.";
            return View(new List<GenreDTO>());
        }
        // Deserialize the response data into a list of ScreenDTO
        var genres = JsonConvert.DeserializeObject<List<GenreDTO>>(Convert.ToString(genreResponse.Result));
        return View(genres);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(GenreCreateDTO model)
    {
        _logger.LogInformation("Received screen creation request: {@model}", model);

        if (!ModelState.IsValid)
        {
            // Log validation errors
            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    _logger.LogWarning("Validation error for {Key}: {Error}",
                        state.Key, error.ErrorMessage);
                }
            }
            // Nếu ModelState không hợp lệ, trả về view với model và thông báo lỗi
            return View(model);
        }
        var token = HttpContext.Session.GetString(Constant.SessionToken);
        var response = await _genreService.CreateGenreAsync<APIResponse>(model, token);
        if (response == null || !response.IsSuccess)
        {
            _logger.LogError("Failed to create genre: {Error}", response?.ErrorMessages?.FirstOrDefault());
            TempData["error"] = response?.ErrorMessages?.FirstOrDefault() ?? "Unable to create genre.";
            return View(model);
        }
        _logger.LogInformation("Genre created successfully: {@model}", model);
        TempData["success"] = "Genre created successfully.";
        return RedirectToAction(nameof(Index));
    }
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var response = await _genreService.GetGenreByIdAsync<APIResponse>(id);
        if (response == null || !response.IsSuccess)
        {
            _logger.LogError("Failed to load genre for editing: {Error}", response?.ErrorMessages?.FirstOrDefault());
            TempData["error"] = response?.ErrorMessages?.FirstOrDefault() ?? "Unable to load genre for editing.";
            return RedirectToAction(nameof(Index));
        }
        // Deserialize the response data into a GenreUpdateDTO
        var genre = JsonConvert.DeserializeObject<GenreUpdateDTO>(Convert.ToString(response.Result)!);
        if (genre == null)
        {
            _logger.LogError("Failed to deserialize genre data for editing.");
            TempData["error"] = "Unable to load genre for editing.";
            return RedirectToAction(nameof(Index));
        }
        _logger.LogInformation("Loaded genre for editing: {@genre}", genre);
        return View(genre);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(GenreUpdateDTO model)
    {
        _logger.LogInformation("Received genre update request: {@model}", model);

        if (!ModelState.IsValid)
        {
            // Log validation errors
            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    _logger.LogWarning("Validation error for {Key}: {Error}",
                        state.Key, error.ErrorMessage);
                }
            }
            // Nếu ModelState không hợp lệ, trả về view với model và thông báo lỗi
            return View(model);
        }
        var token = HttpContext.Session.GetString(Constant.SessionToken);
        var response = await _genreService.UpdateGenreAsync<APIResponse>(model, token);
        if (response == null || !response.IsSuccess)
        {
            _logger.LogError("Failed to update genre: {Error}", response?.ErrorMessages?.FirstOrDefault());
            TempData["error"] = response?.ErrorMessages?.FirstOrDefault() ?? "Unable to update genre.";
            return View(model);
        }
        _logger.LogInformation("Genre updated successfully: {@model}", model);
        TempData["success"] = "Genre updated successfully.";
        return RedirectToAction(nameof(Index));
    }
    [HttpDelete]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("Received request to delete genre with ID: {Id}", id);

        var token = HttpContext.Session.GetString(Constant.SessionToken);
        var response = await _genreService.DeleteGenreAsync<APIResponse>(id, token);
        if (response == null || !response.IsSuccess)
        {
            _logger.LogError("Failed to delete genre with ID {Id}: {Error}", id, response?.ErrorMessages?.FirstOrDefault());
            TempData["error"] = response?.ErrorMessages?.FirstOrDefault() ?? "Unable to delete genre.";
            return RedirectToAction(nameof(Index));
        }
        _logger.LogInformation("Genre with ID {Id} deleted successfully", id);
        TempData["success"] = "Genre deleted successfully.";
        return RedirectToAction(nameof(Index));
    }

}