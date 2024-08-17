using BracketingApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace BracketingApp.Services
{
    public class PairsService
    {
        private static List<Pair<string, string>> _pairs = new List<Pair<string, string>>();
        private static List<string> _individuals = new List<string>();
        private static List<string> _winners = new List<string>();
        private static List<string> _losers = new List<string>();
        private static int _round = 0;

        public IEnumerable<string> GetIndividuals() => _individuals;

        public void AddIndividual(string person)
        {
            if (!_individuals.Contains(person))
            {
                _individuals.Add(person);
            }
        }

        public IEnumerable<PairResultDto> GetPairsWithResults()
        {
            return _pairs.Select(p => new PairResultDto
            {
                First = p.First,
                Second = p.Second,
                Winner = _winners.Contains(p.First) ? p.First : (_winners.Contains(p.Second) ? p.Second : null),
                Loser = _losers.Contains(p.First) ? p.First : (_losers.Contains(p.Second) ? p.Second : null)
            }).ToList();
        }

        public bool UpdatePairResult(PairResultDto updatedResultDto)
        {
            var pair = _pairs.FirstOrDefault(p =>
                (p.First == updatedResultDto.First && p.Second == updatedResultDto.Second) ||
                (p.First == updatedResultDto.Second && p.Second == updatedResultDto.First));

            if (pair == null)
            {
                return false; // Pair not found.
            }

            if (updatedResultDto.First != updatedResultDto.Winner && updatedResultDto.Second != updatedResultDto.Winner)
            {
                return false; // Winner must be one of the pair members.
            }

            if (!_winners.Contains(updatedResultDto.Winner))
            {
                _winners.Add(updatedResultDto.Winner);
            }

            if (updatedResultDto.First != updatedResultDto.Loser && updatedResultDto.Second != updatedResultDto.Loser)
            {
                return false; // Loser must be one of the pair members.
            }

            if (!_losers.Contains(updatedResultDto.Loser))
            {
                _losers.Add(updatedResultDto.Loser);
            }

            return true;
        }

        public IEnumerable<Pair<string, string>> NextRound()
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

            return _pairs;
        }

        public IEnumerable<PairResultDto> GetRandomPairs()
        {
            var random = new Random();
            var shuffledIndividuals = _individuals.OrderBy(x => random.Next()).ToList();

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
                        Second = "No partner"
                    });
                }
            }

            SavePairs(pairs.Select(p => new Pair<string, string>(p.First, p.Second)).ToList());
            return pairs;
        }

        public IEnumerable<Pair<string, string>> GetPairs()
        {
            return _pairs;
        }

        public void SavePairs(List<Pair<string, string>> pairs)
        {
            if (pairs != null && pairs.Any())
            {
                _pairs = pairs;
            }
        }

        public void DeletePairs()
        {
            _pairs.Clear();
                
        }

        public void ClearIndividuals()
        {
            _individuals.Clear();
        }
    }
}
