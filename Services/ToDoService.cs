using Microsoft.AspNetCore.Http.HttpResults;
using shopping.DTOs;
using shopping.Models;
using shopping.Repository;

namespace shopping.Services;



public class ToDoService : IToDoService
{
    private readonly IToDoRepository _repository;
    private readonly ILogger<ToDoService> _logger;

    public ToDoService(IToDoRepository repository, ILogger<ToDoService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    // === LEITURAS ===
    public async Task<List<ToDoItemSummaryDto>> GetAllAsync()
    {
        try
        {
            _logger.LogInformation("Buscando todos os itens");
            var items = await _repository.GetAllAsync();

            if (items == null)
            {
                throw new ArgumentException("Nenhum item encontrado");

            }
            var result = items.Select(item => new ToDoItemSummaryDto
            {
                Id = item.id,
                Title = item.title,
                IsComplete = item.isCompleted,
                CreatedAt = item.createdAt
            }).ToList();

            _logger.LogInformation("Encontrados {Count} itens", result.Count);
            return result;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro ao buscar todos os  items");
            throw;
        }
    }

    public async Task<ToDoItemResponseDto?> GetByIdAsync(int id)
    {
        try
        {
            _logger.LogInformation("Buscando item {Id}", id);

            var item = await _repository.GetByIdAsync(id);
            if (item == null)
            {
                _logger.LogWarning("Item {Id} não encontrado.", id);
                return null;
            }

            return new ToDoItemResponseDto
            {
                Id = item.id,
                Title = item.title,
                Description = item.description,
                IsComplete = item.isCompleted,
                CreatedAt = item.createdAt,
                UpdatedAt = item.updatedAt,
                CreatedBy = item.createdBy,
                UpdatedBy = item.updatedBy
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro ao buscar item {Id} no Service", id);
            throw;
        }
    }

    public async Task<List<ToDoItemSummaryDto>> GetByStatusAsync(bool isComplete)
    {
        try
        {
            _logger.LogInformation("Buscando itens com status {Status}", isComplete ? "completo" : "incompleto");
            var items = await _repository.GetByStatusAsync(isComplete);
            var result = items.Select(item => new ToDoItemSummaryDto
            {
                Id = item.id,
                Title = item.title,
                IsComplete = item.isCompleted,
                CreatedAt = item.createdAt
            }).ToList();

            _logger.LogInformation("Encontrados {Count} itens com status {Status}",
                result.Count, isComplete ? "completo" : "incompleto");

            return result;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro ao buscar itens por status no Service");
            throw;
        }
    }

    // === ESCRITAS ===
    //create
    public async Task<ToDoItemResponseDto> CreateAsync(CreateToDoItemDto dto)
    {
        try
        {
            _logger.LogInformation("Iniciando criação de item: '{Title}'", dto.Title);

            //validando titulo obrigstorio (validado por required, mas vamos garantir)
            if (string.IsNullOrWhiteSpace(dto.Title))
            {
                _logger.LogWarning("Tentativa de criar item sem título");
                throw new ArgumentException("Título é obrigatório");
            }

            //tamanho do titulo
            var titleLENGTH = dto.Title.Trim().Length;
            if (titleLENGTH < 3 || titleLENGTH > 100)
            {
                _logger.LogWarning("O titulo não pode ter menos que 3 caracter e maior que 100, quantidade digitada {lenth}", titleLENGTH);
                throw new ArgumentException("Titulo com caracter abaixo de 3 ou acima de 100 digitos");
            }

            //REGRA DE NEGOCIO: palavras proibidas
            var prohibitedWords = new[] { "teste123", "Delete", "xxX", "333", };
            var titleLower = dto.Title.ToLower();
            if (prohibitedWords.Any(words => titleLower.Contains(words)))
            {
                _logger.LogWarning("Tentativa de criar item com palavra proibida: '{Title}'", dto.Title);
                throw new ArgumentException("Título contém palavras não permitidas");
            }

            //REGRA DE NEGOCIO 2: Verificar titulo duplicado.
            var allTitle = await _repository.GetAllAsync();
            var duplicteTitle = allTitle.Any(d => d.title.Equals(dto.Title.Trim(), StringComparison.OrdinalIgnoreCase));

            if (duplicteTitle)
            {
                _logger.LogWarning("Tentativa de criar item com título duplicado: '{Title}'", dto.Title);
                throw new InvalidOperationException("Já existe um item com este título");
            }

            //REGRA DE NEGÓCIO 3: Normalizar dados (remover espaco do inicio e fim)
            var titleTrim = dto.Title.Trim();
            var descriptionTrim = dto.Description?.Trim();

            //PONTO IMPORTENTE QUE É A CONVERSAO DO DTO -> ENTITY

            var item = new ToDoItem
            {
                title = titleTrim,
                description = descriptionTrim,
                isCompleted = false,
                createdAt = DateTime.UtcNow,
                updatedAt = null,
                createdBy = "system", //Todo: Pegar o contexto da autenticacao
                updatedBy = null
            };
            var created = await _repository.CreateAsync(item); //Salvando no banco

            _logger.LogInformation("Item criado com sucesso: Id = {id}, Titulo : {title}", created.id, created.title);

            return new ToDoItemResponseDto
            {
                Title = created.title,
                Description = created.description,
                CreatedAt = created.createdAt,
                CreatedBy = created.createdBy,
            };
        }
        catch (ArgumentException e)
        {
            _logger.LogWarning(e, "Validação falhou ao criar item");
            throw;
        }
        catch (InvalidOperationException e)
        {
            _logger.LogWarning(e, "Regra de negócio violada ao criar item");
            throw; // Re-lança para Controller tratar (409 Conflict)
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro inesperado ao criar item no Service");
            throw;
        }

    }
    // UPDATE
    public async Task<UpdateToDoItemDto> UpdateAsync(UpdateToDoItemDto dto)
    {
        try
        {
            _logger.LogInformation("Iniciando atualização do item {Id}", dto.Id);

            var existingItem = await _repository.GetByIdAsync(dto.Id);
            if (existingItem == null)
                throw new Exception("Item não encontrado.");

            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new ArgumentException("Título é obrigatório");

            var titleLength = dto.Title.Trim().Length;
            if (titleLength < 3 || titleLength > 200)
                throw new ArgumentException("Título deve ter entre 3 e 200 caracteres");

            if (existingItem.isCompleted && !dto.IsComplete)
                throw new InvalidOperationException(
                    "Não é permitido desmarcar um item que já foi completado."
                );

            var prohibitedWords = new[] { "teste123", "delete", "xxx", "333" };
            var titleLower = dto.Title.ToLower();
            if (prohibitedWords.Any(w => titleLower.Contains(w)))
                throw new ArgumentException("Título contém palavras não permitidas");

            var allTitle = await _repository.GetAllAsync();
            var duplicateTitle = allTitle.Any(d =>
                d.id != dto.Id &&
                d.title.Equals(dto.Title.Trim(), StringComparison.OrdinalIgnoreCase)
            );

            if (duplicateTitle)
                throw new InvalidOperationException("Já existe um item com este título");

            if (!existingItem.isCompleted && dto.IsComplete)
                _logger.LogInformation("Item {Id} marcado como completo {Title}", dto.Id, dto.Title);

            existingItem.title = dto.Title.Trim();
            existingItem.description = dto.Description?.Trim();
            existingItem.isCompleted = dto.IsComplete;
            existingItem.updatedAt = DateTime.UtcNow;
            existingItem.updatedBy = "system";

            await _repository.UpdateAsync(existingItem);

            _logger.LogInformation("Item {Id} atualizado com sucesso", dto.Id);
            //Retorno correto
            return new UpdateToDoItemDto
            {
                Id = existingItem.id,
                Title = existingItem.title,
                Description = existingItem.description,
                IsComplete = existingItem.isCompleted
            };
        }

    catch (ArgumentException e)
        {
            _logger.LogWarning(e, "Validação falhou ao atualizar item {Id}", dto.Id);
            throw;
        }
        catch (InvalidOperationException e)
        {
            _logger.LogWarning(e, "Regra de negócio violada ao atualizar item {Id}", dto.Id);
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro inesperado ao atualizar item {Id} no Service", dto.Id);
            throw;
        }
    }

    //DELETEE
    public async Task DeleteAsync(int id)
    {
        try
        {
            _logger.LogInformation("Iniciando deleção do item {Id}", id);

            //VALIDAÇÃO: Verificar se item existe
            var item = await _repository.GetByIdAsync(id);

            if (item == null)
                throw new Exception("Item não encontrado.");

            //REGRA DE NEGÓCIO 1: Não deletar itens completados há menos de 7 dias
            if (item.isCompleted && item.updatedAt.HasValue)
            {
                var daysSinceCompletion = (DateTime.UtcNow - item.updatedAt.Value).Days;

                if (daysSinceCompletion < 7)
                {
                    _logger.LogWarning(
                        "Tentativa de deletar item {Id} completado há apenas {Days} dias",
                        id,
                        daysSinceCompletion
                    );
                    throw new InvalidOperationException(
                        $"Cannot delete items completed less than 7 days ago. " +
                        $"This item was completed {daysSinceCompletion} day(s) ago. " +
                        $"Wait {7 - daysSinceCompletion} more day(s) or mark as incomplete first."
                    );
                }
            }

            //REGRA DE NEGÓCIO 2: Não deletar itens com palavras-chave "importante" no título
            if (item.title.Contains("important", StringComparison.OrdinalIgnoreCase) ||
                item.title.Contains("urgent", StringComparison.OrdinalIgnoreCase) ||
                item.title.Contains("critical", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning(
                    "Tentativa de deletar item importante {Id}: '{Title}'",
                    id,
                    item.title
                );
                throw new InvalidOperationException(
                    "Items marked as 'important', 'urgent' or 'critical' cannot be deleted. " +
                    "Remove these words from the title before deleting."
                );
            }

            //REGRA DE NEGÓCIO 3: Avisar se está deletando item não completo
            if (!item.isCompleted)
            {
                _logger.LogInformation(
                    "Deletando item {Id} ainda não completado: '{Title}'",
                    id,
                    item.title
                );
            }

            // Deletar do banco
            await _repository.DeleteAsync(id);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Regra de negócio violada ao deletar item {Id}", id);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao deletar item {Id} no Service", id);
            throw;
        }
    }
}