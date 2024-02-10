using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RESTAPI.Data;
using RESTAPI.Model;
using RESTAPI.Model.DTO;

namespace RESTAPI.Controllers
{
    [Route("api/Villa")]
    [ApiController]
    public class VillaController : ControllerBase
    {
        private readonly ILogger<VillaController> _logger;
        private readonly AppDbContext _db;

        public VillaController(AppDbContext db, ILogger<VillaController> logger)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet]
        public ActionResult<IEnumerable<VillaDto>> GetVillas()
        {
            _logger.LogInformation("Getting All Villa");
            return Ok(_db.Villas.ToList());
        }

        [HttpGet("{id:int}" , Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(200, Type = typeof(VillaDto))]
        public ActionResult<VillaDto> GetVilla(int id)
        {
            var villa = _db.Villas.FirstOrDefault(x => x.Id == id);
            if (id == 0)
            {
                _logger.LogError($"Error Input With Id {id}");
                return BadRequest();
            }

            if (villa == null)
            {
                return NotFound();
            }
            return Ok(villa);
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<VillaDto> CreateVilla([FromBody]VillaDto villaDto)
        {
            //Isted of [APICONTROLER]
            /*if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }*/

            if( _db.Villas.FirstOrDefault(x => x.FName.ToLower() == villaDto.FName) != null)
            {
                ModelState.AddModelError("NotUnieqError", "Villa Already Exist!");
                return BadRequest(ModelState);
            }
            if(villaDto == null)
            {
                return BadRequest();
            }
            if(villaDto.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            Villa model = new()
            {
                Amenity = villaDto.Amenity,
                Details = villaDto.Details,
                ImageUrl = villaDto.ImageUrl,
                FName = villaDto.FName,
                LName = villaDto.LName,
                Occupancy = villaDto.Occupancy,
                Rate = villaDto.Rate,
                Sqft = villaDto.Sqft
            };
            _db.Villas.Add(model);
            _db.SaveChanges();

            return CreatedAtRoute("GetVilla", new {id = villaDto.Id }, villaDto);

        }


        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult DeleteVilla(int id)
        {
            if(id == 0)
            {
                return BadRequest();
            }
            var villa = _db.Villas.FirstOrDefault(x => x.Id == id);

            if (villa == null)
            {
                return NotFound();
            }
            _db.Villas.Remove(villa);
            _db.SaveChanges();
            return NoContent();
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult UpdateVilla(int id, [FromBody]VillaDto villaDto)
        {
            if (villaDto == null || id != villaDto.Id)
            {
                return BadRequest();
            }
            
            Villa model = new()
            {
                Id = villaDto.Id,
                Amenity = villaDto.Amenity,
                Details = villaDto.Details,
                ImageUrl = villaDto.ImageUrl,
                FName = villaDto.FName,
                LName = villaDto.LName,
                Occupancy = villaDto.Occupancy,
                Rate = villaDto.Rate,
                Sqft = villaDto.Sqft
            };
            _db.Villas.Update(model);
            _db.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDto> patchDTO)
        {
            if (patchDTO == null || id == 0)
            {
                return BadRequest();
            }
            var villa = _db.Villas.AsNoTracking().FirstOrDefault(x => x.Id == id);
            if (villa == null)
            {
                return BadRequest();
            }
            VillaDto villaDto = new()
            {
                Id = villa.Id,
                Amenity = villa.Amenity,
                Details = villa.Details,
                ImageUrl = villa.ImageUrl,
                FName = villa.FName,
                LName = villa.LName,
                Occupancy = villa.Occupancy,
                Rate = villa.Rate,
                Sqft = villa.Sqft
            };

            patchDTO.ApplyTo(villaDto, ModelState);

            Villa model = new()
            {
                Id = villaDto.Id,
                Amenity = villaDto.Amenity,
                Details = villaDto.Details,
                ImageUrl = villaDto.ImageUrl,
                FName = villaDto.FName,
                LName = villaDto.LName,
                Occupancy = villaDto.Occupancy,
                Rate = villaDto.Rate,
                Sqft = villaDto.Sqft
            };
            _db.Villas.Update(model);
            _db.SaveChanges();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return NoContent();
        }


    }
}