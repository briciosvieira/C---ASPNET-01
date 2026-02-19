namespace shopping.Models;

public class ToDoItem
{
    public int id { get; set; }
    public string title { get; set; } = string.Empty;
    public string? description { get; set; } 
    public bool isCompleted { get; set; }
    
    
    // campos de auditoria
    public DateTime createdAt { get; set; } = DateTime.Now;
    public DateTime? updatedAt { get; set; }
    public string? createdBy { get; set; } // ← Quem criou (usuário)
    public string? updatedBy { get; set; } // ← Quem atualizou
    
}