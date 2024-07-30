using BracketingApp.Models;
using BracketingApp.Services;
using System;

namespace BracketingApp.Services
{
    public class RandomizeService
    {
        private readonly IndividualsService _individualService;

        public RandomizeService(IndividualsService individualService)
        {
            _individualService = individualService;
        }

        public void DisplayIndividuals()
        {
            string individualsList = _individualService.DisplayList();
            Console.WriteLine(individualsList);
        }
    }
}