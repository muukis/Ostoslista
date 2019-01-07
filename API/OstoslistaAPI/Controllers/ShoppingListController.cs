using Microsoft.AspNetCore.Mvc;
using OstoslistaContracts;
using System;
using System.Linq;
using System.Threading.Tasks;
using OstoslistaInterfaces;
using OstoslistaServices;

namespace OstoslistaAPI.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ShoppingListController : BaseController
    {
        private IShoppingListService service = new ShoppingListServiceMock();

        /// <summary>
        /// Get all shopping list items
        /// </summary>
        /// <returns><see cref="ShoppingListItemResult"/> object</returns>
        /// <response code="200">Array of shopping list items</response>
        /// <response code="400">Invalid request</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [ProducesResponseType(typeof(ShoppingListItemResult[]), 200)]
        [ProducesResponseType(typeof(ErrorResult), 400)]
        [ProducesResponseType(typeof(ErrorResult), 500)]
        public async Task<IActionResult> GetAllShoppingListItems()
        {
            try
            {
                return Ok(await service.FindItems(o => true));
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
        /// <returns>Array of <see cref="ShoppingListItemResult"/> objects</returns>
        /// <response code="200">Array of pending shopping list items</response>
        /// <response code="400">Invalid request</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Route("pending")]
        [ProducesResponseType(typeof(ShoppingListItemResult[]), 200)]
        [ProducesResponseType(typeof(ErrorResult), 400)]
        [ProducesResponseType(typeof(ErrorResult), 500)]
        public async Task<IActionResult> GetPendingShoppingListItems()
        {
            try
            {
                return Ok(await service.FindItems(o => o.Pending == true));
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
                var searchResult = (await service.FindItems(o => o.Id == shoppingListItemId)).ToList();

                if (!searchResult.Any())
                {
                    return Error(new ErrorResult
                    {
                        Code = 600,
                        Classification = ErrorClassification.EntityNotFound,
                        Message = "Shopping list item not found"
                    });
                }

                return Ok(searchResult.First());
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
        /// Create new shopping list item
        /// </summary>
        /// <param name="title">Shopping list item title</param>
        /// <returns>Newly created <see cref="ShoppingListItemResult"/> object</returns>
        /// <response code="200">Details about the created shopping list item</response>
        /// <response code="400">Invalid request</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [ProducesResponseType(typeof(ShoppingListItemResult), 200)]
        [ProducesResponseType(typeof(ErrorResult), 400)]
        [ProducesResponseType(typeof(ErrorResult), 500)]
        public async Task<IActionResult> PostShoppingListItem([FromBody] string title)
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

                return Ok(await service.CreateItem(title.Trim()));
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
                var searchResult = (await service.FindItems(o => o.Id == shoppingListItemId)).ToList();

                if (!searchResult.Any())
                {
                    return Error(new ErrorResult
                    {
                        Code = 600,
                        Classification = ErrorClassification.EntityNotFound,
                        Message = "Shopping list item not found"
                    });
                }

                var shoppingListItem = searchResult.First();

                shoppingListItem.Pending = pending;
                shoppingListItem.Modified = DateTime.Now;

                return Ok(await service.Save(shoppingListItem));
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
                int count = await service.DeleteItems(o => o.Id == shoppingListItemId);
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
        /// <returns>Count of deleted unpending shopping items</returns>
        /// <response code="200">Count of deleted unpending shopping items</response>
        /// <response code="400">Invalid request</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete]
        [Route("unpending")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(ErrorResult), 400)]
        [ProducesResponseType(typeof(ErrorResult), 500)]
        public async Task<IActionResult> DeleteAllUnpendingShoppingListItems()
        {
            try
            {
                int count = await service.DeleteItems(o => o.Pending == false);
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
    }
}
