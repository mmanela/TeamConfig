using System.Text;

namespace TeamConfig.Razor
{
    public abstract class RazorTemplateBase
    {
        public StringBuilder Buffer { get; set; }
        public string Thing = "hello";
        public dynamic Model { get; set; }

        protected RazorTemplateBase()
        {
            Buffer = new StringBuilder();
        }

        public virtual void Execute()
        {
        }

        public virtual void Write(object value)
        {
            WriteLiteral(value);
        }

        public virtual void WriteLiteral(object value)
        {
            Buffer.Append(value);
        }
    }
}