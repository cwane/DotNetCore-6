using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using System.Xml.Linq;

namespace Web_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuperHeroController : ControllerBase
    {

        private readonly DataContext _context;
        public SuperHeroController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]

        public async Task<ActionResult<List<SuperHero>>> Get()
        {
            return Ok(await _context.SuperHeroes.ToListAsync());
        }

        //Find a fixed usercode
        [HttpGet("SP")]
        //create or alter procedure UspGetUsers
        //(
        //    @Name varchar(50)
        //)
        //as
        //begin

        //    select*
        //    from SuperHeroes
        //    where Name = @Name

        //end
        //go
        //exec UspGetUsers @Name = 'shiwani'
        public async Task<ActionResult<List<SuperHero>>> GetSP()
        {
            var nameParameter = new SqlParameter("@Name", "Shiwani");
            var result = await _context.SuperHeroes.FromSqlRaw("EXEC UspGetUsers @Name", nameParameter).ToListAsync();
            return Ok(result);
        }

        //Display all Users
        [HttpGet("USP")]

        //create or alter procedure UspUsers
        //as
        //begin

        //    select*
        //    from SuperHeroes


        //end

        public async Task<ActionResult<List<SuperHero>>> GetUSP()
        {
            
            var result = await _context.SuperHeroes.FromSqlRaw("EXEC UspUsers").ToListAsync();
            return Ok(result);
        }

        //Find Specific User
        [HttpGet("UserSP")]

        //create or alter procedure UspGetUserDetail
        //(
        //    @Name varchar(50) = null
        //)
        //as
        //begin
	       // if @Name is not null
	       // begin

        //        select*
        //        from SuperHeroes
        //        where Name LIKE '%' + @Name + '%'
	       // end
	       // else
	       // begin

        //        select*
        //        from SuperHeroes
        //    end
        //end
        public async Task<ActionResult<List<SuperHero>>> GetUserSP(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                // Call stored procedure without passing the name parameter
                return Ok(await _context.SuperHeroes.FromSqlRaw("EXEC UspGetUsers").ToListAsync());
            }
            else
            {
                // Call stored procedure and pass the name parameter
                var result = await _context.SuperHeroes.FromSqlRaw("EXEC UspGetUsers @Name", new SqlParameter("@Name", name)).ToListAsync();
                return Ok(result);
            }
        }


        [HttpGet("{id}")]

        public async Task<ActionResult<SuperHero>> Get(int id)
        {
            var hero = await _context.SuperHeroes.FindAsync(id);
            if (hero == null)

                return BadRequest("Hero not found");

            return Ok(hero);
        }

        [HttpPost]
        public async Task<ActionResult<List<SuperHero>>> AddHero(SuperHero hero)
        {
            _context.SuperHeroes.Add(hero);
            await _context.SaveChangesAsync();
            return Ok(await _context.SuperHeroes.ToListAsync());
        }

        [HttpPut]
        public async Task<ActionResult<List<SuperHero>>> UpdateHero(SuperHero request)
        {
            var dbHero = await _context.SuperHeroes.FindAsync(request.Id);
            if (dbHero == null)

                return BadRequest("Hero not found");
            dbHero.Name = request.Name;
            dbHero.FirstName = request.FirstName;
            dbHero.LastName = request.LastName;
            dbHero.Place = request.Place;

            await _context.SaveChangesAsync();

            return Ok(await _context.SuperHeroes.ToListAsync());
        }


        [HttpDelete]
        public async Task<ActionResult<List<SuperHero>>> DeleteHero(int id)
        {
            var dbHero = await _context.SuperHeroes.FindAsync(id);
            if (dbHero == null)

                return BadRequest("Hero not found");
            _context.SuperHeroes.Remove(dbHero);
            await _context.SaveChangesAsync();
            return Ok(await _context.SuperHeroes.ToListAsync());



        }
    }
}
