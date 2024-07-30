using BracketingApp.Models;
using BracketingApp.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace BracketingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PairsController : ControllerBase
    {
        private static List<Pair<string, string>> _pairs = new List<Pair<string, string>>();
        private static List<string> _individuals = new List<string>();
        private static List<string> _winners = new List<string>();
        private static List<string> _losers = new List<string>();
        private static int _round = 0;

        private readonly IndividualsService _individualsService;

        public PairsController(IndividualsService individualsService)
        {
            _individualsService = individualsService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<string>> GetPersons()
        {
            var individuals = _individualsService.GetIndividuals();
            return Ok(individuals);
        }

        [HttpPost]
        public ActionResult<string> PostPerson([FromBody] string person)
        {
            if (string.IsNullOrWhiteSpace(person))
            {
                return BadRequest("Person is null or empty.");
            }

            _individualsService.AddIndividual(person);
            return Ok(person);
        }

        [HttpGet("getpairswithresults")]
        public ActionResult<IEnumerable<PairResultDto>> GetPairsWithResults()
        {
            var resultPairs = _pairs.Select(p => new PairResultDto
            {
                First = p.First,
                Second = p.Second,
                Winner = _winners.Contains(p.First) ? p.First : (_winners.Contains(p.Second) ? p.Second : null),
                Loser = _losers.Contains(p.First) ? p.First : (_losers.Contains(p.Second) ? p.Second : null)
            }).ToList();

            return Ok(resultPairs);
        }

        [HttpPost("updatepairresult")]
        public ActionResult UpdatePairResult([FromBody] PairResultDto updatedResultDto)
        {
            if (updatedResultDto == null)
            {
                return BadRequest("Result data is null.");
            }

            var pair = _pairs.FirstOrDefault(p =>
                (p.First == updatedResultDto.First && p.Second == updatedResultDto.Second) ||
                (p.First == updatedResultDto.Second && p.Second == updatedResultDto.First));

            if (pair == null)
            {
                return BadRequest("Pair not found.");
            }

            if (updatedResultDto.First != updatedResultDto.Winner && updatedResultDto.Second != updatedResultDto.Winner)
            {
                return BadRequest("Winner must be one of the pair members.");
            }

            if (!_winners.Contains(updatedResultDto.Winner))
            {
                _winners.Add(updatedResultDto.Winner);
            }

            if (updatedResultDto.First != updatedResultDto.Loser && updatedResultDto.Second != updatedResultDto.Loser)
            {
                return BadRequest("Loser must be one of the pair members.");
            }

            if (!_losers.Contains(updatedResultDto.Loser))
            {
                _losers.Add(updatedResultDto.Loser);
            }

            return Ok(new { Message = "Pair result updated successfully." });
        }


        [HttpPost("nextround")]
        public ActionResult<IEnumerable<Pair<string, string>>> NextRound()
        {
            _pairs.Clear();
            _individuals = new List<string>(_winners);
            _winners.Clear();
            _losers.Clear();

            _round++;

            for (int i = 0; i < _individuals.Count; i += 2)
            {
                if (i + 1 < _individuals.Count)
                {
                    var pair = new Pair<string, string>(_individuals[i], _individuals[i + 1]);
                    _pairs.Add(pair);
                }
            }

            return Ok(_pairs);
        }

        [HttpGet("getrandompairs")]
        public ActionResult<IEnumerable<PairResultDto>> GetRandomPairs()
        {
            var individuals = _individualsService.GetIndividuals();

            if (individuals == null || !individuals.Any())
            {
                return BadRequest("No participants available to pair.");
            }

            var random = new Random();
            var shuffledIndividuals = individuals.OrderBy(x => random.Next()).ToList();

            var pairs = new List<PairResultDto>();
            for (int i = 0; i < shuffledIndividuals.Count; i += 2)
            {
                if (i + 1 < shuffledIndividuals.Count)
                {
                    pairs.Add(new PairResultDto
                    {
                        First = shuffledIndividuals[i],
                        Second = shuffledIndividuals[i + 1]
                    });
                }
                else
                {
                    pairs.Add(new PairResultDto
                    {
                        First = shuffledIndividuals[i],
                        Second = null
                    });
                }
            }

            SavePairs(pairs.Select(p => new Pair<string, string>(p.First, p.Second)).ToList());
            return Ok(pairs);
        }

        [HttpGet("getpairs")]
        public ActionResult<IEnumerable<Pair<string, string>>> GetPairs()
        {
            if (!_pairs.Any())
            {
                return NoContent();
            }

            return Ok(_pairs);
        }

        [HttpPost("savepairs")]
        public ActionResult SavePairs([FromBody] List<Pair<string, string>> pairs)
        {
            if (pairs == null || !pairs.Any())
            {
                return BadRequest("Pairs are null or empty.");
            }

            _pairs = pairs;

            return Ok(new { Message = "Pairs saved successfully.", SavedPairs = _pairs });
        }
    }
}
