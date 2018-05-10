using Microsoft.AspNetCore.Mvc;
using OstoslistaContracts;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        [HttpGet]
        [SwaggerOperation("GetShoppingListItems")]
        [SwaggerResponse(200, typeof(ShoppingListItemResult[]), "Returns array of shopping list items")]
        [SwaggerResponse(400, typeof(ErrorResult), "Invalid request")]
        [SwaggerResponse(500, typeof(ErrorResult), "Internal server error")]
        public async Task<IActionResult> GetShoppingListItems()
        {
            try
            {
                return Ok(await service.FindShoppingListItems(o => true));
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
        [HttpGet]
        [Route("{shoppingListItemId}")]
        [SwaggerOperation("GetShoppingListItem")]
        [SwaggerResponse(200, typeof(ShoppingListItemResult), "Returns the shopping list item with the given id")]
        [SwaggerResponse(400, typeof(ErrorResult), "Invalid request")]
        [SwaggerResponse(500, typeof(ErrorResult), "Internal server error")]
        public async Task<IActionResult> GetShoppingListItem([FromRoute] Guid shoppingListItemId)
        {
            try
            {
                var searchResult = (await service.FindShoppingListItems(o => o.Id == shoppingListItemId)).ToList();

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
        [HttpPost]
        [SwaggerOperation("CreateShoppingListItem")]
        [SwaggerResponse(200, typeof(ShoppingListItemResult), "Details about the created shopping list item")]
        [SwaggerResponse(400, typeof(ErrorResult), "Invalid request")]
        [SwaggerResponse(500, typeof(ErrorResult), "Internal server error")]
        public async Task<IActionResult> PostShoppingListItem([FromBody] string title)
        {
            try
            {
                return Ok(await service.CreateShoppingListItem(title));
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
        /// <param name="inCart">New shopping list item in-cart value</param>
        /// <returns><see cref="ShoppingListItemResult"/> object</returns>
        [HttpPut]
        [Route("{shoppingListItemId}/{inCart}")]
        [SwaggerOperation("UpdateShoppingListItemInCartValue")]
        [SwaggerResponse(200, typeof(ShoppingListItemResult), "Details about the updated shopping list item")]
        [SwaggerResponse(400, typeof(ErrorResult), "Invalid request")]
        [SwaggerResponse(500, typeof(ErrorResult), "Internal server error")]
        public async Task<IActionResult> UpdateShoppingListItemInCartValue([FromRoute] Guid shoppingListItemId, [FromRoute] bool inCart)
        {
            try
            {
                var searchResult = (await service.FindShoppingListItems(o => o.Id == shoppingListItemId)).ToList();

                if (!searchResult.Any())
                {
                    return Error(new ErrorResult
                    {
                        Code = 600,
                        Classification = ErrorClassification.EntityNotFound,
                        Message = "Shopping list item not found"
                    });
                }

                var retval = searchResult.First();

                retval.InCart = inCart;
                retval.Modified = DateTime.Now;

                return Ok(retval);
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
        [HttpDelete]
        [Route("{shoppingListItemId}")]
        [SwaggerOperation("DeleteShoppingListItem")]
        [SwaggerResponse(200)]
        [SwaggerResponse(400, typeof(ErrorResult), "Invalid request")]
        [SwaggerResponse(500, typeof(ErrorResult), "Internal server error")]
        public async Task<IActionResult> DeleteShoppingListItem([FromRoute] Guid shoppingListItemId)
        {
            try
            {
                await service.DeleteShoppingListItems(o => o.Id == shoppingListItemId);
                return Ok();
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
