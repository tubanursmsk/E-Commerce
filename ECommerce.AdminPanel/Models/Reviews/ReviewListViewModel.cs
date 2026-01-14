using System.ComponentModel.DataAnnotations;
namespace ECommerce.AdminPanel.Models.Reviews;
public class ReviewListViewModel
{
    public Guid Id { get; set; }
    public string Comment { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}