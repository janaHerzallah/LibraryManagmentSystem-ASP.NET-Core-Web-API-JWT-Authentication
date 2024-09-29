namespace LibraryManagmentSystem.Interfaces
{
    public interface IExcelService
    {
        byte[] GenerateExcelSheet<T>(IEnumerable<T> data, string sheetName);
    }
}
