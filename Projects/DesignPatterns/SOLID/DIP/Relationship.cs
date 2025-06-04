namespace SOLID.DIP;

// hl modules should not depend on low-level; both should depend on abstractions
// abstractions should not depend on details; details should depend on abstractions

public enum Relationship
{
    Parent,
    Child,
    Sibling
}