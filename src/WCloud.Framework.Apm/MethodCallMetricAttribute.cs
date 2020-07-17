using AspectCore.DynamicProxy;
using Elastic.Apm.Api;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WCloud.Framework.Apm
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MethodCallMetricAttribute : AbstractInterceptorAttribute
    {
        bool __current_trans__(out ITransaction transaction)
        {
            transaction = null;
            try
            {
                transaction = Elastic.Apm.Agent.Tracer.CurrentTransaction;
                return transaction != null;
            }
            catch
            {
                return false;
            }
        }

        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            var apm_actived = context.ServiceProvider?.IsApmAgentConfigured() ?? false;
            if (apm_actived && this.__current_trans__(out var transaction))
            {
                var m = context.ImplementationMethod;
                var type_name = m.DeclaringType.FullName;
                var method_name = m.Name;

                var full_name = $"{type_name}.{method_name}";

                var span = transaction.StartSpan(full_name, type_name);

                var params_data = string.Join(",", m.GetParameters().Select(x => $"{x.ParameterType.Name} {x.Name}"));
                span.Labels["function"] = $"{m.ReturnType.FullName} {method_name}({params_data});";

                try
                {
                    await next.Invoke(context);
                }
                catch (Exception e)
                {
                    span.CaptureException(e);
                    throw;
                }
                finally
                {
                    span.End();
                }
            }
            else
            {
                //如果没有开启apm
                await next.Invoke(context);
            }
        }
    }
}
