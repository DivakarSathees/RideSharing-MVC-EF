using System.Collections.Generic;
using System.Linq;
using RideShare.Controllers;
using RideShare.Exceptions;
using RideShare.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace RideShare.Tests
{
    [TestFixture]
public class RideSharingTests
{
    private DbContextOptions<RideSharingDbContext> _dbContextOptions;

    [SetUp]
    public void Setup()
    {
        _dbContextOptions = new DbContextOptionsBuilder<RideSharingDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        using (var dbContext = new RideSharingDbContext(_dbContextOptions))
        {
            // Add test data to the in-memory database
            var ride = new Ride
            {
                RideID = 1,
                DepartureLocation = "Location A",
                Destination = "Location B",
                DateTime = DateTime.Parse("2023-08-30"),
                MaximumCapacity = 4
            };

            dbContext.Rides.Add(ride);
            dbContext.SaveChanges();
        }
    }

    [TearDown]
    public void TearDown()
    {
        using (var dbContext = new RideSharingDbContext(_dbContextOptions))
        {
            // Clear the in-memory database after each test
            dbContext.Database.EnsureDeleted();
        }
    }

    [Test]
    public void JoinRide_ValidCommuter_JoinsSuccessfully()
    {
        using (var dbContext = new RideSharingDbContext(_dbContextOptions))
        {
            // Arrange
            var slotController = new SlotController(dbContext);
            var commuter = new Commuter
            {
                Name = "John Doe",
                Email = "johndoe@example.com",
                Phone = "1234567890"
            };

            // Act
            var result = slotController.JoinRide(1, commuter) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Details", result.ActionName);
            Assert.AreEqual("Ride", result.ControllerName);

            var ride = dbContext.Rides.Include(r => r.Commuters).FirstOrDefault(r => r.RideID == 1);
            Assert.IsNotNull(ride);
            Assert.AreEqual(1, ride.Commuters.Count);
            Assert.AreEqual(4, ride.MaximumCapacity);
        }
    }

    [Test]
    public void JoinRide_ValidCommuter_JoinsSuccessfully2()
    {
        using (var dbContext = new RideSharingDbContext(_dbContextOptions))
        {
            // Arrange
            var slotController = new SlotController(dbContext);
            var commuter = new Commuter
            {
                Name = "John Doe",
                Email = "johndoe@example.com",
                Phone = "1234567890"
            };

            // Act
            var result = slotController.JoinRide(1, commuter) as RedirectToActionResult;

            var ride = dbContext.Rides.Include(r => r.Commuters).FirstOrDefault(r => r.RideID == 1);
            Assert.IsNotNull(ride);
            Assert.AreEqual(1, ride.Commuters.Count);
        }
    }

    [Test]
    public void JoinRide_ValidCommuter_JoinsSuccessfully3()
    {
        using (var dbContext = new RideSharingDbContext(_dbContextOptions))
        {
            // Arrange
            var slotController = new SlotController(dbContext);
            var commuter = new Commuter
            {
                Name = "John Doe",
                Email = "johndoe@example.com",
                Phone = "1234567890"
            };

            // Act
            var result = slotController.JoinRide(1, commuter) as RedirectToActionResult;
            var ride = dbContext.Rides.Include(r => r.Commuters).FirstOrDefault(r => r.RideID == 1);

            Assert.AreEqual(4, ride.MaximumCapacity);
        }
    }




    [Test]
    public void JoinRide_ValidCommuter_JoinsSuccessfully1()
    {
        using (var dbContext = new RideSharingDbContext(_dbContextOptions))
        {
            // Arrange
            var slotController = new SlotController(dbContext);
            var commuter = new Commuter
            {
                Name = "John Doe",
                Email = "johndoe@example.com",
                Phone = "1234567890"
            };

            // Act
            var result = slotController.JoinRide(1, commuter) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Details", result.ActionName);
            Assert.AreEqual("Ride", result.ControllerName);
        }
    }


    // [Test]
    // public void JoinRide_InvalidCommuter_ModelStateInvalid()
    // {
    //     using (var dbContext = new RideSharingDbContext(_dbContextOptions))
    //     {
    //         // Arrange
    //         var slotController = new SlotController(dbContext);
    //         var commuter = new Commuter(); // Invalid commuter with missing required fields

    //         // Act
    //         slotController.ModelState.AddModelError("Name", "Name is required");
    //         var result = slotController.JoinRide(1, commuter) as ViewResult;

    //         // Assert
    //         Assert.IsNotNull(result);
    //         Assert.AreEqual("", result.ViewName); // Returns the same view
    //         Assert.IsFalse(result.ViewData.ModelState.IsValid);
    //         Assert.AreEqual(1, result.ViewData.ModelState.ErrorCount);
    //         Assert.IsTrue(result.ViewData.ModelState.ContainsKey("Name"));
    //     }
    // }

    [Test]
    public void JoinRide_RideNotFound_ReturnsNotFoundResult()
    {
        using (var dbContext = new RideSharingDbContext(_dbContextOptions))
        {
            // Arrange
            var slotController = new SlotController(dbContext);
            var commuter = new Commuter
            {
                Name = "John Doe",
                Email = "johndoe@example.com",
                Phone = "1234567890"
            };

            // Act
            var result = slotController.JoinRide(2, commuter) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }

    [Test]
    public void JoinRide_MaximumCapacityReached_ThrowsException()
    {
        using (var dbContext = new RideSharingDbContext(_dbContextOptions))
        {
            // Arrange
            var slotController = new SlotController(dbContext);
            var commuter1 = new Commuter
            {
                Name = "John Doe",
                Email = "johndoe@example.com",
                Phone = "1234567890"
            };

            var commuter2 = new Commuter
            {
                Name = "Jane Smith",
                Email = "janesmith@example.com",
                Phone = "9876543210"
            };

            var ride = dbContext.Rides.Include(r => r.Commuters).FirstOrDefault(r => r.RideID == 1);
            ride.Commuters.Add(commuter1);
            ride.Commuters.Add(commuter2);
            ride.MaximumCapacity = 2;

            dbContext.SaveChanges();

            var commuter3 = new Commuter
            {
                Name = "Alice Johnson",
                Email = "alicejohnson@example.com",
                Phone = "5555555555"
            };

            // Act & Assert
            Assert.Throws<RideSharingException>(() => slotController.JoinRide(1, commuter3));
        }
    }

    [Test]
    public void JoinRide_MaximumCapacityReached_ThrowsExceptionwith_message()
    {
        using (var dbContext = new RideSharingDbContext(_dbContextOptions))
        {
            // Arrange
            var slotController = new SlotController(dbContext);
            var commuter1 = new Commuter
            {
                Name = "John Doe",
                Email = "johndoe@example.com",
                Phone = "1234567890"
            };

            var commuter2 = new Commuter
            {
                Name = "Jane Smith",
                Email = "janesmith@example.com",
                Phone = "9876543210"
            };

            var ride = dbContext.Rides.Include(r => r.Commuters).FirstOrDefault(r => r.RideID == 1);
            ride.Commuters.Add(commuter1);
            ride.Commuters.Add(commuter2);
            ride.MaximumCapacity = 2;

            dbContext.SaveChanges();

            var commuter3 = new Commuter
            {
                Name = "Alice Johnson",
                Email = "alicejohnson@example.com",
                Phone = "5555555555"
            };

            // Act & Assert
            var ex = Assert.Throws<RideSharingException>(() => slotController.JoinRide(1, commuter3));
            Assert.AreEqual("Maximum capacity reached", ex.Message);

        }
    }

//     [Test]
// public void JoinRide_DestinationSameAsDeparture_ReturnsViewWithValidationError()
// {
//     using (var dbContext = new RideSharingDbContext(_dbContextOptions))
//     {
//         // Arrange
//         var slotController = new SlotController(dbContext);
//         var commuter = new Commuter
//         {
//             Name = "John Doe",
//             Email = "johndoe@example.com",
//             Phone = "1234567890"
//         };

//         // Act
//         var ride = dbContext.Rides.FirstOrDefault(r => r.RideID == 1);
//         ride.Destination = ride.DepartureLocation; // Set the destination as the same as departure
//         dbContext.SaveChanges();

//         var result = slotController.JoinRide(1, commuter) as ViewResult;

//         // Assert
//         Assert.IsNotNull(result);
//         Assert.IsFalse(result.ViewData.ModelState.IsValid);
//         Assert.IsTrue(result.ViewData.ModelState.ContainsKey("Destination"));
//     }
// }

// [Test]
// public void JoinRide_MaximumCapacityNotPositiveInteger_ReturnsViewWithValidationError()
// {
//     using (var dbContext = new RideSharingDbContext(_dbContextOptions))
//     {
//         // Arrange
//         var slotController = new SlotController(dbContext);
//         var commuter = new Commuter
//         {
//             Name = "John Doe",
//             Email = "johndoe@example.com",
//             Phone = "1234567890"
//         };

//         // Act
//         var ride = dbContext.Rides.FirstOrDefault(r => r.RideID == 1);
//         ride.MaximumCapacity = -5; // Set a negative value for MaximumCapacity
//         dbContext.SaveChanges();

//         var result = slotController.JoinRide(1, commuter) as ViewResult;

//         // Assert
//         Assert.IsNotNull(result);
//         Assert.IsFalse(result.ViewData.ModelState.IsValid);
//         Assert.IsTrue(result.ViewData.ModelState.ContainsKey("MaximumCapacity"));
//     }
// }


     [Test]
public void SlotClassExists()
{
    var ride = new Ride();

    Assert.IsNotNull(ride);
}

[Test]
public void BookingClassExists()
{
    var commuter = new Commuter();

    Assert.IsNotNull(commuter);
}



[Test]
public void ApplicationDbContextContainsDbSetSlotProperty()
{
    // var context = new ApplicationDbContext();
using (var dbContext = new RideSharingDbContext(_dbContextOptions))
        {
    var propertyInfo = dbContext.GetType().GetProperty("Rides");

    Assert.IsNotNull(propertyInfo);
    Assert.AreEqual(typeof(DbSet<Ride>), propertyInfo.PropertyType);
        }
}

[Test]
public void ApplicationDbContextContainsDbSetBookingProperty()
{
    // var context = new ApplicationDbContext();
    using (var dbContext = new RideSharingDbContext(_dbContextOptions))
        {

    var propertyInfo = dbContext.GetType().GetProperty("Commuters");

    Assert.IsNotNull(propertyInfo);
    Assert.AreEqual(typeof(DbSet<Commuter>), propertyInfo.PropertyType);
}
}
}

}