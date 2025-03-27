using Common.Domain;
using Common.Domain.Exceptions;

namespace Shop.Domain.SellerAgg
{
    public class Seller : AggregateRoot
    {
        private Seller()
        {

        }

        public Seller(long userId, string shopName, string nationalCode)
        {
            Guard(shopName, nationalCode);
            UserId = userId;
            ShopName = shopName;
            NationalCode = nationalCode;
            Inventories = new List<SellerInventory>();
        }

        public long UserId { get; private set; }

        public string ShopName { get; private set; }

        public string NationalCode { get; private set; }

        public SellerStatus Status { get; private set; }

        public DateTime? LastUpdate { get; private set; }

        public List<SellerInventory> Inventories { get; private set; }

        public void AddInventory(SellerInventory inventory)
        {
            if (Inventories.Any(f => f.ProductId == inventory.ProductId))
                throw new InvalidDomainDataException("Duplicate product ID!");

            Inventories.Add(inventory);
        }

        public void EditInventory(SellerInventory NewInventory)
        {
            var currInventory = Inventories.FirstOrDefault(f => f.Id == NewInventory.Id);
            if (currInventory == null) return;


            Inventories.Remove(currInventory);
            Inventories.Add(NewInventory);

        }

        public void DeleteInventory(long inventoryId)
        {
            var currInventory = Inventories.FirstOrDefault(f => f.Id == inventoryId);
            if (currInventory == null) throw new NullOrEmptyDomainDataException("Product not found!");


            Inventories.Remove(currInventory);

        }

        public void ChangeStatus(SellerStatus status)
        {
            Status = status;
            LastUpdate = DateTime.Now;
        }

        public void Edit(string shopName, string nationalCode)
        {
            Guard(shopName, nationalCode);
            ShopName = shopName;
            NationalCode = nationalCode;
        }

        public void Guard(string shopName, string nationalCode)
        {
            NullOrEmptyDomainDataException.CheckString(shopName, nameof(shopName));
            NullOrEmptyDomainDataException.CheckString(nationalCode, nameof(nationalCode));

            if (IranianNationalIdChecker.IsValid(nationalCode) == false)
                throw new InvalidDomainDataException("Invalid national code");
        }
    }
}
