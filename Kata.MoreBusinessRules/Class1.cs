namespace Kata.MoreBusinessRules;

// http://codekata.com/kata/kata17-more-business-rules/

public enum PaymentMethod
{
    Credit,
    Debit,
    Invoice
}

public record Order(PaymentMethod PaymentMethod);

// Business
public interface IBusinessProcess
{
}

public record AwaitPayment() : IBusinessProcess;

public record struct ProcessPayment() : IBusinessProcess;

public record struct CheckStock() : IBusinessProcess;

public record struct AwaitRestock() : IBusinessProcess;

public record struct ShipOrder() : IBusinessProcess;

public record struct GenerateInvoice() : IBusinessProcess;

public record struct CheckCompanyCredit() : IBusinessProcess;

public interface ICustomerAction
{
}

public record MakeOrder(Order Order) : ICustomerAction;

public record CancelOrder(Order Order) : ICustomerAction;

public record SendCheck(Order Order) : ICustomerAction;

public interface IBusinessAction
{
}

public record Restock() : IBusinessAction;

public record CompletePayment() : IBusinessAction;

public class OrderProcess
{
    private Stack<IBusinessProcess> _processes = new ();

    public OrderProcess(Order order)
    {
        Order = order;
        switch (order.PaymentMethod)
        {
            case PaymentMethod.Credit:
                Enqueue(new CheckStock(), new ProcessPayment(), new ShipOrder());
                break;
            case PaymentMethod.Debit:
                Enqueue(new CheckStock(), new AwaitPayment(), new ProcessPayment(), new ShipOrder());
                break;
            case PaymentMethod.Invoice:
                Enqueue(new CheckCompanyCredit(), new CheckStock(), new GenerateInvoice(), new ShipOrder(), new AwaitPayment(), new ProcessPayment());
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public Order Order { get; }

    public IBusinessProcess Current => _processes.Peek();
    
    public IEnumerable<IBusinessProcess> Processes => _processes;

    public void Handle<T>() where T : IBusinessProcess
    {
        _processes = new Stack<IBusinessProcess>(_processes.Where(p => p.GetType() != typeof(T)));
    }

    public void Handle(Type type)
    {
        _processes = new Stack<IBusinessProcess>(_processes.Where(p => p.GetType() != type));
    }
    
    public void Push(params IBusinessProcess[] processes)
    {
        foreach (var process in processes)
        {
            _processes.Push(process);
        }
    }

    public void Enqueue(params IBusinessProcess[] processes)
    {
        var existingProcesses = _processes.ToList();

        _processes = new Stack<IBusinessProcess>();

        var newOrder = 
            existingProcesses
                .Concat(processes)
                .Reverse();

        foreach (var process in newOrder)
        {
            _processes.Push(process);
        }
    }
};

public interface IBusiness
{
    public OrderProcess? GetOrderStatus(Order order);
    public void Handle(ICustomerAction customerAction);
    public void Handle(IBusinessAction businessAction);

    public void ProcessOrders();
}

public class Business : IBusiness
{
    private readonly Dictionary<Order, OrderProcess> _processes;
    public Business()
    {
        _processes = new Dictionary<Order, OrderProcess>();
    }

    public OrderProcess? GetOrderStatus(Order order) => _processes.ContainsKey(order) ? _processes[order] : null;

    public void Handle(ICustomerAction customerAction)
    {
        switch (customerAction)
        {
            case MakeOrder { Order: { } order}:
                _processes.Add(order, new OrderProcess(order));
                break;
            case CancelOrder { Order: { } order }:
                _processes.Remove(order);
                break;
            case SendCheck { Order: { } order }:
                _processes[order].Handle<AwaitPayment>();
                break;
        }
    }

    public void Handle(IBusinessAction businessAction)
    {
        throw new NotImplementedException();
    }

    public void ProcessOrders()
    {
        foreach (var process in _processes.Values)
        {
            switch (process.Current)
            {
                case AwaitPayment:
                case AwaitRestock:
                    continue;
                default:
                process.Handle(process.GetType());
                    break;
            }
        }
    }
}