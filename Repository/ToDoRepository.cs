using Microsoft.EntityFrameworkCore;
using shopping.Data;
using shopping.Models;

namespace shopping.Repository;

public class ToDoRepository : IToDoRepository
{
    private readonly ToDoContext _context;

    public ToDoRepository(ToDoContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ToDoItem>> GetAllAsync()
    {
        return await _context.TodoItems
            .AsNoTracking()
            .OrderByDescending(or => or.createdAt)
            .ToListAsync();
    }

    public async Task<ToDoItem?> GetByIdAsync(int id)
    {
        return await _context.TodoItems
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.id == id);
    }

    public async Task<ToDoItem> CreateAsync(ToDoItem item)
    {
        await _context.TodoItems.AddAsync(item);
        return item;
    }

    public async Task UpdateAsync(ToDoItem item)
    {
        _context.TodoItems.Update(item);
        await _context.SaveChangesAsync();
        
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.TodoItems.FindAsync(id);
        if (entity != null)
            _context.TodoItems.Remove(entity);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.TodoItems.AnyAsync(t => t.id == id);
    }

    public async Task<IEnumerable<ToDoItem>> GetByStatusAsync(bool isComplete)
    {
        return await _context.TodoItems
            .AsNoTracking()
            .Where(t => t.isCompleted == isComplete)
            .OrderByDescending(or => or.createdAt)
            .ToListAsync();
    }

}
