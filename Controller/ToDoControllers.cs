using Microsoft.AspNetCore.Mvc;
using shopping.DTOs;
using shopping.Services;

namespace shopping.Controller;

[ApiController]
[Route("api/v1/todo")]
public class ToDoControllers: ControllerBase
{
    private readonly IToDoService _service;
    private readonly ILogger<ToDoControllers> _logger;

    public ToDoControllers(IToDoService service, ILogger<ToDoControllers> logger)
    {
        _service = service;
        _logger = logger;
    }
    
    //GET retorna todos os itens, resumido.
    [HttpGet]
    [ProducesResponseType(typeof(List<ToDoItemSummaryDto>), 200)]
    public async Task<ActionResult<List<ToDoItemSummaryDto>>> Get()
    {
        try
        {
            var items =await _service.GetAllAsync();
                return Ok(items);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro no endpoint de busca");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
    
    /// GET /api/v1/todo/{id} - Retorna um item específico (completo)
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ToDoItemResponseDto), 200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<ToDoItemResponseDto>> GetById(int id)
    {
        try
        {
            var item = await _service.GetByIdAsync(id);
            
            if (item == null)
            {
                return NotFound(new { message = $"Item {id} Não encontrado" });
            }

            return Ok(item);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro no endpoint GetById com Id {Id}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
    
    /// GET /api/v1/todo/status/{isComplete} - Busca itens por status
    [HttpGet("status/{isComplete}")]
    [ProducesResponseType(typeof(List<ToDoItemSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ToDoItemSummaryDto>>> GetByStatus(bool isComplete)
    {
        try
        {
            var items = await _service.GetByStatusAsync(isComplete);
            return Ok(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro no endpoint GetByStatus");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
    
    // POST /api/v1/todo - Cria um novo item
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ToDoItemResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ToDoItemResponseDto>> Create([FromBody] CreateToDoItemDto dto)
    {
        try
        {
            // Validação do ModelState (Data Annotations)
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var created = await _service.CreateAsync(dto);
            
            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                created
            );
        }
        catch (ArgumentException ex)
        {
            // Erro de validação (400 Bad Request)
            _logger.LogWarning(ex, "Validação falhou no endpoint Create");
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            // Erro de regra de negócio (409 Conflict)
            _logger.LogWarning(ex, "Regra de negócio violada no endpoint Create");
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado no endpoint Create");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// PUT /api/todo/{id} - Atualiza um item existente
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateToDoItemDto dto)
    {
        try
        {
            // Validação: ID da URL deve corresponder ao ID do DTO
            if (id != dto.Id)
            {
                return BadRequest(new { message = "URL ID does not match body ID" });
            }

            // Validação do ModelState (Data Annotations)
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updated = await _service.UpdateAsync(dto);
            
            if (!updated)
            {
                return NotFound(new { message = $"Item {id} not found" });
            }

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            // Erro de validação (400 Bad Request)
            _logger.LogWarning(ex, "Validação falhou no endpoint Update");
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            // Erro de regra de negócio (409 Conflict)
            _logger.LogWarning(ex, "Regra de negócio violada no endpoint Update");
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado no endpoint Update com Id {Id}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
    
    /// DELETE /api/v1/todo/{id} - Deleta um item
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var deleted = await _service.DeleteAsync(id);
            
            if (!deleted)
            {
                return NotFound(new { message = $"Item {id} Não encontrado" });
            }

            return NoContent();
        }
        catch (InvalidOperationException e)
        {
            // Erro de regra de negócio (409 Conflict)
            _logger.LogWarning(e, "Regra de negócio violada no endpoint Delete");
            return Conflict(new { message = e.Message });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro inesperado no endpoint Delete com Id {Id}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
}
