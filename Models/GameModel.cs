
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YamlDotNet.Serialization;

namespace SlaveBackend.Models
{

    public class GameTemplate
    {
        public int Id { get; set; }
        [YamlMember(Alias = "game")]
        public GameInfo? Game { get; set; }


        [YamlMember(Alias = "commands")]
        public Commands? Commands { get; set; }

        // Change to ICollection
        [YamlMember(Alias = "arguments")]
        public ICollection<GameArgument>? Arguments { get; set; } // Using ICollection instead of Dictionary

        [YamlMember(Alias = "logging")]
        public Logging? Logging { get; set; }
    }



    public class GameArgument
    {
        [YamlMember(Alias = "cvar")] // This will act as the key for logical mapping
        public string? Cvar { get; set; }

        [YamlMember(Alias = "description")]
        public string? Description { get; set; }

        [YamlMember(Alias = "default")]
        public object? Default { get; set; }

        [YamlMember(Alias = "type")]
        public string? Type { get; set; }

        [YamlMember(Alias = "required")]
        public bool Required { get; set; }


    }

    public class GameInfo
    {
        [YamlMember(Alias = "name")]
        [Key]
        public string? Name { get; set; }

        [YamlMember(Alias = "version")]
        public string? Version { get; set; }

        [YamlMember(Alias = "developer")]
        public string? Developer { get; set; }

    }

    public class Commands
    {
        [YamlMember(Alias = "start")]
        public string? Start { get; set; }

        [YamlMember(Alias = "stop")]
        public string? Stop { get; set; }

        [YamlMember(Alias = "restart")]
        public string? Restart { get; set; }
    }

    public class Logging
    {
        [Key]
        public int Id { get; set; }

        [YamlMember(Alias = "enable")]
        public bool Enable { get; set; }

        [YamlMember(Alias = "level")]
        public string? Level { get; set; }

        [YamlMember(Alias = "file")]
        public string? File { get; set; }
    }
}