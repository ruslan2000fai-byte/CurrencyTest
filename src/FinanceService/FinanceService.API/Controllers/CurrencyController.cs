using System.IdentityModel.Tokens.Jwt;
using FinanceService.API.Requests;
using FinanceService.Application.Commands.AddFavoriteCurrency;
using FinanceService.Application.Commands.RemoveFavoriteCurrency;
using FinanceService.Application.Queries.GetUserCurrencies;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceService.API.Controllers;

[ApiController]
[Route("api/finance")]
[Authorize]
public class CurrencyController : ControllerBase
{
    private readonly IMediator _mediator;

    public CurrencyController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private int GetUserId()
    {
        var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
               ?? User.FindFirst("sub")?.Value;
        return int.TryParse(sub, out var id) ? id : 0;
    }

    [HttpGet("currencies")]
    public async Task<IActionResult> GetCurrencies()
    {
        var userId = GetUserId();
        var result = await _mediator.Send(new GetUserCurrenciesQuery(userId));
        return Ok(result);
    }

    [HttpPost("favorites")]
    public async Task<IActionResult> AddFavorite([FromBody] FavoriteRequest request)
    {
        var userId = GetUserId();
        var added = await _mediator.Send(new AddFavoriteCurrencyCommand(userId, request.CurrencyId));
        return added
            ? Ok(new { message = "Added to favorites." })
            : Conflict(new { message = "Currency is already in favorites." });
    }

    [HttpDelete("favorites/{currencyId:int}")]
    public async Task<IActionResult> RemoveFavorite(int currencyId)
    {
        var userId = GetUserId();
        var removed = await _mediator.Send(new RemoveFavoriteCurrencyCommand(userId, currencyId));
        return removed
            ? Ok(new { message = "Removed from favorites." })
            : NotFound(new { message = "Favorite not found." });
    }
}
