using BracketingApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace BracketingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PairsController : ControllerBase
    {
        private static List<Pair<string, string>> _pairs = new List<Pair<string, string>>();
        private static List<string> _individuals = new List<string>();
        private static List<string> _winners = new List<string>();
        private static List<string> _losers = new List<string>(); //just build-in for later. 
        private static int _round = 0;

        // GET: api/pairs
        [HttpGet]
        public ActionResult<IEnumerable<Pair<string, string>>> GetPairs()
        {
            return Ok(_pairs);
        }

        // POST: api/pairs
        [HttpPost]
        public ActionResult<IEnumerable<Pair<string, string>>> PostPerson([FromBody] string person)
        {
            if (string.IsNullOrWhiteSpace(person))
            {
                return BadRequest("Person is null or empty.");
            }

            _individuals.Add(person);


            if (_individuals.Count >= 2)
            {
                // Create a new pair from the first two individuals
                var pair = new Pair<string, string>(_individuals[0], _individuals[1]);  //this should be randomized. after first - winners against e.o.
                _pairs.Add(pair);

                // Remove the paired individuals from the list
                _individuals.RemoveRange(0, 2);
            }

            return Ok(_pairs);
        }

        [HttpPost("recordresult")]
        public ActionResult RecordResult([FromBody] PairResultDto resultDto)
        {
            var pair = resultDto.Pair;
            var winner = resultDto.Winner;
            var loser = resultDto.Loser; 

            if (!_pairs.Contains(pair))
            {
                return BadRequest("Pair not found.");
            }

            if (pair.First != winner && pair.Second != winner)
            {
                return BadRequest("Winner is not part of the pair.");
            }

            _winners.Add(winner);
            _losers.Add(loser); 


            return Ok();
        }

        [HttpGet("getwinners")]
        public ActionResult GetWinners()
        { return Ok(_winners); }

        [HttpGet("getlosers")]
        public ActionResult GetLosers()  //just build-in for later
        { return Ok(_losers); }


        // POST: api/pairs/nextround
        [HttpPost("nextround")]
        public ActionResult<IEnumerable<Pair<string, string>>> NextRound()
        {
            // Clear the current pairs and set individuals list to winners list
            _pairs.Clear();
            _individuals = new List<string>(_winners);
            _winners.Clear();
           // _losers.Clear();

            _round++;

            // Create new pairs for the next round
            for (int i = 0; i < _individuals.Count; i += 2)
            {
                if (i + 1 < _individuals.Count)
                {
                    var pair = new Pair<string, string>(_individuals[i], _individuals[i + 1]);
                    _pairs.Add(pair);
                }
                else
                {
                    // If there's an odd number of individuals, the last one goes to the next round automatically?! -- only solution? or  ?? 
                    _winners.Add(_individuals[i]);
                }
            }

            return Ok(_pairs);
        }
    }
}
