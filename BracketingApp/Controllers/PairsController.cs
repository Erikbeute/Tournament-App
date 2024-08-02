using BracketingApp.Models;
using BracketingApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace BracketingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PairsController : ControllerBase
    {
        private readonly PairsService _pairsService;
        private readonly IndividualsService _individualsService;

        public PairsController(PairsService pairsService, IndividualsService individualsService)
        {
            _pairsService = pairsService;
            _individualsService = individualsService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<string>> GetPersons()
        {
            var individuals = _pairsService.GetIndividuals();
            return Ok(individuals);
        }

        [HttpPost]
        public ActionResult<string> PostPerson([FromBody] string person)
        {
            if (string.IsNullOrWhiteSpace(person))
            {
                return BadRequest("Person is null or empty.");
            }

            _pairsService.AddIndividual(person);
            return Ok(person);
        }

        [HttpGet("getpairswithresults")]
        public ActionResult<IEnumerable<PairResultDto>> GetPairsWithResults()
        {
            var resultPairs = _pairsService.GetPairsWithResults();
            return Ok(resultPairs);
        }

        [HttpPost("updatepairresult")]
        public ActionResult UpdatePairResult([FromBody] PairResultDto updatedResultDto)
        {
            if (updatedResultDto == null)
            {
                return BadRequest();
            }

            var result = _pairsService.UpdatePairResult(updatedResultDto);

            if (!result)
            {
                return BadRequest();
            }

            return Ok(new { Message = "Pair result updated." });
        }

        [HttpPost("nextround")]
        public ActionResult<IEnumerable<Pair<string, string>>> NextRound()
        {
            var pairs = _pairsService.NextRound();
            return Ok(pairs);
        }

        [HttpGet("getrandompairs")]
        public ActionResult<IEnumerable<PairResultDto>> GetRandomPairs()
        {
            var randomPairs = _pairsService.GetRandomPairs();

            if (randomPairs == null || !randomPairs.Any())
            {
                return BadRequest();
            }

            return Ok(randomPairs);
        }

        //useful for debugging. 
        [HttpGet("getpairs")]
        public ActionResult<IEnumerable<Pair<string, string>>> GetPairs()
        {
            var pairs = _pairsService.GetPairs();

            if (pairs == null || !pairs.Any())
            {
                return NoContent();
            }

            return Ok(pairs);
        }

        [HttpPost("savepairs")]
        public ActionResult SavePairs([FromBody] List<Pair<string, string>> pairs)
        {
            if (pairs == null || !pairs.Any())
            {
                return BadRequest();
            }

            _pairsService.SavePairs(pairs);

            return Ok(new { Message = "Pairs saved.", SavedPairs = pairs });
        }


        [HttpDelete("reset")]
        public ActionResult DeleteAll()
        {
            _pairsService.DeletePairs();
            _pairsService.ClearIndividuals();
            return Ok(new { Message = "All participants and pairs deleted." });
        }
    }
}
