namespace MoneyManager.Communication
{
    public class Result<T>
    {
        public T ResultObject { get; set; }
        public ResultInfo Status { get; set; }
        public string FailureMessage { get; set; }
        public string DetailedFailureMessage { get; set; }
    }

    public enum ResultInfo
    {
        Success,
        BadCsvHeader,
        DbUpdateError
    }
}