namespace Kata.SuperMarketPricing;

public record Product(string Name, Price Price);
public record Price(decimal Cost);
public record Payment(decimal Money);
public record Transaction(IEnumerable<Product> Products, Payment Payment);

public record SpecialDeal(string Name, Price Price, IEnumerable<Product> Components) : Product(Name, Price);


