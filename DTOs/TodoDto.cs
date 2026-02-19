using System.ComponentModel.DataAnnotations;

namespace shopping.DTOs;

// DTO para CRIAR item (POST /api/todo)
// Cliente envia: título e descrição
// ==========================================
public record CreateToDoItemDto
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters")]
    public string Title { get; init; } = string.Empty;  // ← PascalCase + init
    
    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; init; }  // ← PascalCase + init + sem MinimumLength
    
    // NÃO tem: id, isCompleted, CreatedAt, updatedAt, CreatedBy, UpdatedBy
}

// ==========================================
// DTO para ATUALIZAR item (PUT /api/todo/{id})
// Cliente envia: id, título, descrição, status
// ==========================================
public record UpdateToDoItemDto
{   
    [Required]
    public int Id { get; init; }  // ← PascalCase
    
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters")]
    public string Title { get; init; } = string.Empty;  // ← PascalCase
    
    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; init; }  // ← PascalCase + nullable + sem Required
    public bool IsComplete { get; init; }  // ← PascalCase + sem Required (bool tem valor padrão false)
}

// ==========================================
// DTO para RESPOSTA COMPLETA (GET /api/todo/{id})
// Servidor retorna: todos os campos (sem dados sensíveis)
// ==========================================
public record ToDoItemResponseDto
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsComplete { get; init; }
    
    // Auditoria visível
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public string? CreatedBy { get; init; }
    public string? UpdatedBy { get; init; }
}

// ==========================================
// DTO para LISTAGEM RESUMIDA (GET /api/todo)
// Servidor retorna: só dados essenciais (performance)
// ==========================================
public record ToDoItemSummaryDto
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public bool IsComplete { get; init; }
    public DateTime CreatedAt { get; init; }
}