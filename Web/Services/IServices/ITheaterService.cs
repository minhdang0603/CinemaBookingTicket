using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Models.DTOs.Request;
using Web.Models.DTOs.Response;

namespace Web.Services.IServices;

public interface ITheaterService
{
    Task<T> GetTheaterByIdAsync<T>(int id);
    Task<T> CreateTheaterAsync<T>(TheaterCreateDTO theater, string token = null);
    Task<T> UpdateTheaterAsync<T>(int id, TheaterUpdateDTO theater, string token = null);
    Task<T> DeleteTheaterAsync<T>(int id, string token = null);
    Task<T> GetAllTheatersAsync<T>();
}
