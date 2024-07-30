namespace BracketingApp.Services
{
    public class IndividualsService
    {
        private readonly List<string> _individuals = new List<string>();

        public List<string> GetIndividuals()
        {
            return _individuals;
        }

        public void AddIndividual(string person)
        {
            if (!string.IsNullOrWhiteSpace(person))
            {
                _individuals.Add(person);
            }
        }

        public void RemoveIndividual(string person)
        {
            if (!string.IsNullOrWhiteSpace(person))
            {
                _individuals.Remove(person);
            }
        }

        public void EditIndividual(string oldName, string newName)
        {
            if (!string.IsNullOrWhiteSpace(oldName) && !string.IsNullOrWhiteSpace(newName))
            {
                int index = _individuals.IndexOf(oldName);
                if (index >= 0)
                {
                    _individuals[index] = newName;
                }
            }
        }

        public string DisplayList()
        {
            return string.Join(", ", _individuals);
        }
    }
}