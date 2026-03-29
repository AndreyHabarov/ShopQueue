using ShopQueue.Domain.Entities;

namespace ShopQueue.Responses;

public record ShopResponse(Guid Id, string Name, string Address, DateTime CreatedAt)
{
    public static ShopResponse FromEntity(Shop shop)
    {
        return new ShopResponse(shop.Id, shop.Name, shop.Address, shop.CreatedAt);
    }
};