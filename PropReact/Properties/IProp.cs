namespace PropReact.Properties;


// todo: IGetOnlyProp
public interface IProp<TValue> : IValueOwner
{
    public TValue V { get; set; }
}