using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkoutHelper.Shared.Enums;
using WorkoutHelper.Shared.Models;
using WorkoutHelperAPI.Data;

namespace WorkoutHelperAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ExerciseController : ControllerBase
    {
        private readonly AppDBContext _context;
        public ExerciseController(AppDBContext context)
        {
            _context = context;
        }

        [HttpGet("Exercises/{id}")]
        public async Task<ActionResult<List<Exercise>>> GetExercise(int id)
        {
            Exercise? exercise = null;

            try
            {
                exercise = await _context.Exercises.FindAsync(id);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }

            if (exercise == null)
                return NotFound("Exercise not found.");

            return Ok(exercise);
        }

        [HttpGet("Exercises")]
        public async Task<ActionResult<List<Exercise>>> Exercises()
        {
            List<Exercise>? exercises = null;

            try
            {
                exercises = await _context.Exercises.ToListAsync();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }

            if (exercises == null)
                return NotFound("Exercises not found.");

            return Ok(exercises);
        }


        [Authorize(Roles = "TrustedContributor, Admin")]
        [HttpPost("AddExercise")]
        public ActionResult AddExercise(Exercise exerciseInfo)
        {
            if (_context.Exercises.Any(x => x.Name.ToLower() == exerciseInfo.Name.ToLower()))
                return BadRequest();

            exerciseInfo.Id = null;

            _context.Exercises.Add(exerciseInfo);
            _context.SaveChanges();

            return Ok();
        }

    }
}
