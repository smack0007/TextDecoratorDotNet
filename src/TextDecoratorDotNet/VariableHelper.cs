using System.Collections;
using System.Linq;

namespace TextDecoratorDotNet
{
    public static class VariableHelper
    {
        public static bool IsFalsyValue(object value)
        {
            if (value == null)
                return true;

            if (value is bool && ((bool)value) == false)
                return true;

            if (value is int && ((int)value) == 0)
                return true;

            if (value is string && ((string)value).Length == 0)
                return true;

            if (value is IEnumerable && !(((IEnumerable)value).Cast<object>().Any()))
                return true;

            return false;
        }
    }
}
