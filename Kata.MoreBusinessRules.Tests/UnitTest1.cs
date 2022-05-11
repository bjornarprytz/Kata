using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Kata.MoreBusinessRules.Tests;

public class Tests
{
    private Business _business;
    [SetUp]
    public void Setup()
    {
        _business = new Business();
    }

    [Test]
    [TestCase(PaymentMethod.Credit, typeof(CheckStock), typeof(ProcessPayment), typeof(ShipOrder) )]
    [TestCase(PaymentMethod.Debit, typeof(CheckStock), typeof(AwaitPayment), typeof(ProcessPayment), typeof(ShipOrder))]
    [TestCase(PaymentMethod.Invoice, typeof(CheckCompanyCredit), typeof(CheckStock), typeof(GenerateInvoice), typeof(ShipOrder), typeof(AwaitPayment), typeof(ProcessPayment))]
    public void OrderWithPaymentMethod_OrderOfBusinessIsCorrect(PaymentMethod method, params Type[] expectedOrder)
    {
        // Arrange
        var order = new MakeOrder(new Order(method));
        
        // Act
        _business.Handle(order);
        
        // Assert
        var orderProcesses = _business.GetOrderStatus(order.Order).Processes.ToList();

        orderProcesses.Count.Should().Be(expectedOrder.Length);
        
        foreach (var (businessProcess, type) in orderProcesses.Zip(expectedOrder))
        {
            businessProcess.Should().BeOfType(type);
        }
    }

    [Test]
    [TestCase(PaymentMethod.Debit)]
    [TestCase(PaymentMethod.Invoice)]
    public void OrderAwaitingPayment_CustomerSendsCheck_OrderProceeds(PaymentMethod method)
    {
        // Arrange
        var order = new Order(method);
        var orderAction = new MakeOrder(order);
        var sendCheckAction = new SendCheck(order);
        
        // Act
        _business.Handle(orderAction);
        _business.ProcessOrders();
        _business.Handle(sendCheckAction);

        // Assert
        _business.GetOrderStatus(order)?.Processes.Should().NotContain(new AwaitPayment());
    }
    
    [Test]
    [TestCase(PaymentMethod.Credit)]
    [TestCase(PaymentMethod.Debit)]
    [TestCase(PaymentMethod.Invoice)]
    public void OrderUnderway_CustomerCancelsOrder_OrderIsDeleted(PaymentMethod method)
    {
        // Arrange
        var order = new Order(method);
        var orderAction = new MakeOrder(order);
        var cancelOrder = new CancelOrder(order);
        
        // Act
        _business.Handle(orderAction);
        _business.ProcessOrders();
        _business.Handle(cancelOrder);

        // Assert
        _business.GetOrderStatus(order).Should().BeNull();
    }
}