using SlaveBackend.Models;

namespace SlaveBackend.Services
{
    public class GameTemplateService
    {
        private readonly List<GameTemplate> _gameTemplates = new List<GameTemplate>();

        public IEnumerable<GameTemplate> GetAllGameTemplates()
        {
            return _gameTemplates;
        }

        public GameTemplate? GetGameTemplateById(int id)
        {
            return _gameTemplates.FirstOrDefault(gt => gt.Id == id);
        }

        public void AddGameTemplate(GameTemplate gameTemplate)
        {
            gameTemplate.Id = _gameTemplates.Count + 1; // Auto-increment ID
            _gameTemplates.Add(gameTemplate);
        }

        public void UpdateGameTemplate(int id, GameTemplate updatedTemplate)
        {
            var existingTemplate = GetGameTemplateById(id);
            if (existingTemplate != null)
            {
                existingTemplate.Game = updatedTemplate.Game;
                existingTemplate.Commands = updatedTemplate.Commands;
                existingTemplate.Arguments = updatedTemplate.Arguments;
                existingTemplate.Logging = updatedTemplate.Logging;
            }
        }

        public void DeleteGameTemplate(int id)
        {
            var template = GetGameTemplateById(id);
            if (template != null)
            {
                _gameTemplates.Remove(template);
            }
        }
    }
}
