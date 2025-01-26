namespace SlaveBackend.Models
{
    public class GameInstance
    {
        public Guid Id { get; set; } = Guid.NewGuid(); // Unique identifier for the instance
        public string TemplateName { get; set; } = string.Empty; // Reference to the template
        public Dictionary<string, GameArgument> Arguments { get; set; } = new Dictionary<string, GameArgument>(); // Instance-specific configuration
        public DateTime StartTime { get; set; } = DateTime.Now; // Track when the instance started
        public bool IsRunning { get; set; } = false;
    }
}
