using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Models.DTOs.Request;
using Web.Models.DTOs.Response;
namespace Web.Services.IServices
{
    public interface IProvinceService
    {
        Task<T> GetAllProvincesAsync<T>();
    }
}