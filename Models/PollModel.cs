

using System.Collections.Generic;


namespace Telegram_WPF.Models
{
    internal class PollModel
    {
        public long? Id { get; set; } = null;
        public string? PollQuestion { get; set; } = null;
        public string? PollSolution { get; set; } = null;
        public List<string?>? PollAnswers { get; set; } = new List<string?>();
        public List<int?>? PollResults { get; set; } = new List<int?>();//count of voters
        public int? PollTotalVoters { get; set; } = null;
    }
}
