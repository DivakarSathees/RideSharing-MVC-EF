using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RideShare.Models

{
    public class Ride
{
    public int RideID { get; set; }
    public string DepartureLocation { get; set; }
    public string Destination { get; set; }
    public DateTime DateTime { get; set; }
    
    [System.ComponentModel.DataAnnotations.Range(1, int.MaxValue, ErrorMessage = "Maximum capacity must be a positive integer.")]
    public int MaximumCapacity { get; set; }
    public List<Commuter> Commuters { get; set; }
}
}