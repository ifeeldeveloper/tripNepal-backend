using AdminPanelTN.DAL;
using AdminPanelTN.DTO;
using AdminPanelTN.Model;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AdminPanelTN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DestinationController : ControllerBase
    {
        private readonly TripNepalDbContext _db;

        public DestinationController(TripNepalDbContext _context)
        {
            _db = _context;
        }
        // GET: api/<DestinationController>
        [HttpGet]
        public IActionResult Get()
        {
            var destinationList = _db.Destinations.Where(x => x.IsActive == true).ToList();
            return Ok(destinationList);
        }

        // GET api/<DestinationController>/5
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var destination = _db.Destinations.Where(x => x.Id == id).Where(x => x.IsActive == true).FirstOrDefault();

            return Ok(destination);
        }

        // POST api/<DestinationController>
        [HttpPost]
        public void Add([FromBody] DestinationDTO destinations)
        {

            Destination newDestination = new Destination();
            newDestination.Id = destinations.Id;
            newDestination.Photo = destinations.Photo;
            newDestination.Name = destinations.Name;
            newDestination.Description = destinations.Description;
            newDestination.Latitude = destinations.Latitude;
            newDestination.Longitude = destinations.Longitude;
            newDestination.IsActive = true;

            _db.Add(newDestination);
            _db.SaveChanges();
        }

        // PUT api/<DestinationController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] DestinationDTO destinationDTO)
        {
            var editDestination = _db.Destinations.Where(x => x.Id == id).FirstOrDefault();

            editDestination.Photo = destinationDTO.Photo;
            editDestination.Name = destinationDTO.Name;
            editDestination.Description = destinationDTO.Description;
            editDestination.Latitude = destinationDTO.Latitude;
            editDestination.Longitude = destinationDTO.Longitude;
            editDestination.IsActive = destinationDTO.IsActive;

            _db.SaveChanges();
            return Ok(editDestination);
        }

        // DELETE api/<DestinationController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            var destination = _db.Destinations.Where(x => x.Id == id).FirstOrDefault();
            destination.IsActive = false;
            _db.SaveChanges();
        }
    }
}
