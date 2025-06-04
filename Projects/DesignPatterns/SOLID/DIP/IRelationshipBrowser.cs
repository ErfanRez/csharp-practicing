namespace SOLID.DIP;

public interface IRelationshipBrowser
{
    IEnumerable<Person> FindAllChildrenOf(string name);
}