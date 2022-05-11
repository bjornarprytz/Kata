using MediatR;

namespace Kata.BusinessRules;

// http://codekata.com/kata/kata16-business-rules/

public record Person();
public record Agent() : Person;

public record PackingSlip(IEnumerable<IPhysical> Products);

// Products
public interface IPhysical { }
public record Book() : IPhysical;
public record Video(string Title) : IPhysical;
public record Membership();
public record MembershipUpgrade(Membership NewMembership);

// Payment Requests

public interface IPaymentFor<out T> : IRequest
{
    T Product { get; }
    Agent Agent { get; }
}
public record MembershipPayment(Membership Membership, Person Owner, Agent Agent) : IPaymentFor<Membership>
{
    public Membership Product => Membership;
}

public record MembershipUpgradePayment(MembershipUpgrade MembershipUpgrade, Person Owner, Agent Agent) : IPaymentFor<MembershipUpgrade>
{
    public MembershipUpgrade Product => MembershipUpgrade;
}

public record BookPayment(Book Book, Person Buyer, Agent Agent) : IPaymentFor<Book>
{
    public Book Product => Book;
}

public record VideoPayment(Video Video, Person Buyer, Agent Agent) : IPaymentFor<Video>
{
    public Video Product => Video;
}

public class PaymentHandler : IRequestHandler<IRequest>
{
    public Task<Unit> Handle(IRequest request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Handling request of type {request.GetType()}");
        return Unit.Task;
    }
}

public class PaymentPipe : IPipelineBehavior<IRequest, Unit>
{
    private readonly IShippingService _shippingService;
    private readonly IRoyaltyDepartment _royaltyDepartment;
    private readonly IMembershipService _membershipService;
    private readonly IEmailService _emailService;
    private readonly IPaymentService _paymentService;

    public PaymentPipe(IShippingService shippingService,
        IRoyaltyDepartment royaltyDepartment,
        IMembershipService membershipService,
        IEmailService emailService, 
        IPaymentService paymentService)
    {
        _shippingService = shippingService;
        _royaltyDepartment = royaltyDepartment;
        _membershipService = membershipService;
        _emailService = emailService;
        _paymentService = paymentService;
    }
    
    public Task<Unit> Handle(IRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<Unit> next)
    {
        var result = next();
        
        switch (request)
        {
            case MembershipPayment membershipPayment:
                _membershipService.Activate(membershipPayment.Membership, membershipPayment.Owner);
                _emailService.SendMail(membershipPayment.Owner, "Activated membership");
                break;
            case MembershipUpgradePayment membershipUpgradePayment:
                _membershipService.Upgrade(membershipUpgradePayment.Product.NewMembership, membershipUpgradePayment.Owner);
                _emailService.SendMail(membershipUpgradePayment.Owner, "Upgraded membership");
                break;
        }

        if (request is not IPaymentFor<IPhysical> physicalPayment) return result;
        
        var packingSlip = _shippingService.GeneratePackingSlip(physicalPayment.Product);

        if (request is VideoPayment { Video.Title: "Learning to Ski" } )
        {
            packingSlip = new PackingSlip(packingSlip.Products.Append(new Video("First Aid")));
        }
        
        _shippingService.SendPackage(packingSlip);
        
        if (physicalPayment.Product is Book)
        {
            _royaltyDepartment.SendPackingSlip(packingSlip);
        }
        
        _paymentService.Pay(physicalPayment.Agent);
        
        return result;
    }
}


public interface IShippingService
{
    PackingSlip GeneratePackingSlip(params IPhysical[] products);

    void SendPackage(PackingSlip packingSlip);
}

public interface IMembershipService
{
    void Activate(Membership membership, Person owner);
    void Upgrade(Membership membership, Person owner);
}

public interface IEmailService
{
    void SendMail(Person recipient, string text);
}

public interface IPaymentService
{
    void Pay(Person person);
}

public interface IRoyaltyDepartment
{
    void SendPackingSlip(PackingSlip packingSlip);
}