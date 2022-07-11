using System.Linq.Expressions;
using System.Reflection;

namespace DependencyInjection
{
    public class ReflectionBasedActivationBuilder : BasedActivationBuilder
    {
        protected override Func<IScope, object> BuildActivationInternal(TypeBasedServiceDescriptor tb,
                                                                        ConstructorInfo ctor,
                                                                        ParameterInfo[] args,
                                                                        ServiceDescriptor descriptor)
        {
            return s =>
            {
                var argsForCtor = new object[args.Length];

                for (int i = 0; i < args.Length; i++)
                {
                    argsForCtor[i] = s.Resolve(args[i].ParameterType);
                }

                return ctor.Invoke(argsForCtor);
            };
        }
    }   
}
