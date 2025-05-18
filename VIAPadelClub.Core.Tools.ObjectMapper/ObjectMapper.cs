using System.Text.Json;

namespace VIAPadelClub.Core.Tools.ObjectMapper;

public class ObjectMapper(IServiceProvider serviceProvider) : IMapper
{
    public TOutput Map<TOutput>(object input) where TOutput : class
    {
        var inputType = input.GetType();
        var outputType = typeof(TOutput);
        var mappingInterface = typeof(IMappingConfig<,>).MakeGenericType(inputType, outputType);
        var mappingConfig = serviceProvider.GetService(mappingInterface);

        if (mappingConfig != null)
        {
            var method = mappingInterface.GetMethod("Map");
            if (method != null)
            {
                var result = method.Invoke(mappingConfig, new[] { input });
                return (TOutput)result!;
            }
        }

        // fallback to default mapping
        var toJson = JsonSerializer.Serialize(input);
        return JsonSerializer.Deserialize<TOutput>(toJson)!;
    }
}