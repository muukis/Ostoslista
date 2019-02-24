using Microsoft.AspNetCore.Mvc;
using OstoslistaContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using OstoslistaAPI.Common;
using OstoslistaAPI.Hubs;
using OstoslistaData;

namespace OstoslistaAPI.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    [Route("api/[controller]")]
    [Produces("application/json")]
    [AllowAnonymous]
    public class ShoppingListController : BaseController
    {
        private readonly IShoppingListService _service;
        private readonly IHubContext<ShoppingListHub, IMessages> _hubContext;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <param name="hubContext"></param>
        public ShoppingListController(IShoppingListService service, IHubContext<ShoppingListHub, IMessages> hubContext)
        {
            _service = service;
            _hubContext = hubContext;
        }

        /// <summary>
        /// Get all shopping list items
        /// </summary>
        /// <param name="shopperName">Shopper name</param>
        /// <returns><see cref="ShoppingListItemResult"/> object</returns>
        /// <response code="200">Array of shopping list items</response>
        /// <response code="400">Invalid request</response>
        /// <response code="401">Unauthorized request</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Route("{shopperName}")]
        [ProducesResponseType(typeof(ShoppingListItemResult[]), 200)]
        [ProducesResponseType(typeof(ErrorResult), 400)]
        [ProducesResponseType(typeof(ErrorResult), 401)]
        [ProducesResponseType(typeof(ErrorResult), 500)]
        public async Task<IActionResult> GetAllShoppingListItems([FromRoute] string shopperName)
        {
            try
            {
                var shopper = await _service.GetShopper(shopperName);

                if (!shopper.BypassAuthentication() && !User.GetShopperReadAuthorization(shopper))
                {
                    return Error(new ErrorResult
                    {
                        Code = HttpStatusCode.Unauthorized,
                        Classification = ErrorClassification.AuthorizationError,
                        Message = "Unauthorized request"
                    });
                }

                var items = (await _service.FindItems(o => string.Equals(o.Shopper.Name, shopperName, StringComparison.InvariantCulture))).ToResults();
                return Ok(items.OrderBy(o => o.Title));
            }
            catch (Exception)
            {
                return Error(new ErrorResult
                {
                    Code = HttpStatusCode.InternalServerError,
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
        /// <response code="401">Unauthorized request</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Route("{shopperName}/pending")]
        [ProducesResponseType(typeof(ShoppingListItemResult[]), 200)]
        [ProducesResponseType(typeof(ErrorResult), 400)]
        [ProducesResponseType(typeof(ErrorResult), 401)]
        [ProducesResponseType(typeof(ErrorResult), 500)]
        public async Task<IActionResult> GetPendingShoppingListItems([FromRoute] string shopperName)
        {
            try
            {
                var shopper = await _service.GetShopper(shopperName);

                if (!User.GetShopperReadAuthorization(shopper))
                {
                    return Error(new ErrorResult
                    {
                        Code = HttpStatusCode.Unauthorized,
                        Classification = ErrorClassification.AuthorizationError,
                        Message = "Unauthorized request"
                    });
                }

                var items = (await _service.FindItems(o => string.Equals(o.Shopper.Name, shopperName, StringComparison.InvariantCulture) && o.Pending == true)).ToResults();
                return Ok(items);
            }
            catch (Exception)
            {
                return Error(new ErrorResult
                {
                    Code = HttpStatusCode.InternalServerError,
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
        /// <response code="401">Unauthorized request</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Route("{shoppingListItemId}")]
        [ProducesResponseType(typeof(ShoppingListItemResult), 200)]
        [ProducesResponseType(typeof(ErrorResult), 400)]
        [ProducesResponseType(typeof(ErrorResult), 401)]
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
                        Code = HttpStatusCode.BadRequest,
                        Classification = ErrorClassification.EntityNotFound,
                        Message = "Shopping list item not found"
                    });
                }

                var item = searchResult.First();

                if (!User.GetShopperReadAuthorization(item.Shopper))
                {
                    return Error(new ErrorResult
                    {
                        Code = HttpStatusCode.Unauthorized,
                        Classification = ErrorClassification.AuthorizationError,
                        Message = "Unauthorized request"
                    });
                }

                return Ok(item.ToResult());
            }
            catch (Exception)
            {
                return Error(new ErrorResult
                {
                    Code = HttpStatusCode.InternalServerError,
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
        /// <response code="401">Unauthorized request</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [Route("{shopperName}")]
        [ProducesResponseType(typeof(ShoppingListItemResult), 200)]
        [ProducesResponseType(typeof(ErrorResult), 400)]
        [ProducesResponseType(typeof(ErrorResult), 401)]
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
        /// <response code="401">Unauthorized request</response>
        /// <response code="500">Internal server error</response>
        [HttpPut]
        [Route("{shopperName}/add/{title}")]
        [ProducesResponseType(typeof(ShoppingListItemResult), 200)]
        [ProducesResponseType(typeof(ErrorResult), 400)]
        [ProducesResponseType(typeof(ErrorResult), 401)]
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
                        Code = HttpStatusCode.BadRequest,
                        Classification = ErrorClassification.InvalidArgument,
                        Message = "Invalid title value"
                    });
                }

                var shopper = await _service.GetShopper(shopperName);

                if (!shopper.BypassAuthentication() && !User.GetShopperWriteAuthorization(shopper))
                {
                    return Error(new ErrorResult
                    {
                        Code = HttpStatusCode.Unauthorized,
                        Classification = ErrorClassification.AuthorizationError,
                        Message = "Unauthorized request"
                    });
                }

                var retval = (await _service.CreateItem(shopperName, title.Trim())).ToResult();

                if (retval != null)
                {
                    await _hubContext.Clients.Group(retval.ShopperName).NewItemCreated(retval.Id, retval.Title, retval.Pending);
                }

                return Ok(retval);
            }
            catch (Exception)
            {
                return Error(new ErrorResult
                {
                    Code = HttpStatusCode.InternalServerError,
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
        /// <response code="401">Unauthorized request</response>
        /// <response code="500">Internal server error</response>
        [HttpPut]
        [Route("{shoppingListItemId}/{pending}")]
        [ProducesResponseType(typeof(ShoppingListItemResult), 200)]
        [ProducesResponseType(typeof(ErrorResult), 400)]
        [ProducesResponseType(typeof(ErrorResult), 401)]
        [ProducesResponseType(typeof(ErrorResult), 500)]
        public async Task<IActionResult> UpdateShoppingListItemPendingValue([FromRoute] Guid shoppingListItemId, [FromRoute] bool pending)
        {
            try
            {
                var item = (await _service.FindItems(o => o.Id == shoppingListItemId)).Single();

                if (!item.Shopper.BypassAuthentication() && !User.GetShopperWriteAuthorization(item.Shopper))
                {
                    return Error(new ErrorResult
                    {
                        Code = HttpStatusCode.Unauthorized,
                        Classification = ErrorClassification.AuthorizationError,
                        Message = "Unauthorized request"
                    });
                }

                var retval = (await _service.UpdateItemPendingStatus(shoppingListItemId, pending)).ToResult();

                if (retval != null)
                {
                    await _hubContext.Clients.Group(retval.ShopperName).ItemPendingChanged(retval.Id, retval.Pending);
                }

                return Ok(retval);
            }
            catch (Exception)
            {
                return Error(new ErrorResult
                {
                    Code = HttpStatusCode.InternalServerError,
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
        /// <response code="401">Unauthorized request</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete]
        [Route("{shoppingListItemId}")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(ErrorResult), 400)]
        [ProducesResponseType(typeof(ErrorResult), 401)]
        [ProducesResponseType(typeof(ErrorResult), 500)]
        public async Task<IActionResult> DeleteShoppingListItem([FromRoute] Guid shoppingListItemId)
        {
            try
            {
                var item = (await _service.FindItems(o => o.Id == shoppingListItemId)).Single();

                if (!item.Shopper.BypassAuthentication() && !User.GetShopperWriteAuthorization(item.Shopper))
                {
                    return Error(new ErrorResult
                    {
                        Code = HttpStatusCode.Unauthorized,
                        Classification = ErrorClassification.AuthorizationError,
                        Message = "Unauthorized request"
                    });
                }

                var items = (await _service.DeleteItems(o => o.Id == shoppingListItemId)).ToList();
                await SendDeleteMessageToHub(items);
                return Ok(items.Count);
            }
            catch (Exception)
            {
                return Error(new ErrorResult
                {
                    Code = HttpStatusCode.InternalServerError,
                    Classification = ErrorClassification.InternalError,
                    Message = "Failed deleting shopping list item"
                });
            }
        }

        private async Task SendDeleteMessageToHub(List<ShoppingListItemEntity> items)
        {
            if (items?.Count > 0)
            {
                foreach (var item in items)
                {
                    if (!item.Id.HasValue)
                    {
                        continue;
                    }

                    await _hubContext.Clients.Group(item.Shopper.Name).RemoveItem(item.Id.Value);
                }
            }
        }

        /// <summary>
        /// Delete all unpending shopping list items
        /// </summary>
        /// <param name="shopperName">Shopper name</param>
        /// <returns>Count of deleted unpending shopping items</returns>
        /// <response code="200">Count of deleted unpending shopping items</response>
        /// <response code="400">Invalid request</response>
        /// <response code="401">Unauthorized request</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete]
        [Route("{shopperName}/unpending")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(ErrorResult), 400)]
        [ProducesResponseType(typeof(ErrorResult), 401)]
        [ProducesResponseType(typeof(ErrorResult), 500)]
        public async Task<IActionResult> DeleteAllUnpendingShoppingListItems([FromRoute] string shopperName)
        {
            try
            {
                var shopper = await _service.GetShopper(shopperName);

                if (!shopper.BypassAuthentication() && !User.GetShopperWriteAuthorization(shopper))
                {
                    return Error(new ErrorResult
                    {
                        Code = HttpStatusCode.Unauthorized,
                        Classification = ErrorClassification.AuthorizationError,
                        Message = "Unauthorized request"
                    });
                }

                var items = (await _service.DeleteItems(o => string.Equals(o.Shopper.Name, shopperName, StringComparison.InvariantCulture) && o.Pending == false)).ToList();
                await SendDeleteMessageToHub(items);
                return Ok(items.Count);
            }
            catch (Exception)
            {
                return Error(new ErrorResult
                {
                    Code = HttpStatusCode.InternalServerError,
                    Classification = ErrorClassification.InternalError,
                    Message = "Failed deleting shopping list item"
                });
            }
        }

        /// <summary>
        /// Create new shopper
        /// </summary>
        /// <param name="shopperName">Shopper name</param>
        /// <returns>Shopper name</returns>
        /// <response code="200">Shopper name</response>
        /// <response code="400">Invalid request</response>
        /// <response code="401">Unauthorized request</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [Route("addShopper")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(ErrorResult), 400)]
        [ProducesResponseType(typeof(ErrorResult), 401)]
        [ProducesResponseType(typeof(ErrorResult), 500)]
        public async Task<IActionResult> CreateNewShopper([FromBody] ShopperNameDto shopperName)
        {
            try
            {
                var shopper = await _service.CreateShopper(shopperName.Name, User.GetUserEmailIdentifier());
                return Ok(shopper.Name);
            }
            catch (Exception)
            {
                return Error(new ErrorResult
                {
                    Code = HttpStatusCode.InternalServerError,
                    Classification = ErrorClassification.InternalError,
                    Message = "Failed creating shopper"
                });
            }
        }

        /// <summary>
        /// Get shopper settings
        /// </summary>
        /// <param name="shopperName">Shopper name</param>
        /// <returns>Shopper settings</returns>
        /// <response code="200">Shopper settings</response>
        /// <response code="400">Invalid request</response>
        /// <response code="401">Unauthorized request</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Route("{shopperName}/settings")]
        [ProducesResponseType(typeof(GetShopperSettingsResult), 200)]
        [ProducesResponseType(typeof(ErrorResult), 400)]
        [ProducesResponseType(typeof(ErrorResult), 401)]
        [ProducesResponseType(typeof(ErrorResult), 500)]
        public async Task<IActionResult> GetShopperSettings([FromRoute] string shopperName)
        {
            try
            {
                var shopper = await _service.GetShopper(shopperName);

                if (!User.GetShopperOwnerAuthorization(shopper))
                {
                    return Error(new ErrorResult
                    {
                        Code = HttpStatusCode.Unauthorized,
                        Classification = ErrorClassification.AuthorizationError,
                        Message = "Unauthorized request"
                    });
                }

                return Ok(shopper.ToSettingsResult());
            }
            catch (Exception)
            {
                return Error(new ErrorResult
                {
                    Code = HttpStatusCode.InternalServerError,
                    Classification = ErrorClassification.InternalError,
                    Message = "Failed getting shopper settings"
                });
            }
        }

        /// <summary>
        /// Save shopper settings
        /// </summary>
        /// <param name="shopperSettings">Shopper name</param>
        /// <returns>Shopper settings</returns>
        /// <response code="200">Shopper settings</response>
        /// <response code="400">Invalid request</response>
        /// <response code="401">Unauthorized request</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [Route("saveShopperSettings")]
        [ProducesResponseType(typeof(GetShopperSettingsResult), 200)]
        [ProducesResponseType(typeof(ErrorResult), 400)]
        [ProducesResponseType(typeof(ErrorResult), 401)]
        [ProducesResponseType(typeof(ErrorResult), 500)]
        public async Task<IActionResult> SaveShopperSettings([FromBody] SetShopperSettingsDto shopperSettings)
        {
            try
            {
                var shopper = await _service.GetShopper(shopperSettings.Name);

                if (!User.GetShopperOwnerAuthorization(shopper))
                {
                    return Error(new ErrorResult
                    {
                        Code = HttpStatusCode.Unauthorized,
                        Classification = ErrorClassification.AuthorizationError,
                        Message = "Unauthorized request"
                    });
                }

                shopper = await _service.SaveShopperSettings(shopperSettings.ToDataObject(shopper.Id));
                return Ok(shopper.ToSettingsResult());
            }
            catch (Exception)
            {
                return Error(new ErrorResult
                {
                    Code = HttpStatusCode.InternalServerError,
                    Classification = ErrorClassification.InternalError,
                    Message = "Failed setting shopper settings"
                });
            }
        }

        /// <summary>
        /// Create new shopper
        /// </summary>
        /// <param name="shopperName">Shopper name</param>
        /// <returns>Shopper settings</returns>
        /// <response code="200">Array of shopper friend requests</response>
        /// <response code="400">Invalid request</response>
        /// <response code="401">Unauthorized request</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Route("{shopperName}/friendRequests")]
        [ProducesResponseType(typeof(ShopperFriendRequestResult[]), 200)]
        [ProducesResponseType(typeof(ErrorResult), 400)]
        [ProducesResponseType(typeof(ErrorResult), 401)]
        [ProducesResponseType(typeof(ErrorResult), 500)]
        public async Task<IActionResult> GetShopperFriendRequests([FromRoute] string shopperName)
        {
            try
            {
                var shopper = await _service.GetShopper(shopperName);

                if (!User.GetShopperOwnerAuthorization(shopper))
                {
                    return Error(new ErrorResult
                    {
                        Code = HttpStatusCode.Unauthorized,
                        Classification = ErrorClassification.AuthorizationError,
                        Message = "Unauthorized request"
                    });
                }

                return Ok(shopper.FriendRequests.ToResults<ShopperFriendRequestResult>());
            }
            catch (Exception)
            {
                return Error(new ErrorResult
                {
                    Code = HttpStatusCode.InternalServerError,
                    Classification = ErrorClassification.InternalError,
                    Message = "Failed getting shopper friend requests"
                });
            }
        }

        /// <summary>
        /// Create new shopper
        /// </summary>
        /// <param name="shopperName">Shopper name</param>
        /// <returns>Shopper settings</returns>
        /// <response code="200">Array of shopper friends</response>
        /// <response code="400">Invalid request</response>
        /// <response code="401">Unauthorized request</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Route("{shopperName}/friends")]
        [ProducesResponseType(typeof(ShopperFriendResult[]), 200)]
        [ProducesResponseType(typeof(ErrorResult), 400)]
        [ProducesResponseType(typeof(ErrorResult), 401)]
        [ProducesResponseType(typeof(ErrorResult), 500)]
        public async Task<IActionResult> GetShopperFriends([FromRoute] string shopperName)
        {
            try
            {
                var shopper = await _service.GetShopper(shopperName);

                if (!User.GetShopperOwnerAuthorization(shopper))
                {
                    return Error(new ErrorResult
                    {
                        Code = HttpStatusCode.Unauthorized,
                        Classification = ErrorClassification.AuthorizationError,
                        Message = "Unauthorized request"
                    });
                }

                return Ok(shopper.Friends.ToResults<ShopperFriendResult>());
            }
            catch (Exception)
            {
                return Error(new ErrorResult
                {
                    Code = HttpStatusCode.InternalServerError,
                    Classification = ErrorClassification.InternalError,
                    Message = "Failed getting shopper friends"
                });
            }
        }

        /// <summary>
        /// Set shopper friend request approved or rejected
        /// </summary>
        /// <param name="shopperFriendRequestId">Shopper friend request identifier</param>
        /// <param name="approve">Approve shopper request identifier</param>
        /// <returns>Shopper friend if approved</returns>
        /// <response code="200">Shopper friend</response>
        /// <response code="400">Invalid request</response>
        /// <response code="401">Unauthorized request</response>
        /// <response code="500">Internal server error</response>
        [HttpPut]
        [Route("setShopperFriendRequest/{shopperFriendRequestId}/{approve}")]
        [ProducesResponseType(typeof(ShopperFriendResult), 200)]
        [ProducesResponseType(typeof(ErrorResult), 400)]
        [ProducesResponseType(typeof(ErrorResult), 401)]
        [ProducesResponseType(typeof(ErrorResult), 500)]
        public async Task<IActionResult> SetShopperFriendRequest([FromRoute] Guid shopperFriendRequestId, [FromRoute] bool approve)
        {
            try
            {
                var shopperFriendRequest = await _service.GetShopperFriendRequest(shopperFriendRequestId);

                if (!User.GetShopperOwnerAuthorization(shopperFriendRequest.Shopper))
                {
                    return Error(new ErrorResult
                    {
                        Code = HttpStatusCode.Unauthorized,
                        Classification = ErrorClassification.AuthorizationError,
                        Message = "Unauthorized request"
                    });
                }

                var retval = await _service.SetShopperFriendRequest(shopperFriendRequestId, approve);
                return Ok(retval?.ToResult<ShopperFriendResult>());
            }
            catch (Exception)
            {
                return Error(new ErrorResult
                {
                    Code = HttpStatusCode.InternalServerError,
                    Classification = ErrorClassification.InternalError,
                    Message = $"Failed setting shopper friend request {(approve ? "approved" : "rejected")}"
                });
            }
        }

        /// <summary>
        /// Delete shopper friend
        /// </summary>
        /// <param name="shopperFriendId">Shopper friend identifier</param>
        /// <returns>Deleted shopper friend</returns>
        /// <response code="200">Shopper friend</response>
        /// <response code="400">Invalid request</response>
        /// <response code="401">Unauthorized request</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete]
        [Route("deleteShopperFriend/{shopperFriendId}")]
        [ProducesResponseType(typeof(ShopperFriendResult), 200)]
        [ProducesResponseType(typeof(ErrorResult), 400)]
        [ProducesResponseType(typeof(ErrorResult), 401)]
        [ProducesResponseType(typeof(ErrorResult), 500)]
        public async Task<IActionResult> DeleteShopperFriend([FromRoute] Guid shopperFriendId)
        {
            try
            {
                var shopperFriend = await _service.GetShopperFriend(shopperFriendId);

                if (!User.GetShopperOwnerAuthorization(shopperFriend.Shopper) &&
                    !string.Equals(shopperFriend.Email, User.GetUserEmailIdentifier(), StringComparison.InvariantCultureIgnoreCase))
                {
                    return Error(new ErrorResult
                    {
                        Code = HttpStatusCode.Unauthorized,
                        Classification = ErrorClassification.AuthorizationError,
                        Message = "Unauthorized request"
                    });
                }

                var retval = await _service.DeleteShopperFriend(shopperFriendId);
                return Ok(retval?.ToResult<ShopperFriendResult>());
            }
            catch (Exception)
            {
                return Error(new ErrorResult
                {
                    Code = HttpStatusCode.InternalServerError,
                    Classification = ErrorClassification.InternalError,
                    Message = "Failed deleting shopper friend"
                });
            }
        }

        /// <summary>
        /// Create shopper friend request
        /// </summary>
        /// <param name="shopperName">Shopper name</param>
        /// <returns>Created shopper friend request</returns>
        /// <response code="200">Shopper friend request</response>
        /// <response code="400">Invalid request</response>
        /// <response code="401">Unauthorized request</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Route("createShopperFriendRequest/{shopperName}")]
        [ProducesResponseType(typeof(ShopperFriendRequestResult), 200)]
        [ProducesResponseType(typeof(ErrorResult), 400)]
        [ProducesResponseType(typeof(ErrorResult), 401)]
        [ProducesResponseType(typeof(ErrorResult), 500)]
        public async Task<IActionResult> CreateShopperFriendRequest([FromRoute] string shopperName)
        {
            try
            {
                var shopper = await _service.GetShopper(shopperName);

                if (!(shopper?.AllowNewFriendRequests ?? false) ||
                    !User.Identity.IsAuthenticated)
                {
                    return Error(new ErrorResult
                    {
                        Code = HttpStatusCode.Unauthorized,
                        Classification = ErrorClassification.AuthorizationError,
                        Message = "Unauthorized request"
                    });
                }

                string userEmailIdentifier = User.GetUserEmailIdentifier();

                if (shopper.FriendRequests.Any(o => string.Equals(o.Email, userEmailIdentifier, StringComparison.InvariantCultureIgnoreCase)) ||
                    shopper.Friends.Any(o => string.Equals(o.Email, userEmailIdentifier, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return Error(new ErrorResult
                    {
                        Code = HttpStatusCode.BadRequest,
                        Classification = ErrorClassification.InvalidOperation,
                        Message = "User is already shopper friend or shopper friend requested"
                    });
                }

                var retval = await _service.CreateShopperFriendRequest(
                    shopper.Id ?? Guid.Empty, userEmailIdentifier, User.Identity.Name, User.GetUserImageUrl());

                return Ok(retval?.ToResult<ShopperFriendRequestResult>());
            }
            catch (Exception)
            {
                return Error(new ErrorResult
                {
                    Code = HttpStatusCode.InternalServerError,
                    Classification = ErrorClassification.InternalError,
                    Message = "Failed creating shopper friend requests"
                });
            }
        }

        /// <summary>
        /// Delete my shopper friend request
        /// </summary>
        /// <param name="shopperName">Shopper name</param>
        /// <returns>Deleted shopper friend request</returns>
        /// <response code="200">Shopper friend request</response>
        /// <response code="400">Invalid request</response>
        /// <response code="401">Unauthorized request</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete]
        [Route("deleteMyShopperFriendRequest/{shopperName}")]
        [ProducesResponseType(typeof(ShopperFriendRequestResult), 200)]
        [ProducesResponseType(typeof(ErrorResult), 400)]
        [ProducesResponseType(typeof(ErrorResult), 401)]
        [ProducesResponseType(typeof(ErrorResult), 500)]
        public async Task<IActionResult> DeleteMyShopperFriendRequest([FromRoute] string shopperName)
        {
            try
            {
                var shopper = await _service.GetShopper(shopperName);
                var retval = await _service.DeleteShopperFriendRequestByEmail(shopper.Id ?? Guid.Empty, User.GetUserEmailIdentifier());
                return Ok(retval?.ToResult<ShopperFriendRequestResult>());
            }
            catch (Exception)
            {
                return Error(new ErrorResult
                {
                    Code = HttpStatusCode.InternalServerError,
                    Classification = ErrorClassification.InternalError,
                    Message = "Failed deleting my shopper friend request"
                });
            }
        }

        /// <summary>
        /// Delete my shopper friend
        /// </summary>
        /// <param name="shopperName">Shopper name</param>
        /// <returns>Deleted shopper friend</returns>
        /// <response code="200">Shopper friend</response>
        /// <response code="400">Invalid request</response>
        /// <response code="401">Unauthorized request</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete]
        [Route("deleteMyShopperFriend/{shopperName}")]
        [ProducesResponseType(typeof(ShopperFriendResult), 200)]
        [ProducesResponseType(typeof(ErrorResult), 400)]
        [ProducesResponseType(typeof(ErrorResult), 401)]
        [ProducesResponseType(typeof(ErrorResult), 500)]
        public async Task<IActionResult> DeleteMyShopperFriend([FromRoute] string shopperName)
        {
            try
            {
                var shopper = await _service.GetShopper(shopperName);
                var retval = await _service.DeleteShopperFriendByEmail(shopper.Id ?? Guid.Empty, User.GetUserEmailIdentifier());
                return Ok(retval?.ToResult<ShopperFriendResult>());
            }
            catch (Exception)
            {
                return Error(new ErrorResult
                {
                    Code = HttpStatusCode.InternalServerError,
                    Classification = ErrorClassification.InternalError,
                    Message = "Failed deleting my shopper friend"
                });
            }
        }

        /// <summary>
        /// Get my shoppers
        /// </summary>
        /// <returns>All the shoppers the user is involved in</returns>
        /// <response code="200">List of shoppers</response>
        /// <response code="400">Invalid request</response>
        /// <response code="401">Unauthorized request</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Route("getMyShoppers")]
        [ProducesResponseType(typeof(MyShoppersResult), 200)]
        [ProducesResponseType(typeof(ErrorResult), 400)]
        [ProducesResponseType(typeof(ErrorResult), 401)]
        [ProducesResponseType(typeof(ErrorResult), 500)]
        public async Task<IActionResult> GetMyShoppers()
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return Error(new ErrorResult
                    {
                        Code = HttpStatusCode.Unauthorized,
                        Classification = ErrorClassification.AuthorizationError,
                        Message = "Unauthorized request"
                    });
                }

                var email = User.GetUserEmailIdentifier();
                var retval = new MyShoppersResult
                {
                    MyShoppers = (await _service.GetMyShoppers(email)).ToMyShopperResults(),
                    FriendShoppers = (await _service.GetFriendShoppers(email)).ToFriendShoppersResult(),
                    FriendRequestedShoppers = (await _service.GetFriendRequestedShoppers(email)).ToResults<FriendRequestedShoppersResult>(),
                };

                return Ok(retval);
            }
            catch (Exception)
            {
                return Error(new ErrorResult
                {
                    Code = HttpStatusCode.InternalServerError,
                    Classification = ErrorClassification.InternalError,
                    Message = "Failed getting my shoppers"
                });
            }
        }
    }
}
