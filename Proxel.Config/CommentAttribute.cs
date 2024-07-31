namespace Proxel.Config
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class CommentAttribute : Attribute
    {
        public string Text { get; }

        public CommentAttribute(string text)
        {
            Text = text;
        }
    }
}
