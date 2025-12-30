using AutoMapper;
using ECommerce.Application.DTOs.Order;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Responses;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<OrderDto>>> GetAllAsync()
    {
        var orders = await _unitOfWork.Orders.GetAllAsync();
        return ApiResponse<IEnumerable<OrderDto>>.SuccessResult(_mapper.Map<IEnumerable<OrderDto>>(orders));
    }

    public async Task<ApiResponse<OrderDto>> GetByIdAsync(Guid id)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(id);
        if (order == null) return ApiResponse<OrderDto>.ErrorResult("Sipariş bulunamadı.");
        return ApiResponse<OrderDto>.SuccessResult(_mapper.Map<OrderDto>(order));
    }

    public async Task<ApiResponse<IEnumerable<OrderDto>>> SearchByOrderNumberAsync(string orderNumber)
{
    if (string.IsNullOrWhiteSpace(orderNumber))
        return await GetAllAsync();

    // Sadece OrderNumber içinde (Büyük/Küçük harf duyarsız) arama yapar
    // Önemli: DTO'da OrderItems listesinin dolu gelmesi için Repository'de Include(x => x.OrderItems) yapılmalıdır.
    var orders = await _unitOfWork.Orders.FindAsync(o => 
        o.OrderNumber.ToLower().Contains(orderNumber.ToLower()));

    if (orders == null || !orders.Any())
    {
        return ApiResponse<IEnumerable<OrderDto>>.SuccessResult(new List<OrderDto>(), $"'{orderNumber}' numaralı sipariş kaydı bulunamadı.");
    }

    var dtos = _mapper.Map<IEnumerable<OrderDto>>(orders);
    return ApiResponse<IEnumerable<OrderDto>>.SuccessResult(dtos);
}

    public async Task<ApiResponse<Guid>> CreateOrderAsync(OrderCreateDto dto)
    {
        // 1. Sipariş numarasını otomatik üret (Yönergeye uygun benzersiz kod)
        var order = _mapper.Map<Order>(dto);
        order.OrderNumber = "ORD-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
        order.Status = ECommerce.Domain.Enums.OrderStatus.Pending;

        // 2. Sipariş kalemlerini ekle ve STOK KONTROLÜ yap
        foreach (var item in order.OrderItems)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId); //UnitOfWork kullanmanın faydası: Eğer sipariş kalemlerinde bir hata çıkarsa, siparişin kendisi de kaydedilmeyecek.
            if (product == null || product.Stock < item.Quantity)
            {
                return ApiResponse<Guid>.ErrorResult($"{product?.Name ?? "Ürün"} için yetersiz stok!");
            }

            // Stok düşürme işlemi
            product.Stock -= item.Quantity;
            _unitOfWork.Products.Update(product);
        }

        await _unitOfWork.Orders.AddAsync(order);

        // 3. Tek bir SaveChanges ile her şeyi (Sipariş + Kalemler + Stok Güncelleme) kaydet
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<Guid>.SuccessResult(order.Id, "Siparişiniz başarıyla oluşturuldu.");
    }

    public async Task<ApiResponse<bool>> UpdateStatusAsync(Guid id, ECommerce.Domain.Enums.OrderStatus status)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(id);
        if (order == null) return ApiResponse<bool>.ErrorResult("Sipariş bulunamadı.");

        order.Status = status;
        _unitOfWork.Orders.Update(order);
        await _unitOfWork.SaveChangesAsync();
        return ApiResponse<bool>.SuccessResult(true, "Sipariş durumu güncellendi.");
    }





}

