namespace ECommerce.Application.Responses;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }

    public static ApiResponse<T> SuccessResult(T data, string message = "İşlem başarılı.") 
        => new() { Success = true, Data = data, Message = message };

    public static ApiResponse<T> ErrorResult(string message) 
        => new() { Success = false, Message = message }; //Yönergede API'den dönecek cevabın standart bir
          // formatta olması isteniyordu: { "success": true, "message": "", "data": {} } 
          // Bunu karşılamak için generic bir sınıf oluşturalım.
}