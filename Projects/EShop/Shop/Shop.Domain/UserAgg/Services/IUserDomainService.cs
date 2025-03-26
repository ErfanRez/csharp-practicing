namespace Shop.Domain.UserAgg.Services;

public interface IUserDomainService
{
    bool EmailExists(string email);

    bool PhoneNumberExists(string phoneNumber);
}

