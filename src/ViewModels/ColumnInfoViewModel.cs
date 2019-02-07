using System.Text;
using ArcmapSpy.Utils;

namespace ArcmapSpy.ViewModels
{
    public class ColumnInfoViewModel
    {
        public string ColumnName { get; set; }
        public string Alias { get; set; }
        public string Value { get; set; }
        public string FieldType { get; set; }
        public bool Required { get; set; }
        public bool Editable { get; set; }
        public bool Nullable { get; set; }
        public int Length { get; set; }
        public int Precision { get; set; }
        public bool IsSubtype { get; set; }
        public KeyValueList<int, string> SubtypeValues { get; set; }
        public KeyValueList<object, string> DomainValues { get; set; }

        public string ColumnToolTip
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Fieldtype: " + FieldType);
                sb.AppendLine("Required: " + Required);
                sb.AppendLine("Editable: " + Editable);
                sb.AppendLine("Nullable: " + Nullable);
                sb.AppendLine("Length: " + Length);
                sb.AppendLine("Precision: " + Precision);
                sb.AppendLine("Alias: " + Alias);
                sb.AppendLine("IsSubtype: " + IsSubtype);

                if (SubtypeValues != null)
                {
                    sb.AppendLine();
                    sb.AppendLine("Subtype values:");
                    foreach (var lookupValue in SubtypeValues)
                        sb.AppendLine(lookupValue.Key + ": " + lookupValue.Value);
                }

                if (DomainValues != null)
                {
                    sb.AppendLine();
                    sb.AppendLine("Domain values:");
                    foreach (var domainValue in DomainValues)
                        sb.AppendLine(domainValue.Key + ": " + domainValue.Value);
                }
                return sb.ToString();
            }
        }
    }
}