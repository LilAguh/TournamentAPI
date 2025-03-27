
using Config;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Cards;
using Services.Interfaces;
using System.Security.Claims;
using static Models.Exceptions.CustomException;

namespace TournamentApiV2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CardController : ControllerBase
    {
        // Servicio de cartas inyectado para delegar la lógica de negocio.
        private readonly ICardService _cardService;

        // Constructor que recibe el servicio de cartas.
        public CardController(ICardService cardService)
        {
            _cardService = cardService;
        }

        // POST /Card
        // Endpoint para crear una nueva carta.
        // Solo accesible para usuarios con rol "Admin".
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateCard([FromBody] CardRequestDto card)
        {
            var createdCard = await _cardService.CreateCardAsync(card, GetAdminId());
            return CreatedAtAction(nameof(GetCardById), new { id = createdCard.Id }, createdCard);
        }

        // GET /Card/{id}
        // Endpoint para obtener los datos de una carta específica por su ID.
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCardById(int id)
        {
            var card = await _cardService.GetCardByIdAsync(id);
            return Ok(card);
        }

        // GET /Card/AllCards
        // Endpoint para obtener la lista de todas las cartas.
        [HttpGet("AllCards")]
        public async Task<IActionResult> GetAllCards()
        {
            var cards = await _cardService.GetAllCardsAsync();
            return Ok(cards);
        }

        // PUT /Card/{id}
        // Endpoint para actualizar una carta existente.
        // Solo accesible para usuarios con rol "Admin".
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCard(int id, [FromBody] CardRequestDto cardDto)
        {
            var updatedCard = await _cardService.UpdateCardAsync(id, cardDto, GetAdminId());
            return Ok(updatedCard);
        }

        // DELETE /Card/{id}
        // Endpoint para eliminar una carta.
        // Solo accesible para usuarios con rol "Admin".
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCard(int id)
        {
            var deleted = await _cardService.DeleteCardAsync(id, GetAdminId());
            return Ok(ErrorMessages.CardDeleted);
        }

        // Método auxiliar para extraer el ID del administrador desde el token JWT.
        private int GetAdminId()
        {
            var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(adminIdClaim))
            {
                throw new UnauthorizedException(ErrorMessages.AdminTokenNotFound);
            }

            if (!int.TryParse(adminIdClaim, out int adminId))
            {
                throw new UnauthorizedException(ErrorMessages.InvalidFormatTokenId);
            }

            return adminId;
        }
    }
}