using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FluentAssertions;
using MediatR;
using NSubstitute;
using NUnit.Framework;

namespace Kata.BusinessRules.Tests;

public class Tests
{
    private PaymentPipe _paymentPipe;
    
    private IShippingService _shippingServiceMock;
    private IRoyaltyDepartment _royaltyDepartmentMock;
    private IMembershipService _membershipServiceMock;
    private IEmailService _emailServiceMock;
    private IPaymentService _paymentServiceMock;


    [SetUp]
    public void Setup()
    {
        _shippingServiceMock = Substitute.For<IShippingService>();
        _royaltyDepartmentMock = Substitute.For<IRoyaltyDepartment>();
        _membershipServiceMock = Substitute.For<IMembershipService>();
        _emailServiceMock = Substitute.For<IEmailService>();
        _paymentServiceMock = Substitute.For<IPaymentService>();
        
        _paymentPipe = new PaymentPipe(
            _shippingServiceMock, 
            _royaltyDepartmentMock, 
            _membershipServiceMock,
            _emailServiceMock,
            _paymentServiceMock);
    }

    [Test]
    public void PaymentIsForPhysicalProduct_IsPackagedForShipping()
    {
        // Arrange
        var book = new Book();
        var payment = Substitute.For<IPaymentFor<Book>>();
        payment.Product.Returns(book);

        // Act
        _paymentPipe.Handle(payment, CancellationToken.None, () => Unit.Task);
        
        // Assert
        _shippingServiceMock.Received().GeneratePackingSlip(book);
    }

    [Test]
    public void PaymentIsNotForPhysicalProduct_IsNotPackagedForShipping()
    {
        // Arrange
        
        var membership = new Membership();
        var payment = Substitute.For<IPaymentFor<Membership>>();
        payment.Product.Returns(membership);

        // Act
        _paymentPipe.Handle(payment, CancellationToken.None, () => Unit.Task);
        
        // Assert
        _shippingServiceMock.DidNotReceiveWithAnyArgs().GeneratePackingSlip(default!);
    }
    
    [Test]
    public void PaymentIsNotForBook_DontCreatePackingSlipForRoyaltyDepartment()
    {
        // Arrange
        var video = new Video("");
        var productPayment = Substitute.For<IPaymentFor<Video>>();
        productPayment.Product.Returns(video);

        var packingSlip = new PackingSlip(new List<IPhysical>() { video });
        
        _shippingServiceMock.GeneratePackingSlip().ReturnsForAnyArgs(packingSlip);
        
        // Act
        _paymentPipe.Handle(productPayment, CancellationToken.None, () => Unit.Task);
        
        // Assert
        _royaltyDepartmentMock.DidNotReceiveWithAnyArgs().SendPackingSlip(packingSlip);
    }
    
    [Test]
    public void PaymentForBook_CreatePackingSlipForRoyaltyDepartment()
    {
        // Arrange
        var book = new Book();
        var productPayment = new BookPayment(book, new Person(), new Agent());

        var packingSlip = new PackingSlip(new List<IPhysical>() { book });
        
        _shippingServiceMock.GeneratePackingSlip(book).Returns(packingSlip);
        
        // Act
        _paymentPipe.Handle(productPayment, CancellationToken.None, () => Unit.Task);
        
        // Assert
        _royaltyDepartmentMock.Received().SendPackingSlip(packingSlip);
    }

    [Test] public void PaymentForMembership_MembershipIsActivated()
    {
        // Arrange
        var membership = new Membership();
        var owner = new Person();
        var payment = new MembershipPayment(membership, owner, new Agent());

        // Act
        _paymentPipe.Handle(payment, CancellationToken.None, () => Unit.Task);
        
        // Assert
        _membershipServiceMock.Received().Activate(membership, owner);
    }
    
    [Test] public void PaymentForNotMembership_MembershipIsNotActivated()
    {
        // Arrange
        var payment = Substitute.For<IPaymentFor<object>>();
        var notMembership = new object();

        payment.Product.Returns(notMembership);

        // Act
        _paymentPipe.Handle(payment, CancellationToken.None, () => Unit.Task);
        
         // Assert
         _membershipServiceMock.DidNotReceiveWithAnyArgs().Activate(default!, default!);
    }
    
    [Test] public void PaymentForUpgradeMembership_MembershipIsUpgraded()
    {
        // Arrange
        var membership = new Membership();
        var membershipUpgrade = new MembershipUpgrade(membership);
        var owner = new Person();
        var payment = new MembershipUpgradePayment(membershipUpgrade, owner, new Agent());

        // Act
        _paymentPipe.Handle(payment, CancellationToken.None, () => Unit.Task);
        
        // Assert
        _membershipServiceMock.Received().Upgrade(membership, owner);
    }
    
    [Test] public void PaymentForUpgradeMembership_EmailOwner()
    {
        // Arrange
        var owner = new Person();
        var payment = new MembershipUpgradePayment(new MembershipUpgrade(new Membership()), owner, new Agent());

        // Act
        _paymentPipe.Handle(payment, CancellationToken.None, () => Unit.Task);
        
        // Assert
        _emailServiceMock.Received().SendMail(owner, "Upgraded membership");
    }
    
    [Test] public void PaymentForMembership_EmailOwner()
    {
        // Arrange
        var owner = new Person();
        var payment = new MembershipPayment(new Membership(), owner, new Agent());

        // Act
        _paymentPipe.Handle(payment, CancellationToken.None, () => Unit.Task);
        
        // Assert
        _emailServiceMock.Received().SendMail(owner, "Activated membership");
    }
    
    [Test] public void PaymentLearningToSki_SendFirstAidToo()
    {
        // Arrange
        var video = new Video("Learning to Ski");
        var payment = new VideoPayment(video, new Person(), new Agent());
        
        var packingSlip = new PackingSlip(new List<IPhysical> { video });
        _shippingServiceMock.GeneratePackingSlip(video).Returns(packingSlip);

        var sentPackingSlip = new PackingSlip(Enumerable.Empty<IPhysical>());
        
        _shippingServiceMock.SendPackage(Arg.Do<PackingSlip>(slip => sentPackingSlip = slip));
        
        // Act
        _paymentPipe.Handle(payment, CancellationToken.None, () => Unit.Task);
        
        // Assert
        sentPackingSlip.Products.Should().Contain(physical => CheckProduct(physical, "Learning to Ski"));
        sentPackingSlip.Products.Should().Contain(physical => CheckProduct(physical, "First Aid"));
    }
    
    [Test] public void PaymentNotLearningToSki_DontSendFirstAidToo()
    {
        // Arrange
        var payment = Substitute.For<IPaymentFor<Video>>();
        var video = new Video("Heist");
        payment.Product.Returns(video);
        
        var packingSlip = new PackingSlip(new List<IPhysical> { video });
        _shippingServiceMock.GeneratePackingSlip(video).Returns(packingSlip);

        var sentPackingSlip = new PackingSlip(Enumerable.Empty<IPhysical>());
        
        _shippingServiceMock.SendPackage(Arg.Do<PackingSlip>(slip => sentPackingSlip = slip));
        
        // Act
        _paymentPipe.Handle(payment, CancellationToken.None, () => Unit.Task);
        
        // Assert
        sentPackingSlip.Products.Should().NotContain(physical => CheckProduct(physical, "First Aid"));
    }
    
    [Test] public void PaymentIsPhysicalProduct_SendCommissionToAgent()
    {
        // Arrange
        var payment = Substitute.For<IPaymentFor<IPhysical>>();
        var agent = new Agent();
        payment.Agent.Returns(agent);

        // Act
        _paymentPipe.Handle(payment, CancellationToken.None, () => Unit.Task);
        
        // Assert
        _paymentServiceMock.Received().Pay(agent);
    }
    
    private static bool CheckProduct(IPhysical physical, string title) => physical is Video v && v.Title == title;
}