namespace System
{
#if NET20 || NET40
    public interface IProgress<T>
    {
        void Report(T data);
    }
#endif
}