using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
// using gym.Data;
using RideShare.Exceptions; 

using RideShare.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RideShare.Controllers
{

public class SlotController : Controller
{
    private readonly RideSharingDbContext _dbContext;

    public SlotController(RideSharingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IActionResult JoinRide(int rideId)
    {
        var ride = _dbContext.Rides.Include(r => r.Commuters).FirstOrDefault(r => r.RideID == rideId);
        if (ride == null)
        {
            return NotFound();
        }

        int joinedCommuters = ride.Commuters.Count;
        if (joinedCommuters >= ride.MaximumCapacity)
        {
            throw new RideSharingException("Maximum capacity reached");
        }

        ViewBag.RideId = rideId;

        return View();
    }
[HttpPost]
public IActionResult JoinRide(int rideId, Commuter commuter)
{
    var ride = _dbContext.Rides.Include(r => r.Commuters).FirstOrDefault(r => r.RideID == rideId);
    if (ride == null)
    {
        return NotFound();
    }

    int joinedCommuters = ride.Commuters.Count;
    if (joinedCommuters >= ride.MaximumCapacity)
    {
        throw new RideSharingException("Maximum capacity reached");
    }

    // if (ModelState.IsValid)
    {
        commuter.RideID = rideId;
        _dbContext.Commuters.Add(commuter);
        _dbContext.SaveChanges();

        // ride.MaximumCapacity--; // Decrease available seats
        _dbContext.SaveChanges();

        return RedirectToAction("Details", "Ride", new { id = rideId });
    }

    return View(commuter);
}

}
}