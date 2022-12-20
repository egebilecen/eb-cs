class ListItem
{
    public string Text  { get; set; }
    public object Value { get; set; }

    public ListItem(string text, object obj)
    {
        Text  = text;
        Value = obj;
    }

    public override string ToString()
    {
        return Text;
    }
}
