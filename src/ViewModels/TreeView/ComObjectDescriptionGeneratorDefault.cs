using System;
using System.Text;
using System.Reflection;

namespace ArcmapSpy.ViewModels.TreeView
{
    public class ComObjectDescriptionGeneratorDefault : IComObjectDescriptionGenerator
    {
        public string GererateDescription(object obj, Type typ)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(typ.Name);
            sb.AppendLine();

            sb.AppendLine("*** Properties ***");
            sb.AppendLine();
            PropertyInfo[] propertyInfos = typ.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo propInfo in propertyInfos)
            {
                string propertyName = propInfo.Name;
                string propertyType = propInfo.PropertyType.Name;
                string propertyReadWrite = (propInfo.CanRead ? "r" : "-") + (propInfo.CanWrite ? "w" : "-");

                string propertyValue = string.Empty;
                if (propInfo.CanRead && (propInfo.PropertyType != typeof(object)))
                {
                    try
                    {
                        // Calling GetValue on a PropertyType Object can lead to ArcMap aborts.
                        propertyValue = propInfo.GetValue(obj, null).ToString();
                    }
                    catch (Exception ex)
                    {
                        propertyValue = string.Format("exception->'{0}'", ex.Message);
                    }
                }

                sb.AppendFormat("{0} [{1} {2}]: {3}", propertyName, propertyType, propertyReadWrite, propertyValue);
                sb.AppendLine();
            }
            sb.AppendLine();

            // public methods auslesen
            sb.AppendLine("*** Methods ***");
            sb.AppendLine();
            MethodInfo[] methodInfos = typ.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            foreach (MethodInfo methodInfo in methodInfos)
            {
                bool isPropertyAccessor = methodInfo.IsSpecialName && (methodInfo.Name.StartsWith("set_") || methodInfo.Name.StartsWith("get_"));
                if (!isPropertyAccessor)
                {
                    sb.AppendFormat("{0}(...) [{1}]", methodInfo.Name, methodInfo.ReturnType.Name);
                    sb.AppendLine();
                }
            }

            sb.AppendLine();
            return sb.ToString();
        }
    }
}
