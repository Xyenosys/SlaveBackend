using System.Runtime.Versioning;

namespace SlaveBackend.Models
{
    public class Hostinfo
    {
        public string? OSDescription { get; set; }
        public string? OSArchitecture { get; set; }
        public string MachineName { get; set; }
        public int? ProcessorCount { get; set; }
        public string? Framework { get; set; }
        public int? DriveCount { get; set; }
        public ulong? TotalMemory { get; set; }
    }
}
