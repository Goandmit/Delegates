namespace Delegates
{
    internal interface IStringHandler<T>
        where T : class
    {            
        string ConvertToString(T entity);
        T GetFromStringSplited(string[] stringSplited);        
    }
}
