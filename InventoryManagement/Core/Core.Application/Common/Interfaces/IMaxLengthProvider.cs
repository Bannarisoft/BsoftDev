namespace Core.Application.Common.Interfaces
{
    public interface IMaxLengthProvider
    {
        int? GetMaxLength<T>(string propertyName) where T : class;
    }
}
