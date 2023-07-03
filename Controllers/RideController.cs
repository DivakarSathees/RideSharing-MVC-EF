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
public class RideController : Controller
{
    private readonly RideSharingDbContext _dbContext;

    public RideController(RideSharingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IActionResult AvailableRides()
{
    var rides = _dbContext.Rides.Include(r => r.Commuters).ToList();
    return View(rides);
}


    public IActionResult Details(int id)
    {
        var ride = _dbContext.Rides.Include(r => r.Commuters).FirstOrDefault(r => r.RideID == id);
        if (ride == null)
        {
            return NotFound();
        }

        int joinedCommuters = ride.Commuters.Count;
        int availableSeats = ride.MaximumCapacity - joinedCommuters;

        ViewBag.JoinedCommuters = joinedCommuters;
        ViewBag.AvailableSeats = availableSeats;

        return View(ride);
    }
}
}