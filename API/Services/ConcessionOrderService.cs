using API.Data.Models;
using API.DTOs.Request;
using API.DTOs.Response;
using API.Exceptions;
using API.Repositories.IRepositories;
using API.Services.IServices;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Utility;

namespace API.Services;

public class ConcessionOrderService : IConcessionOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<ConcessionOrderService> _logger;
    private readonly UserManager<ApplicationUser> _userManager;

    public ConcessionOrderService(IUnitOfWork unitOfWork,
                                IMapper mapper,
                                IHttpContextAccessor httpContextAccessor,
                                ILogger<ConcessionOrderService> logger,
                                UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _userManager = userManager;
    }

    public async Task<ConcessionOrderDTO> CreateConcessionOrderAsync(ConcessionOrderCreateDTO concessionOrderCreateDTO)
    {
        // Validate booking exists
        if (concessionOrderCreateDTO.BookingId.HasValue)
        {
            var booking = await _unitOfWork.Booking.GetAsync(b => b.Id == concessionOrderCreateDTO.BookingId);
            if (booking == null)
            {
                throw new AppException(ErrorCodes.EntityNotFound("Booking", concessionOrderCreateDTO.BookingId.Value));
            }
        }

        // Validate concession items exist
        foreach (var detail in concessionOrderCreateDTO.ConcessionOrderDetails)
        {
            var concession = await _unitOfWork.Concession.GetAsync(c => c.Id == detail.ConcessionId);
            if (concession == null)
            {
                throw new AppException(ErrorCodes.EntityNotFound("Concession", detail.ConcessionId));
            }
        }

        // Create concession order
        var concessionOrder = new ConcessionOrder
        {
            BookingId = concessionOrderCreateDTO.BookingId,
            OrderDate = DateTime.UtcNow,
            TotalAmount = concessionOrderCreateDTO.ConcessionOrderDetails.Sum(d => d.Quantity * d.UnitPrice),
            OrderStatus = Constant.Payment_Status_Pending,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            LastUpdatedAt = DateTime.UtcNow,
            ConcessionOrderDetails = new List<ConcessionOrderDetail>()
        };

        // Add concession order details
        foreach (var detail in concessionOrderCreateDTO.ConcessionOrderDetails)
        {
            var concessionOrderDetail = new ConcessionOrderDetail
            {
                ConcessionId = detail.ConcessionId,
                Quantity = detail.Quantity,
                UnitPrice = detail.UnitPrice,
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow
            };

            concessionOrder.ConcessionOrderDetails.Add(concessionOrderDetail);
        }

        // Add concession order to repository
        await _unitOfWork.ConcessionOrder.CreateAsync(concessionOrder);
        await _unitOfWork.SaveAsync();

        // Map to DTO and return
        var result = _mapper.Map<ConcessionOrderDTO>(concessionOrder);
        return result;
    }

    public async Task DeleteConcessionOrderAsync(int concessionOrderId)
    {
        var concessionOrder = await _unitOfWork.ConcessionOrder.GetAsync(
            co => co.Id == concessionOrderId,
            includeProperties: "ConcessionOrderDetails");

        if (concessionOrder == null)
        {
            throw new AppException(ErrorCodes.EntityNotFound("ConcessionOrder", concessionOrderId));
        }

        // Check user authorization if needed
        // (Skipping this check since we may need to delete as part of booking deletion)

        // Delete related concession order details first (though this should be handled by cascade delete)
        foreach (var detail in concessionOrder.ConcessionOrderDetails.ToList())
        {
            await _unitOfWork.ConcessionOrderDetail.RemoveAsync(detail);
        }

        // Delete concession order
        await _unitOfWork.ConcessionOrder.RemoveAsync(concessionOrder);
        await _unitOfWork.SaveAsync();
    }

    public async Task<List<ConcessionOrderDTO>> GetAllConcessionOrdersAsync()
    {
        var concessionOrders = await _unitOfWork.ConcessionOrder.GetAllAsync(includeProperties: "ConcessionOrderDetails");
        var result = _mapper.Map<List<ConcessionOrderDTO>>(concessionOrders);
        return result;
    }

    public async Task<ConcessionOrderDTO> GetConcessionOrderByIdAsync(int concessionOrderId)
    {
        var concessionOrder = await _unitOfWork.ConcessionOrder.GetAsync(
            co => co.Id == concessionOrderId,
            includeProperties: "ConcessionOrderDetails,ConcessionOrderDetails.Concession");

        if (concessionOrder == null)
        {
            throw new AppException(ErrorCodes.EntityNotFound("ConcessionOrder", concessionOrderId));
        }

        var result = _mapper.Map<ConcessionOrderDTO>(concessionOrder);
        return result;
    }

    public async Task<List<ConcessionOrderDTO>> GetConcessionOrdersByBookingIdAsync(int bookingId)
    {
        var concessionOrders = await _unitOfWork.ConcessionOrder.GetAllAsync(
            co => co.BookingId == bookingId,
            includeProperties: "ConcessionOrderDetails,ConcessionOrderDetails.Concession");

        var result = _mapper.Map<List<ConcessionOrderDTO>>(concessionOrders);
        return result;
    }
}
