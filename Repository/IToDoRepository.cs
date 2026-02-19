using shopping.Models;

namespace shopping.Repository;

public interface IToDoRepository
{
    Task<IEnumerable<ToDoItem>> GetAllAsync(); //buscar todos
    Task<ToDoItem?> GetByIdAsync(int id); // busca por id, colocando <toDoItem?> indica que o id pode ser null
    Task<ToDoItem> CreateAsync(ToDoItem item); //item criado
    Task UpdateAsync(ToDoItem item); // ← Retorna se foi atualizado
    Task DeleteAsync(int id); // ← Retorna se foi deletado
    Task<bool> ExistsAsync(int id); // ← Útil para validações
    Task<IEnumerable<ToDoItem>> GetByStatusAsync(bool isComplete); // ← Exemplo de query customizada
}