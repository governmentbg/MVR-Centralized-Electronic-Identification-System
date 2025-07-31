namespace eID.PJS.Services.LogFileMover
{
    public class LogFileMoverSettings
    {
        public int OrphanFilesMoveIntervalInMinutes { get; set; } = 120;

        public void Validate()
        {
            if (OrphanFilesMoveIntervalInMinutes <= 0)
            {
                throw new ArgumentNullException(nameof(OrphanFilesMoveIntervalInMinutes));
            }
        }
        }
}
