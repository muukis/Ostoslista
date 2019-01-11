using Microsoft.AspNetCore.Mvc;
using OstoslistaContracts;
using System;
using System.Linq;
using System.Threading.Tasks;
using OstoslistaData;

namespace OstoslistaAPI.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ShoppingListController : BaseController
    {
        private readonly IShoppingListService _service;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        public ShoppingListController(IShoppingListService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get all shopping list items
        /// </summary>
        /// <param name="shopperName">Shopper name</param>
        /// <returns><see cref="ShoppingListItemResult"/> object</returns>
        /// <response code="200">Array of shopping list items</response>
        /// <response code="400">Invalid request</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Route("{shopperName}")]
        [ProducesResponseType(typeof(ShoppingListItemResult[]), 200)]
        [ProducesResponseType(typeof(ErrorResult), 400)]
        [ProducesResponseType(typeof(ErrorResult), 500)]
        public async Task<IActionResult> GetAllShoppingListItems([FromRoute] string shopperName)
        {
            try
            {
                var items = (await _service.FindItems(o => string.Equals(o.Shopper.Name, shopperName, StringComparison.InvariantCulture))).ToResults();
                return Ok(items.OrderBy(o => o.Title));
            }
            catch (Exception)
            {
                return Error(new ErrorResult
                {
                    Code = 900,
                    Classification = ErrorClassification.InternalError,
                    Message = "Failed getting shopping list items"
                });
            }
        }

        /// <summary>
        /// Get all pending shopping list items
        /// </summary>
        /// <param name="shopperName">Shopper name</param>
        /// <returns>Array of <see cref="ShoppingListItemResult"/> objects</returns>
        /// <response code="200">Array of pending shopping list items</response>
        /// <response code="400">Invalid request</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Route("{shopperName}/pending")]
        [ProducesResponseType(typeof(ShoppingListItemResult[]), 200)]
        [ProducesResponseType(typeof(ErrorResult), 400)]
        [ProducesResponseType(typeof(ErrorResult), 500)]
        public async Task<IActionResult> GetPendingShoppingListItems([FromRoute] string shopperName)
        {
            try
            {
                var items = (await _service.FindItems(o => string.Equals(o.Shopper.Name, shopperName, StringComparison.InvariantCulture) && o.Pending == true)).ToResults();
                return Ok(items);
            }
            catch (Exception)
            {
                return Error(new ErrorResult
                {
                    Code = 900,
                    Classification = ErrorClassification.InternalError,
                    Message = "Failed getting pending shopping list items"
                });
            }
        }

        /// <summary>
        /// Get shopping list item
        /// </summary>
        /// <param name="shoppingListItemId">Shopping list item identifier</param>
        /// <returns><see cref="ShoppingListItemResult"/> object</returns>
        /// <response code="200">Shopping list item with the given id</response>
        /// <response code="400">Invalid request</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Route("{shoppingListItemId}")]
        [ProducesResponseType(typeof(ShoppingListItemResult), 200)]
        [ProducesResponseType(typeof(ErrorResult), 400)]
        [ProducesResponseType(typeof(ErrorResult), 500)]
        public async Task<IActionResult> GetShoppingListItem([FromRoute] Guid shoppingListItemId)
        {
            try
            {
                var searchResult = (await _service.FindItems(o => o.Id == shoppingListItemId)).ToList();

                if (!searchResult.Any())
                {
                    return Error(new ErrorResult
                    {
                        Code = 600,
                        Classification = ErrorClassification.EntityNotFound,
                        Message = "Shopping list item not found"
                    });
                }

                return Ok(searchResult.First().ToResult());
            }
            catch (Exception)
            {
                return Error(new ErrorResult
                {
                    Code = 900,
                    Classification = ErrorClassification.InternalError,
                    Message = "Failed getting shopping list item"
                });
            }
        }

        /// <summary>
        /// Create new shopping list item with request type POST
        /// </summary>
        /// <param name="shopperName">Shopper name</param>
        /// <param name="title">Shopping list item title</param>
        /// <returns>Newly created <see cref="ShoppingListItemResult"/> object</returns>
        /// <response code="200">Details about the created shopping list item</response>
        /// <response code="400">Invalid request</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [Route("{shopperName}")]
        [ProducesResponseType(typeof(ShoppingListItemResult), 200)]
        [ProducesResponseType(typeof(ErrorResult), 400)]
        [ProducesResponseType(typeof(ErrorResult), 500)]
        public async Task<IActionResult> PostShoppingListItem([FromRoute] string shopperName, [FromBody] ShoppingListTitleDto title)
        {
            return await SaveNewShoppingListItem(shopperName, title?.Title);
        }

        /// <summary>
        /// Create new shopping list item with request type PUT
        /// </summary>
        /// <param name="shopperName">Shopper name</param>
        /// <param name="title">Shopping list item title</param>
        /// <returns>Newly created <see cref="ShoppingListItemResult"/> object</returns>
        /// <response code="200">Details about the created shopping list item</response>
        /// <response code="400">Invalid request</response>
        /// <response code="500">Internal server error</response>
        [HttpPut]
        [Route("{shopperName}/add/{title}")]
        [ProducesResponseType(typeof(ShoppingListItemResult), 200)]
        [ProducesResponseType(typeof(ErrorResult), 400)]
        [ProducesResponseType(typeof(ErrorResult), 500)]
        public async Task<IActionResult> SetShoppingListItem([FromRoute] string shopperName, [FromRoute] string title)
        {
            return await SaveNewShoppingListItem(shopperName, title);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shopperName">Shopper name</param>
        /// <param name="title"></param>
        /// <returns></returns>
        private async Task<IActionResult> SaveNewShoppingListItem(string shopperName, string title)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(title))
                {
                    return Error(new ErrorResult
                    {
                        Code = 400,
                        Classification = ErrorClassification.InvalidArgument,
                        Message = "Invalid title value"
                    });
                }

                return Ok((await _service.CreateItem(shopperName, title.Trim())).ToResult());
            }
            catch (Exception)
            {
                return Error(new ErrorResult
                {
                    Code = 900,
                    Classification = ErrorClassification.InternalError,
                    Message = "Failed creating new shopping list item"
                });
            }
        }

        /// <summary>
        /// Update shopping list item in-cart value
        /// </summary>
        /// <param name="shoppingListItemId">Shopping list item identifier</param>
        /// <param name="pending">New shopping list item pending value</param>
        /// <returns><see cref="ShoppingListItemResult"/> object</returns>
        /// <response code="200">Details about the updated shopping list item</response>
        /// <response code="400">Invalid request</response>
        /// <response code="500">Internal server error</response>
        [HttpPut]
        [Route("{shoppingListItemId}/{pending}")]
        [ProducesResponseType(typeof(ShoppingListItemResult), 200)]
        [ProducesResponseType(typeof(ErrorResult), 400)]
        [ProducesResponseType(typeof(ErrorResult), 500)]
        public async Task<IActionResult> UpdateShoppingListItemPendingValue([FromRoute] Guid shoppingListItemId, [FromRoute] bool pending)
        {
            try
            {
                return Ok((await _service.UpdateItemPendingStatus(shoppingListItemId, pending)).ToResult());
            }
            catch (Exception)
            {
                return Error(new ErrorResult
                {
                    Code = 900,
                    Classification = ErrorClassification.InternalError,
                    Message = "Failed updating shopping list item"
                });
            }
        }

        /// <summary>
        /// Delete shopping list item
        /// </summary>
        /// <param name="shoppingListItemId">Shopping list item identifier</param>
        /// <returns>Count of deleted shopping items</returns>
        /// <response code="200">Count of deleted shopping items</response>
        /// <response code="400">Invalid request</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete]
        [Route("{shoppingListItemId}")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(ErrorResult), 400)]
        [ProducesResponseType(typeof(ErrorResult), 500)]
        public async Task<IActionResult> DeleteShoppingListItem([FromRoute] Guid shoppingListItemId)
        {
            try
            {
                int count = await _service.DeleteItems(o => o.Id == shoppingListItemId);
                return Ok(count);
            }
            catch (Exception)
            {
                return Error(new ErrorResult
                {
                    Code = 900,
                    Classification = ErrorClassification.InternalError,
                    Message = "Failed deleting shopping list item"
                });
            }
        }

        /// <summary>
        /// Delete all unpending shopping list items
        /// </summary>
        /// <param name="shopperName">Shopper name</param>
        /// <returns>Count of deleted unpending shopping items</returns>
        /// <response code="200">Count of deleted unpending shopping items</response>
        /// <response code="400">Invalid request</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete]
        [Route("{shopperName}/unpending")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(ErrorResult), 400)]
        [ProducesResponseType(typeof(ErrorResult), 500)]
        public async Task<IActionResult> DeleteAllUnpendingShoppingListItems([FromRoute] string shopperName)
        {
            try
            {
                int count = await _service.DeleteItems(o => o.Pending == false);
                return Ok(count);
            }
            catch (Exception)
            {
                return Error(new ErrorResult
                {
                    Code = 900,
                    Classification = ErrorClassification.InternalError,
                    Message = "Failed deleting shopping list item"
                });
            }
        }

        /// <summary>
        /// Create new shopper
        /// </summary>
        /// <param name="shopperName">Shopper name</param>
        /// <returns>Count of deleted unpending shopping items</returns>
        /// <response code="200">Shopper name</response>
        /// <response code="400">Invalid request</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [Route("addShopper")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(ErrorResult), 400)]
        [ProducesResponseType(typeof(ErrorResult), 500)]
        public async Task<IActionResult> CreateNewShopper([FromBody] ShopperNameDto shopperName)
        {
            try
            {
                var shopper = await _service.CreateShopper(shopperName.Name);
                return Ok(shopper.Name);
            }
            catch (Exception)
            {
                return Error(new ErrorResult
                {
                    Code = 900,
                    Classification = ErrorClassification.InternalError,
                    Message = "Failed deleting shopping list item"
                });
            }
        }
    }
}
