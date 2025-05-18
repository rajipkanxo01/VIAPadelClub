namespace VIAPadelClub.Core.Tools.ObjectMapper;

public interface IMappingConfig<in TInput, out TOutput> where TInput : class where TOutput : class
{
    TOutput Map(TInput input);
}