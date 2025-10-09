namespace Creational;
public class Builder
{
    public static void Main()
    {
        Person person = Person.Builder()
            .Lives()
                .WithZipCode("1234")
                .At("Sample Street")
                .In("New York")
            .Works()
                .As("Engineer")
                .At("Google")
                .Earning(100_000)
                .Build();
    }
}

class Person
{

    // Address Info
    public string Street { get; set; }
    public string ZipCode { get; set; }
    public string City { get; set; }

    // Employment Info
    public string CompanyName { get; set; }
    public string Position { get; set; }
    public int Salary { get; set; }

    public static PersonBuilder Builder() => new();
}


// Builder Facade
class PersonBuilder
{
    private Person _person = new();

    public PersonAddressBuilder Lives() => new(_person);
    public PersonJobBuilder Works() => new(_person);

    public Person Build() => this._person;
}


class PersonAddressBuilder : PersonBuilder
{
    private Person _person;

    public PersonAddressBuilder(Person personAddress)
    {
        _person = personAddress;
    }

    public PersonAddressBuilder At(string street)
    {
        _person.Street = street;
        return this;
    }

    public PersonAddressBuilder In(string city)
    {
        _person.City = city;
        return this;
    }

    public PersonAddressBuilder WithZipCode(string zipCode)
    {
        _person.ZipCode = zipCode;
        return this;
    }


}


class PersonJobBuilder : PersonBuilder
{
    private Person _person;

    public PersonJobBuilder(Person person)
    {
        _person = person;
    }

    public PersonJobBuilder At(string company)
    {
        _person.CompanyName = company;
        return this;
    }

    public PersonJobBuilder As(string position)
    {
        _person.Position = position;
        return this;
    }

    public PersonJobBuilder Earning(int salary)
    {
        _person.Salary = salary;
        return this;
    }

}