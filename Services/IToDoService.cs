using shopping.DTOs;

namespace shopping.Services;
public interface IToDoService
{
    // Leituras
    Task<List<ToDoItemSummaryDto>> GetAllAsync();
    Task<ToDoItemResponseDto?> GetByIdAsync(int id);
    Task<List<ToDoItemSummaryDto>> GetByStatusAsync(bool isComplete);
    
    // Escritas

    Task<UpdateToDoItemDto>UpdateAsync(UpdateToDoItemDto dto);
    Task DeleteAsync(int id);
    Task<ToDoItemResponseDto> CreateAsync(CreateToDoItemDto dto); // ← Método correto
    
}